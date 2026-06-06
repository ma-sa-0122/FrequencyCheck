using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Karaoke
{
    public partial class Karaoke_Prototype : Form
    {
        private PitchDetector detector;

        private int samplingRate = 16384;
        private int fftSize = 2048;
        private int measuresPerSec = 30;

        private int samplingInterval = 512;
        const int VALID_MAX_FREQ = 1000;
        private double minEnergy = 1e-4;

        private AudioFileReader music;
        private AudioInputService audioIn;
        private AudioOutputService audioOut;
        private AnalysisService analysisService;

        private volatile float latency = 0f;
        // Live flags copied from UI on UI thread and read atomically by analysis thread
        private volatile bool useFourierFlag = false;
        private volatile bool useHPSFlag = false;
        private PlaybackController playbackController;
        private SettingsController settingsController;

        // マイク入力はバッファサイズが不安定なので、固定長のリングバッファを用意
        double[] ringBuffer_waveIn;
        int ringBuffer_waveInIndex;

        private StripLine playLine; // 再生位置のガイド線

        private Stopwatch stopwatch = new Stopwatch();  // 曲を再生しないときの経過時間

        private SongData currentSong;  // 読み込んだSongData
        private int currentPage = 0;
        // 現ページの秒数範囲
        private double pageStartSec = 0;
        private double pageEndSec = 0;
        // 曲未選択の場合のページ長
        private const double CONST_PAGE_SEC = 5.0;

        private Bitmap noteOverlayBitmap = null;

        private double[] pitchArray;   // ページ単位のピッチ配列
        private int pitchIndex = 0;

        private int arrayRange = 1024;  // 1ページあたりのピッチ描画点数

        // オーバーラップするfftSizeのリングバッファ
        private double[] ringBuffer_fft;
        private int ringBuffer_fftIndex;



        public Karaoke_Prototype()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            InitializeComponent();
            SetDeviceList();
            SetMusicList();
            SetupCharts();

            timerUI.Start();

            // Playback controller: delegates call into this form's methods
            playbackController = new PlaybackController(StartPlayback, StopPlayback, PageForwardPlayback, PageBackPlayback);

            // Settings controller keeps last UI values (updated on UI tick)
            settingsController = new SettingsController();

            // Wire immediate UI changes to settings snapshot to avoid races (selection changes)
            comboBoxDeviceIn.SelectedIndexChanged += (s, e) => settingsController?.SetSelectedDevices(comboBoxDeviceIn.SelectedItem as MMDevice, settingsController?.SelectedOutputDevice);
            comboBoxDeviceOut.SelectedIndexChanged += (s, e) => settingsController?.SetSelectedDevices(settingsController?.SelectedInputDevice, comboBoxDeviceOut.SelectedItem as MMDevice);
            listBoxMusic.SelectedIndexChanged += (s, e) => settingsController?.UpdateFromUI(samprate.Text, fftSizeUpDown.Text, (int)mps.Value, (double)energyUpDown.Value, trackBarVolume.Value, comboBoxDeviceIn.SelectedItem, comboBoxDeviceOut.SelectedItem, latencyUpDown.Value, listBoxMusic.SelectedItem);
        }

        private void Karaoke_Shown(object sender, EventArgs e)
        {
            /* 見た目をよくするために、ダミーデータを用意 */
            chartInputWave.Series[0].Points.AddXY(0, 0);
            chartPitch.Series[0].Points.AddXY(0, 0);

            // x領域の最大値が小数、整数に依らず固定させる
            var area = chartPitch.ChartAreas[0];
            area.InnerPlotPosition.Auto = false;
            area.InnerPlotPosition.Width = 100; // 100%
            area.InnerPlotPosition.Height = 90; // 90%(横軸のメモリラベル印字分の余裕を設ける)
            area.InnerPlotPosition.X = 11;
            area.InnerPlotPosition.Y = 0;
        }


        /*
         チャート関係
         */
        private void SetupCharts()
        {
            // 波形チャート設定
            SetupChart(chartInputWave, -1, 1, samplingInterval, samplingInterval, "Waveform");

            // ピッチチャート設定
            SetupPitchChart(chartPitch);
        }

        private void SetupChart(Chart chart, double ymin, double ymax, double xmax, int interval, string seriesName)
        {
            chart.Series.Clear();
            var series = new Series(seriesName)
            {
                ChartType = SeriesChartType.FastLine
            };
            chart.Series.Add(series);

            // ChartArea設定
            var area = chart.ChartAreas[0];
            area.AxisY.Minimum = ymin;
            area.AxisY.Maximum = ymax;
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = xmax;
            area.AxisX.Interval = interval;

            // 凡例設定
            chart.Legends.Add(new Legend("Default"));
            chart.Legends[0].Docking = Docking.Top;
            chart.Legends[0].Alignment = StringAlignment.Far;
            chart.Legends[0].IsDockedInsideChartArea = true;

            // 余白調整（例: Chart全体内の描画位置調整）
            area.Position.Auto = false;
            area.Position.X = 10;
            area.Position.Y = 10;
            area.Position.Width = 100;
            area.Position.Height = 90;
        }

        private void SetupPitchChart(Chart chartPitch)
        {
            chartPitch.Series.Clear();

            // 歌声用の折れ線
            var seriesPitch = new Series("Singing")
            {
                ChartType = SeriesChartType.FastLine,
                Color = Color.Blue,
                BorderWidth = 2,
                YAxisType = AxisType.Primary,
                IsVisibleInLegend = false
            };
            chartPitch.Series.Add(seriesPitch);

            var area = chartPitch.ChartAreas[0];
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = 5;
            area.AxisY.Minimum = 48;  // C3くらい
            area.AxisY.Maximum = 72;  // C5くらい
            area.AxisY.Interval = 1;

            area.AxisX.LabelStyle.Format = "0.####";

            SetupYAxis(60); // C4を中心にラベルを張る

            // 再生位置を示す赤線
            playLine = new StripLine
            {
                Interval = 0,
                StripWidth = 0,     // 幅 0 で線になる
                BorderColor = Color.Red,
                BorderWidth = 2
            };
            area.AxisX.StripLines.Add(playLine);

            // ガイド描画を PrePaint で行う。歌声を上に表示したいので、ガイドが先
            // PostPaint を使用すると、ガイドが折れ線の上に表示される。
            chartPitch.PrePaint += chartPitch_PrePaint;
        }

        private void SetupYAxis(int centerNote, int maxNote = -1, int minNote = -1)
        {
            var axisY = chartPitch.ChartAreas[0].AxisY;
            axisY.CustomLabels.Clear();

            if (maxNote < 0) maxNote = centerNote + 12;
            if (minNote < 0) minNote = centerNote - 12;

            axisY.Minimum = minNote;
            axisY.Maximum = maxNote;

            string[] noteNames = { "C", "C#", "D", "D#", "E", "F",
                           "F#", "G", "G#", "A", "A#", "B" };

            for (int midi = minNote; midi <= maxNote; midi++)
            {
                string label = noteNames[midi % 12];
                axisY.CustomLabels.Add(midi - 0.5, midi + 0.5, label);
            }
        }


        private void Reset_chartInputWave(double xmax)
        {
            var area = chartInputWave.ChartAreas[0];
            area.AxisX.Maximum = xmax;
            area.AxisX.Interval = xmax;

            /* ダミーデータを描画。再描画時は Clear ではなく SetValueY で */
            var waveform = chartInputWave.Series[0];
            waveform.Points.Clear();
            for (int i = 0; i < samplingInterval; i++)
                waveform.Points.AddXY(i, 0);
        }


        /* 
         WaveIn, WaveOut の初期設定（buttonPlayで使用）
         */

        private void SetupAudioOut(string songName = null, int volume = 100)
        {
            // Use provided songName/volume, fallback to UI if null
            string effectiveSongName = songName ?? (listBoxMusic.SelectedItem as string);
            int effectiveVolume = volume;
            if (songName == null && listBoxMusic.SelectedIndex < 0)
            {
                LoadSong();
                return;
            }
            if (songName == null)
            {
                effectiveVolume = trackBarVolume.Value;
            }
            // Dispose previous music if any to ensure new selection takes effect immediately
            try { music?.Dispose(); } catch (Exception ex) { Debug.WriteLine($"Error disposing previous music: {ex}"); }
            music = null;

            try
            {
                music = new AudioFileReader(NoteUtils.changeFIleNameToPath(effectiveSongName));
                music.Volume = effectiveVolume / 100.0f;

                if (audioOut == null)
                {
                    MessageBox.Show("Output device is not initialized. Playback aborted.", "Device Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                audioOut.Init(music);
                LoadSong(effectiveSongName);
                audioOut.Play();
                timerPlay.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SetupAudioOut: {ex}");
                MessageBox.Show($"Failed to start playback: {ex.Message}", "Playback Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { music?.Dispose(); } catch { }
                music = null;
            }
        }






        /*
         デバイス、楽曲リストの設定
         */
        private void SetDeviceList()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            // データソースに直接バインド
            comboBoxDeviceIn.DataSource = devices.ToList();
            comboBoxDeviceIn.DisplayMember = "FriendlyName"; // 表示用
            comboBoxDeviceIn.ValueMember = null;
            comboBoxDeviceIn.SelectedIndex = 0;

            devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            comboBoxDeviceOut.DataSource = devices.ToList();
            comboBoxDeviceOut.DisplayMember = "FriendlyName"; // 表示用
            comboBoxDeviceOut.ValueMember = null;
            comboBoxDeviceOut.SelectedIndex = 0;

            // Clear any cached device references in settings controller after re-enumeration
            settingsController?.ClearDeviceSelection();
        }

        private void SetMusicList()
        {
            List<string> songNames = NoteUtils.getMusicList();
            listBoxMusic.Items.Clear();
            foreach (string name in songNames)
            {
                listBoxMusic.Items.Add(name);
            }
        }


        /*
         イベント
         */
        private void Karaoke_KeyDown(object sender, KeyEventArgs e)
        {
            /* KeyPreview を true にしてるので、こいつが先にキー処理できる */
            bool handled = false;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (OctaveUpDown.Value < OctaveUpDown.Maximum)
                    {
                        OctaveUpDown.Value += 1;
                        handled = true;
                    }
                    break;

                case Keys.Down:
                    if (OctaveUpDown.Value > OctaveUpDown.Minimum)
                    {
                        OctaveUpDown.Value -= 1;
                        handled = true;
                    }
                    break;

                case Keys.Left:
                    if (latencyUpDown.Value < latencyUpDown.Maximum)
                    {
                        latencyUpDown.Value += 0.01m;
                        handled = true;
                    }
                    break;

                case Keys.Right:
                    if (latencyUpDown.Value > latencyUpDown.Minimum)
                    {
                        latencyUpDown.Value -= 0.01m;
                        handled = true;
                    }
                    break;
            }

            if (handled)
            {
                e.Handled = true;          // .NET用
                e.SuppressKeyPress = true; // イベントをコントロールに渡さない設定
            }
        }

        private void trackBarVolume_ValueChanged(object sender, EventArgs e)
        {
            if (music != null)
                music.Volume = trackBarVolume.Value / 100.0f;
        }

        private void latencyUpDown_ValueChanged(object sender, EventArgs e)
        {
            latency = (float)latencyUpDown.Value;
        }

        private void chartPitch_PrePaint(object sender, ChartPaintEventArgs e)
        {
            if (noteOverlayBitmap != null)
            {
                var g = e.ChartGraphics.Graphics;
                g.DrawImage(noteOverlayBitmap, 0, 0);
            }
        }

        private void samprate_SelectedItemChanged(object sender, EventArgs e)
        {
            int rate = int.Parse(samprate.Text);
            int minThreshold = rate / (int)mps.Value;

            fftSizeUpDown.SelectedItem = null;
            fftSizeUpDown.Items.Clear();

            // 2^n を降順に列挙
            for (int n = 16; n >= 1; n--)
            {
                int size = 1 << n;

                if (size > rate) continue;  // サンプリングレートを超えるものはスキップ
                if (size < minThreshold) break;     // 閾値を下回ったら終了（以降も小さいだけ）

                fftSizeUpDown.Items.Add(size);      // リストに追加
            }

            // 追加されたものがあれば最大値を選択
            if (fftSizeUpDown.Items.Count > 0)
            {
                fftSizeUpDown.SelectedItem = fftSizeUpDown.Items[0];
            }
        }

        private void buttonDeviceUpdate_Click(object sender, EventArgs e)
        {
            SetDeviceList();
        }

        private void buttonMusicUpdate_Click(object sender, EventArgs e)
        {
            SetMusicList();
        }

        // Controller-invoked methods (extracted from original handlers)
        private readonly object playbackLock = new object();

        private void StartPlayback()
        {
            lock (playbackLock)
            {
                // If a previous playback wasn't fully stopped, ensure cleanup first (internal, without locking)
                if (analysisToken != null)
                {
                    try { StopPlaybackInternal(); } catch (Exception ex) { Debug.WriteLine($"Error stopping previous playback: {ex}"); }
                }

                buttonPlay.Enabled = false;
                buttonStop.Enabled = true;

                lyricsBox.Clear();
                lyricsBox.AppendText("（セットアップ中・・・）");
                lyricsBox.SelectionLength = 0;
                lyricsBox.Update();

                // Refresh settings snapshot from UI immediately to avoid race with recent UI changes
                settingsController?.UpdateFromUI(samprate.Text, fftSizeUpDown.Text, (int)mps.Value, (double)energyUpDown.Value,
                                                trackBarVolume.Value, comboBoxDeviceIn.SelectedItem, comboBoxDeviceOut.SelectedItem, latencyUpDown.Value, listBoxMusic.SelectedItem);

                // Prefer current UI selection (combo boxes / list) to avoid stale SettingsController objects from prior device lists.
                int effectiveSamplingRate = settingsController?.SamplingRate ?? (int.TryParse(samprate.Text, out var sr) ? sr : 16384);
                int effectiveFFTSize = settingsController?.FFTSize ?? (int.TryParse(fftSizeUpDown.Text, out var fs) ? fs : 2048);
                int effectiveMeasuresPerSec = settingsController?.MeasuresPerSec ?? (int)mps.Value;
                double effectiveMinEnergy = settingsController?.MinEnergy ?? (double)energyUpDown.Value;
                int effectiveVolume = settingsController?.Volume ?? trackBarVolume.Value;
                var effectiveInputDevice = (comboBoxDeviceIn.SelectedItem as MMDevice) ?? settingsController?.SelectedInputDevice;
                var effectiveOutputDevice = (comboBoxDeviceOut.SelectedItem as MMDevice) ?? settingsController?.SelectedOutputDevice;
                string effectiveSongName = (listBoxMusic.SelectedItem as string) ?? settingsController?.SelectedSongName;

                samplingRate = effectiveSamplingRate;
                fftSize = effectiveFFTSize;
                measuresPerSec = effectiveMeasuresPerSec;
                minEnergy = effectiveMinEnergy;

                detector = new PitchDetector(samplingRate, fftSize, measuresPerSec);
                samplingInterval = samplingRate / measuresPerSec;
                UpdatePhaseSec(CONST_PAGE_SEC);

                ringBuffer_waveIn = new double[samplingInterval];
                ringBuffer_waveInIndex = 0;

                ringBuffer_fft = new double[samplingInterval * 4];
                ringBuffer_fftIndex = 0;

                /* チャートの再初期化 */
                Reset_chartInputWave(samplingInterval);
                chartPitch.Series[0].Points.Clear();

                /* 入出力作成 */
                var enumerator = new MMDeviceEnumerator();
                try
                {
                    if (effectiveInputDevice == null)
                        effectiveInputDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to get default input device: {ex}");
                    effectiveInputDevice = null;
                }

                try
                {
                    if (effectiveOutputDevice == null)
                        effectiveOutputDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to get default output device: {ex}");
                    effectiveOutputDevice = null;
                }

                try
                {
                    if (effectiveInputDevice != null)
                    {
                        audioIn = new AudioInputService(effectiveInputDevice, samplingRate);
                        audioIn.DataAvailable += WaveIn_DataAvailable;
                        audioIn.Start();
                    }
                    else
                    {
                        Debug.WriteLine("No input device available, skipping audioIn initialization.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error initializing audioIn: {ex}");
                    MessageBox.Show("Failed to initialize input device. Check device availability.", "Device Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    StopPlayback();
                    return;
                }

                /* キャリブレーション */
                latency = (float)LatencyCalibrator.CalibrateLatency(samplingRate);
                latencyUpDown.Value = (decimal)latency;
                if (settingsController != null) settingsController.Latency = latency;

                try
                {
                    if (effectiveOutputDevice == null)
                    {
                        Debug.WriteLine("No output device available.");
                        MessageBox.Show("No output device available. Playback aborted.", "Device Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        StopPlayback();
                        return;
                    }

                    audioOut = new AudioOutputService(effectiveOutputDevice, latency);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error initializing audioOut: {ex}");
                    MessageBox.Show("Failed to initialize output device. Check device availability.", "Device Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    StopPlayback();
                    return;
                }

                lyricsBox.Clear();
                lyricsBox.Update();

                timerUI.Start();

                /* 本番用に設定 */
                // audioIn and audioOut are initialized above and ready to use
                SetupAudioOut(effectiveSongName, effectiveVolume);

                analysisToken = new CancellationTokenSource();
                analysisService = new AnalysisService(detector, samplingInterval, samplingRate, minEnergy);
                // Run analysis in a guarded task so exceptions are observed and cancellation is waited on during stop
                analysisTask = Task.Run(async () =>
                {
                    try
                    {
                        await analysisService.Run(analyzeQueue, audioQueue, drawQueue, pitchQueue, GetCurrentTime, () => (double)latency, () => useFourierFlag, () => useHPSFlag, analysisToken.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.WriteLine("AnalysisService cancelled");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"AnalysisService error: {ex}");
                        MessageBox.Show($"Analysis service error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }
        }

        private void StopPlaybackInternal()
        {
            try
            {
                buttonPlay.Enabled = true;
                buttonStop.Enabled = false;

                // First, stop incoming audio to avoid new work being enqueued
                try
                {
                    if (audioIn != null)
                    {
                        audioIn.DataAvailable -= WaveIn_DataAvailable;
                        try { audioIn.Stop(); } catch (Exception ex) { Debug.WriteLine($"Error stopping audioIn: {ex}"); }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error while disabling audioIn: {ex}");
                }

                // Pause output early so playback halts immediately
                try { audioOut?.Pause(); } catch (Exception ex) { Debug.WriteLine($"Error pausing audioOut: {ex}"); }

                // Cancel analysis and wait for task to finish (with timeout)
                try
                {
                    analysisToken?.Cancel();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error cancelling analysis token: {ex}");
                }

                if (analysisTask != null)
                {
                    try
                    {
                        if (!analysisTask.Wait(3000)) // wait up to 3s
                        {
                            Debug.WriteLine("Analysis task did not complete within timeout");
                        }
                    }
                    catch (AggregateException aex)
                    {
                        Debug.WriteLine($"Analysis task aggregate exception: {aex.Flatten()}");
                    }
                    finally
                    {
                        analysisTask = null;
                    }
                }

                analysisToken = null;

                // Clear buffers safely
                ringBuffer_waveIn = null;
                ringBuffer_waveInIndex = 0;
                ringBuffer_fft = null;
                ringBuffer_fftIndex = 0;
                while (audioQueue.TryDequeue(out _)) { }
                while (analyzeQueue.TryDequeue(out _)) { }

                // Dispose audio devices with guards
                try { audioIn?.Dispose(); } catch (Exception ex) { Debug.WriteLine($"Error disposing audioIn: {ex}"); }
                audioIn = null;

                try { audioOut?.Dispose(); } catch (Exception ex) { Debug.WriteLine($"Error disposing audioOut: {ex}"); }
                audioOut = null;

                try { music?.Dispose(); } catch (Exception ex) { Debug.WriteLine($"Error disposing music: {ex}"); }
                music = null;

                timerPlay.Stop();
                timerUI.Stop();
                stopwatch.Stop();

                try { detector?.DisposeFFTWindow(); } catch (Exception ex) { Debug.WriteLine($"Error disposing detector FFT window: {ex}"); }

                // ガイド削除
                try { noteOverlayBitmap?.Dispose(); } catch (Exception ex) { Debug.WriteLine($"Error disposing bitmap: {ex}"); }
                noteOverlayBitmap = null;
                pageCache.Clear();

                // 歌詞削除
                lyricsBox.Clear();

                /* チャート初期化 */
                Reset_chartInputWave(samplingInterval);
                chartPitch.Series[0].Points.Clear();

                chartPitch.Invalidate();

                // 楽曲設定削除
                currentPage = 0;
                currentSong = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error in StopPlayback: {ex}");
            }
        }

        private void StopPlayback()
        {
            lock (playbackLock)
            {
                StopPlaybackInternal();
            }
        }

        private void PageBackPlayback()
        {
            double playSec = currentSong.Pages[currentPage].StartSec;
            music.CurrentTime = TimeSpan.FromSeconds(playSec);

            trackBarMusic.Value = Math.Max(0, (int)(playSec - latency));

            chartPitch.Series["Singing"].Points.Clear();
            LoadPage(currentPage);
            UpdateGuideOverlay();
        }

        private void PageForwardPlayback()
        {
            double playSec = currentSong.Pages[currentPage].StartSec;
            music.CurrentTime = TimeSpan.FromSeconds(playSec);

            trackBarMusic.Value = Math.Max(0, (int)(playSec - latency));

            chartPitch.Series["Singing"].Points.Clear();
            LoadPage(currentPage);
            UpdateGuideOverlay();
        }

        private void buttonDeSelect_Click(object sender, EventArgs e)
        {
            listBoxMusic.SelectedIndex = -1;
        }

        private void buttonPageBack_Click(object sender, EventArgs e)
        {
            if ((currentSong != null) && (0 < currentPage))
            {
                currentPage--;
                playbackController?.PageBack();
            }
        }

        private void buttonPageForward_Click(object sender, EventArgs e)
        {
            if ((currentSong != null) && (currentPage + 1 < currentSong.Pages.Count))
            {
                currentPage++;
                playbackController?.PageForward();
            }
        }

        private CancellationTokenSource analysisToken;
        private Task analysisTask;

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            playbackController?.Play();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            playbackController?.Stop();
        }


        /*
         音声受け取り・描画
         */

        // マイク入力用バッファ（リングバッファ）
        private readonly ConcurrentQueue<double[]> analyzeQueue = new ConcurrentQueue<double[]>();
        private readonly ConcurrentQueue<double[]> audioQueue = new ConcurrentQueue<double[]>();

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            int samples = e.BytesRecorded / 2; // 16bit PCM → short

            short[] buffer = new short[samples];
            Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesRecorded);

            double[] processed = new double[samples];
            for (int i = 0; i < buffer.Length; i++)
                processed[i] = buffer[i] / (double)short.MaxValue;

            // 可変長バッファを samplingInterval長 で切り出し
            int waveIndex = 0;
            while (waveIndex < samples)
            {
                int length = Math.Min(samplingInterval - ringBuffer_waveInIndex, samples - waveIndex);

                Array.Copy(processed, waveIndex, ringBuffer_waveIn, ringBuffer_waveInIndex, length);

                ringBuffer_waveInIndex += length;
                waveIndex += length;

                // 1フレーム分たまったら解析用にコピー
                if (ringBuffer_waveInIndex >= samplingInterval)
                {
                    var frame = new double[samplingInterval];
                    Array.Copy(ringBuffer_waveIn, frame, samplingInterval);
                    analyzeQueue.Enqueue(frame);

                    ringBuffer_waveInIndex = 0;
                }
            }

            // 最新フレームを描画用に積む
            while (audioQueue.TryDequeue(out _)) { }
            var drawFrame = new double[samplingInterval];
            Array.Copy(ringBuffer_waveIn, drawFrame, samplingInterval);
            audioQueue.Enqueue(drawFrame);
        }


        /*
         ガイド表示
         */

        private int guideBaseNote = 60; // ページごとの中心音
        private Dictionary<int, Bitmap> pageCache = new Dictionary<int, Bitmap>();  // ガイドページのキャッシュ

        private void LoadSong(string songName = null)
        {
            if (songName == null)
            {
                currentSong = null;

                trackBarMusic.Minimum = 0;
                trackBarMusic.Maximum = 0;
                trackBarMusic.Value = 0;

                labelMusicLength.Text = "00:00";
            }
            else
            {
                currentSong = NoteUtils.getSongData(songName);

                trackBarMusic.Minimum = 0;
                trackBarMusic.Maximum = currentSong.length;
                trackBarMusic.TickFrequency = currentSong.length;
                trackBarMusic.SmallChange = 1;
                trackBarMusic.LargeChange = 10;
                trackBarMusic.Value = 0;

                int m = currentSong.length / 60;
                int s = currentSong.length % 60;
                labelMusicLength.Text = m.ToString("D2") + ":" + s.ToString("D2");
            }

            labelCurrentTime.Text = "00:00";

            // ガイドノート削除
            noteOverlayBitmap?.Dispose();
            noteOverlayBitmap = new Bitmap(chartPitch.Width, chartPitch.Height);

            currentPage = 0;
            pageCache.Clear();
            LoadPage(currentPage);
            UpdateGuideOverlay();

            stopwatch.Restart();
            stopwatch.Start();
        }

        private void LoadPage(int pageIndex)
        {
            currentPage = pageIndex;

            if (currentSong == null)
            {
                // 固定ページ
                pageStartSec = pageIndex * CONST_PAGE_SEC;
                pageEndSec = (pageIndex + 1) * CONST_PAGE_SEC;
                UpdatePhaseSec(CONST_PAGE_SEC);
                pitchIndex = 0;

                chartPitch.ChartAreas[0].AxisX.Maximum = 5;
                chartPitch.ChartAreas[0].AxisX.Interval = 5;

                // C4 を中心に固定
                guideBaseNote = 60;
                SetupYAxis(guideBaseNote);

                chartPitch.Invalidate();
                return;
            }

            // 楽曲演奏時
            var page = currentSong.Pages[pageIndex];
            pageStartSec = page.StartSec;
            pageEndSec = page.EndSec;
            double pageSecond = pageEndSec - pageStartSec;

            UpdatePhaseSec(pageSecond);
            pitchIndex = 0;

            chartPitch.ChartAreas[0].AxisX.Maximum = pageSecond;
            chartPitch.ChartAreas[0].AxisX.Interval = pageSecond;

            // 歌詞表示
            lyricsBox.Clear();

            if (page.lyricsData != null)
            {
                foreach (var span in page.lyricsData)
                {
                    lyricsBox.SelectionColor = ColorUtils.ParseColor(span.ForeColor);
                    lyricsBox.AppendText(span.Text);
                }
                lyricsBox.SelectionLength = 0;
            }
            else
            {
                // 旧データ
                lyricsBox.Text = page.lyrics;
                lyricsBox.ForeColor = Color.Black;
            }

            // ページ中心音 (最小+最大の平均)
            int diff = 0;
            int minNote = -1;
            int maxNote = -1;
            if (page.Notes.Count > 0)
            {
                minNote = page.Notes.Min(n => n.MidiNote);
                maxNote = page.Notes.Max(n => n.MidiNote);
                guideBaseNote = (minNote + maxNote) / 2;
                diff = maxNote - minNote;
            }
            else
            {
                guideBaseNote = 60;
            }

            if (diff > 22) SetupYAxis(guideBaseNote, maxNote + 1, minNote - 1);
            else SetupYAxis(guideBaseNote);

            // 再描画を強制
            chartPitch.Invalidate();
        }

        private void UpdatePhaseSec(double second)
        {
            arrayRange = (int)(measuresPerSec * second);
            pitchArray = new double[arrayRange];
        }

        private void UpdateGuideOverlay()
        {
            if (currentSong == null) return;
            if (currentPage < 0 || currentPage >= currentSong.Pages.Count) return;

            // すでにキャッシュがあればそれを使う
            if (pageCache.TryGetValue(currentPage, out var cached))
            {
                noteOverlayBitmap?.Dispose();
                noteOverlayBitmap = (Bitmap)cached.Clone();
                return;
            }

            // なければ新規生成
            var bmp = RenderPageToBitmap(currentPage);

            // キャッシュに保存
            pageCache[currentPage] = (Bitmap)bmp.Clone();

            noteOverlayBitmap?.Dispose();
            noteOverlayBitmap = bmp;
        }

        private Bitmap RenderPageToBitmap(int pageIndex)
        {
            var page = currentSong.Pages[pageIndex];
            var ca = chartPitch.ChartAreas[0];

            Bitmap bmp = new Bitmap(chartPitch.Width, chartPitch.Height);

            // (float)ca.AxisX.ValueToPixelPosition(); は重い。
            // 手計算で求める。必要な値の計算
            double pageSec = page.EndSec - page.StartSec;
            float chartWidth = chartPitch.Width;
            float chartHeight = chartPitch.Height;

            var pos = ca.Position;
            var inner = ca.InnerPlotPosition;

            // ChartArea（外枠）
            float areaX = chartWidth * pos.X / 100f;
            float areaY = chartHeight * pos.Y / 100f;
            float areaW = chartWidth * pos.Width / 100f;
            float areaH = chartHeight * pos.Height / 100f;

            // Plot領域（実際の描画領域）
            float plotX = areaX + areaW * inner.X / 100f;
            float plotY = areaY + areaH * inner.Y / 100f;
            float plotW = areaW * inner.Width / 100f;
            float plotH = areaH * inner.Height / 100f;

            // X軸
            double xScale = plotW / pageSec;

            // Y軸
            double yMin = ca.AxisY.Minimum;
            double yMax = ca.AxisY.Maximum;
            double yScale = plotH / (yMax - yMin);


            using (var g = Graphics.FromImage(bmp))
            {
                foreach (var note in page.Notes)
                {
                    double x1 = note.StartSec - page.StartSec;
                    double x2 = x1 + note.Duration;
                    double y = note.MidiNote;

                    float px1 = plotX + (float)(x1 * xScale);
                    float px2 = plotX + (float)(x2 * xScale);

                    float pyTop = plotY + (float)((yMax - (y + 0.5)) * yScale);
                    float pyBottom = plotY + (float)((yMax - (y - 0.5)) * yScale);

                    RectangleF rect = new RectangleF(px1, pyTop, px2 - px1, pyBottom - pyTop);

                    Color baseColor = ColorTranslator.FromHtml(note.Color);
                    using (var brush = new SolidBrush(Color.FromArgb(128, baseColor)))
                    using (var pen = new Pen(ControlPaint.Dark(baseColor), 1))
                    {
                        g.FillRectangle(brush, rect);
                        g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                    }
                }
            }

            return bmp;
        }


        /*
         UI描画
         */
        // 再生時間の表示とトラックバーの制御
        private void timerPlay_Tick(object sender, EventArgs e)
        {
            int ct = Math.Max(0, (int)(music.CurrentTime.TotalSeconds - latency));
            if (ct > trackBarMusic.Maximum)
            {
                timerPlay.Stop();
                return;
            }
            trackBarMusic.Value = ct;

            int m = ct / 60;
            int s = ct % 60;
            labelCurrentTime.Text = m.ToString("D2") + ":" + s.ToString("D2");
        }



        // グラフ描画関係
        ConcurrentQueue<double[]> drawQueue = new ConcurrentQueue<double[]>();
        ConcurrentQueue<PitchResult> pitchQueue = new ConcurrentQueue<PitchResult>();

        private double freq = 0.0;
        // latency auto-adjust samples (UI thread only)
        private readonly List<double> latencySamples = new List<double>();
        private const int LATENCY_SAMPLE_WINDOW = 30; // keep last N estimates
        private const double LATENCY_ALPHA = 0.25; // smoothing factor (not used for auto-apply by default)
        // Suggested latency computed from samples but not automatically applied during playback
        private double suggestedLatency = double.NaN;


        private void timerUI_Tick(object sender, EventArgs e)
        {
            // Update settings snapshot from UI (UI thread)
            settingsController.UpdateFromUI(samprate.Text, fftSizeUpDown.Text, (int)mps.Value, (double)energyUpDown.Value,
                                            trackBarVolume.Value, comboBoxDeviceIn.SelectedItem, comboBoxDeviceOut.SelectedItem, latencyUpDown.Value, listBoxMusic.SelectedItem);

            // Copy UI flags to atomic fields (UI thread)
            useFourierFlag = isFourier.Checked;
            useHPSFlag = isHPS.Checked;

            // 再生位置を基準に
            double currentSec = GetCurrentTime();

            HandlePageTransition(currentSec);

            // 音声波形の表示（最新1件だけ）
            if (drawQueue.TryDequeue(out var waveform))
            {
                UpdateWaveform(waveform);
            }

            // ピッチ折れ線描画
            while (pitchQueue.TryDequeue(out var p))
            {
                AddPitch(p);
                TryRecordLatencySample(p);
            }

            // 再生ラインの位置
            playLine.IntervalOffset = currentSec - pageStartSec;

            labelFreq.Text = freq.ToString();
        }


        private void HandlePageTransition(double currentSec)
        {
            if (music != null && currentSong != null)
            {
                // 曲再生時

                // ページ遷移判定
                // while で遅れた分を一気にページ遷移する
                while (currentPage < currentSong.Pages.Count &&
                       currentSec >= currentSong.Pages[currentPage].EndSec)
                {
                    currentPage++;

                    if (currentPage < currentSong.Pages.Count)
                    {
                        chartPitch.Series["Singing"].Points.Clear();

                        LoadPage(currentPage);
                        UpdateGuideOverlay();

                        pageStartSec = currentSong.Pages[currentPage].StartSec;
                        pageEndSec = currentSong.Pages[currentPage].EndSec;
                    }
                    else
                    {
                        // 曲の終了なのでWaveOutを終わらせる（Stopボタンの挙動を呼ぶ）
                        buttonStop.PerformClick();
                        return;
                    }
                }
            }
            else
            {
                // 曲無しの固定時間版
                int newPage = (int)(currentSec / CONST_PAGE_SEC);

                if (newPage == currentPage) return;

                currentPage = newPage;

                if (currentSong == null || currentPage < currentSong.Pages.Count)
                {
                    chartPitch.Series["Singing"].Points.Clear();

                    LoadPage(currentPage);
                    UpdateGuideOverlay();

                    pageStartSec = currentPage * CONST_PAGE_SEC;
                    pageEndSec = pageStartSec + CONST_PAGE_SEC;
                }
                else
                {
                    // 曲の終了なのでWaveOutを終わらせる（Stopボタンの挙動を呼ぶ）
                    buttonStop.PerformClick();
                }
            }
        }


        private void UpdateWaveform(double[] data)
        {
            var series = chartInputWave.Series[0];

            for (int i = 0; i < data.Length; i++)
                series.Points[i].SetValueY(data[i]);

            chartInputWave.Invalidate();
        }


        private void AddPitch(PitchResult p)
        {
            var series = chartPitch.Series["Singing"];

            double relTime = p.TimeSec - pageStartSec;

            double hosei_pitch = p.Pitch + ((int)OctaveUpDown.Value * 12);

            series.Points.AddXY(relTime, p.Pitch >= 0 ? hosei_pitch : double.NaN);
        }

        // Try to record a latency sample from a detected pitch by comparing raw detection time to nearest guide note time
        private void TryRecordLatencySample(PitchResult p)
        {
            try
            {
                if (p.Pitch < 0) return; // ignore invalid pitch
                if (currentSong == null) return;

                // Ensure page index is valid
                if (currentPage < 0 || currentPage >= currentSong.Pages.Count) return;
                var page = currentSong.Pages[currentPage];
                if (page == null || page.Notes == null || page.Notes.Count == 0) return;

                // Find nearest note by start time
                double bestDiff = double.MaxValue;
                dynamic bestNote = null;
                foreach (var note in page.Notes)
                {
                    double diff = Math.Abs(p.RawTimeSec - note.StartSec);
                    if (diff < bestDiff)
                    {
                        bestDiff = diff;
                        bestNote = note;
                    }
                }

                // Accept sample only if within reasonable time window
                if (bestNote == null || bestDiff > 1.0) return; // ignore if too far

                // Optionally check pitch closeness to note (in semitones)
                if (Math.Abs(p.Pitch - bestNote.MidiNote) > 3.0) return; // not close enough

                double sampleLatency = p.RawTimeSec - bestNote.StartSec; // positive if audio arrives later than guide

                // Record sample (UI thread) — NOT automatically applied during playback
                // to avoid playline jitter and to preserve performer rhythm visibility
                latencySamples.Add(sampleLatency);
                if (latencySamples.Count > LATENCY_SAMPLE_WINDOW)
                    latencySamples.RemoveAt(0);

                // Compute robust estimate: median (stored but not applied automatically)
                var sorted = latencySamples.OrderBy(x => x).ToList();
                suggestedLatency = sorted[sorted.Count / 2];

                // (Future: offer manual "Apply Calibration" button to apply suggestedLatency)
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error recording latency sample: {ex}");
            }
        }

        private double GetCurrentTime()
        {
            if (music != null)
                return music.CurrentTime.TotalSeconds - latency;

            return stopwatch.Elapsed.TotalSeconds;
        }
    }
}
