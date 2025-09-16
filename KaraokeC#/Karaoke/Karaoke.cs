using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using NAudio.Dsp;
using NAudio.Wave;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;
using Series = System.Windows.Forms.DataVisualization.Charting.Series;
using Complex = System.Numerics.Complex;
using System.Collections.Concurrent;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using NAudio.CoreAudioApi;
using System.Threading;

namespace Karaoke
{
    public partial class Karaoke : Form
    {
        private PitchDetector detector;

        private int samplingRate = 16384;
        private int fftSize = 2048;
        private int measuresPerSec = 30;

        private int samplingInterval = 512;
        const int VALID_MAX_FREQ = 1000;
        private double minEnergy = 1e-4;

        private WasapiCapture wasapiCapture;
        private WasapiOut wasapiOut;
        private AudioFileReader music;

        private double latency = 0.05;

        // wasapiCapture はバッファサイズが不安定なので、固定長のリングバッファを用意
        double[] ringBuffer_waveIn;
        int ringBuffer_waveInIndex;

        private StripLine playLine; // 再生位置のガイド線

        private SongData currentSong;  // 読み込んだSongData
        private int currentPage = 0;
        private int lastDrawnPage = -1;  // 最後に描画したページ番号
        // 現ページの秒数範囲
        private double pageStartSec = 0;
        private double pageEndSec = 0;

        private Bitmap noteOverlayBitmap = null;

        private double[] pitchArray;   // ページ単位のピッチ配列
        private int pitchIndex = 0;

        private int arrayRange = 1024;  // 1ページあたりのピッチ描画点数

        // オーバーラップするfftSizeのリングバッファ
        private double[] ringBuffer_fft;
        private int ringBuffer_fftIndex;

        public Karaoke()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            InitializeComponent();
            SetDeviceList();
            SetMusicList();
            SetupCharts();

            timerUI.Start();
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

        private void SetupYAxis(int centerNote)
        {
            var axisY = chartPitch.ChartAreas[0].AxisY;
            axisY.CustomLabels.Clear();

            int minNote = centerNote - 12;
            int maxNote = centerNote + 12;

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
        private void CreateWasapiCapture(int selectedIndex)
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            if (selectedIndex < 0)
                selectedIndex = 0;

            var device = devices[selectedIndex];
            wasapiCapture = new WasapiCapture(device);
            wasapiCapture.WaveFormat = new WaveFormat(samplingRate, 16, 1);
        }
        private void SetupWasapiCapture()
        {
            wasapiCapture.DataAvailable += WaveIn_DataAvailable;
            wasapiCapture.StartRecording();
        }

        private void CreateWasapiOut()
        {
            wasapiOut = new WasapiOut(AudioClientShareMode.Shared, true, (int)(latency * 1000));
        }
        private void SetupWasapiOut()
        {
            if (listBoxMusic.SelectedIndex < 0)
                LoadSong();
            else
            {
                /* WaveOut で楽曲再生 */
                string songName = listBoxMusic.SelectedItem as string;

                music = new AudioFileReader(NoteUtils.changeFIleNameToPath(songName));
                wasapiOut.Init(music);

                LoadSong(songName);

                wasapiOut.Play();
                timerPlay.Start();
            }
        }

        /*
         遅延評価
         */
        private double CalibrateLatency()
        {
            var waveFormat = new WaveFormat(samplingRate, 1);

            using (var output = new WasapiOut(AudioClientShareMode.Shared, true, (int)(latency * 1000)))
            {
                wasapiCapture.WaveFormat = waveFormat;

                // 短いクリック音（サンプル長 = 1ms）
                float[] click = new float[waveFormat.SampleRate / 1000];
                click[0] = 1.0f;
                var buffer = new BufferedWaveProvider(waveFormat);
                buffer.AddSamples(click.SelectMany(v => BitConverter.GetBytes((short)(v * short.MaxValue))).ToArray(), 0, click.Length * 2);

                output.Init(buffer);

                var recorded = new List<byte>();
                AutoResetEvent done = new AutoResetEvent(false);

                wasapiCapture.DataAvailable += (s, e) =>
                {
                    recorded.AddRange(e.Buffer.Take(e.BytesRecorded));
                    if (recorded.Count >= waveFormat.SampleRate * 2) // 1秒分録音したら
                        done.Set();
                };

                wasapiCapture.StartRecording();
                output.Play();

                done.WaitOne(1000);
                wasapiCapture.StopRecording();

                // 録音データからピーク検出
                short[] pcm = new short[recorded.Count / 2];
                Buffer.BlockCopy(recorded.ToArray(), 0, pcm, 0, recorded.Count);

                int peakIndex = Array.IndexOf(pcm, pcm.Max());
                double delaySeconds = (double)peakIndex / waveFormat.SampleRate;

                return delaySeconds;
            }
        }



        /*
         デバイス、楽曲リストの設定
         */
        private void SetDeviceList()
        {
            listBoxDevices.Items.Clear();

            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            foreach (var dev in devices)
            {
                listBoxDevices.Items.Add(dev.FriendlyName);
            }
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

        private void isFourier_CheckedChanged(object sender, EventArgs e)
        {
            // HPSオプションの有効、無効化
            isHPS.Enabled = isFourier.Checked;
            if (!isFourier.Checked)
            {
                isHPS.Checked = false;
            }
        }

        private void buttonDeviceUpdate_Click(object sender, EventArgs e)
        {
            SetDeviceList();
            SetMusicList();
        }

        private void buttonDeSelect_Click(object sender, EventArgs e)
        {
            listBoxMusic.SelectedIndex = -1;
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            buttonPlay.Enabled = false;
            buttonStop.Enabled = true;

            /* 変数の設定 */
            samplingRate = int.Parse(samprate.Text);
            fftSize = int.Parse(fftSizeUpDown.Text);
            measuresPerSec = (int)mps.Value;
            minEnergy = (double)energyUpDown.Value;

            detector = new PitchDetector(samplingRate, fftSize, measuresPerSec);
            samplingInterval = samplingRate / measuresPerSec;
            UpdatePhaseSec(5.0);

            ringBuffer_waveIn = new double[samplingInterval];
            ringBuffer_waveInIndex = 0;

            ringBuffer_fft = new double[samplingInterval * 4];
            ringBuffer_fftIndex = 0;

            /* チャートの再初期化 */
            Reset_chartInputWave(samplingInterval);
            chartPitch.Series[0].Points.Clear();

            /* 入出力作成 */
            CreateWasapiCapture(listBoxDevices.SelectedIndex);
            CreateWasapiOut();

            /* キャリブレーション */
            labelLyrics.Text = "（セットアップ中・・・）";
            labelLyrics.Update();
            latency = CalibrateLatency();

            Thread.Sleep(500);
            labelLyrics.Text = "";
            labelLyrics.Update();

            /* 本番用に設定 */
            SetupWasapiCapture();
            SetupWasapiOut();

            Task.Run(AnalysisLoop);
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonPlay.Enabled = true;
            buttonStop.Enabled = false;

            if (wasapiCapture != null)
            {
                wasapiCapture.StopRecording();
                wasapiCapture.Dispose();
                wasapiCapture = null;
            }
            if (wasapiOut != null)
            {
                wasapiOut.Pause();
                wasapiOut.Dispose();
                wasapiOut = null;

                music?.Dispose();
                music = null;

                timerPlay.Stop();
            }

            detector.DisposeFFTWindow();
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
        private void LoadSong(string songName = null)
        {
            if (songName == null)
                currentSong = null;
            else
            {
                currentSong = NoteUtils.getSongData(songName);

                trackBarMusic.Minimum = 0;
                trackBarMusic.Maximum = currentSong.length;
                trackBarMusic.TickFrequency = currentSong.length;
                trackBarMusic.SmallChange = 1;
                trackBarMusic.LargeChange = 10;
                trackBarMusic.Value = 0;
            }

            lastDrawnPage = -1;
            currentPage = 0;
            LoadPage(currentPage);
            UpdateGuideOverlay();
        }

        private int guideBaseNote = 60; // ページごとの中心音

        private void LoadPage(int pageIndex)
        {
            currentPage = pageIndex;

            if (currentSong == null)
            {
                // 5秒固定ページ
                pageStartSec = pageIndex * 5.0;
                pageEndSec = (pageIndex + 1) * 5.0;
                UpdatePhaseSec(5.0);
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
            labelLyrics.Text = page.lyrics;

            // ページ中心音 (最小+最大の平均)
            if (page.Notes.Count > 0)
            {
                int minNote = page.Notes.Min(n => n.MidiNote);
                int maxNote = page.Notes.Max(n => n.MidiNote);
                guideBaseNote = (minNote + maxNote) / 2;
            }
            else
            {
                guideBaseNote = 60;
            }

            SetupYAxis(guideBaseNote);

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

            if (currentPage == lastDrawnPage) return;
            lastDrawnPage = currentPage;

            var page = currentSong.Pages[currentPage];
            var ca = chartPitch.ChartAreas[0];

            noteOverlayBitmap?.Dispose();
            noteOverlayBitmap = new Bitmap(chartPitch.Width, chartPitch.Height);

            using (var g = Graphics.FromImage(noteOverlayBitmap))
            {
                foreach (var note in page.Notes)
                {
                    double x1 = note.StartSec - page.StartSec;
                    double x2 = x1 + note.Duration;
                    double y = note.MidiNote;

                    float px1 = (float)ca.AxisX.ValueToPixelPosition(x1);
                    float px2 = (float)ca.AxisX.ValueToPixelPosition(x2);
                    float pyTop = (float)ca.AxisY.ValueToPixelPosition(y + 0.5);
                    float pyBottom = (float)ca.AxisY.ValueToPixelPosition(y - 0.5);

                    RectangleF rect = new RectangleF(px1, pyTop, px2 - px1, pyBottom - pyTop);

                    Color baseColor = ColorTranslator.FromHtml(note.Color);
                    Color fillColor = Color.FromArgb(128, baseColor);
                    Color borderColor = ControlPaint.Dark(baseColor);
                    using (var brush = new SolidBrush(fillColor))
                    using (var pen = new Pen(borderColor, 1))
                    {
                        g.FillRectangle(brush, rect);
                        g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                    }
                }
            }
        }


        /*
         UI描画
         */
        private void timerPlay_Tick(object sender, EventArgs e)
        {
            int ct = (int)(music.CurrentTime.TotalSeconds - latency);
            if (ct > trackBarMusic.Maximum)
            {
                timerPlay.Stop();
                return;
            }
            trackBarMusic.Value = ct;
        }

        private int lastPitchIndex = 0;

        private void timerUI_Tick(object sender, EventArgs e)
        {
            // マイク波形描画
            while (audioQueue.TryDequeue(out var waveform))
            {
                double[] fixedWaveform = new double[samplingInterval];
                // 必要に応じて切り捨て or ゼロ詰め
                int copyLen = Math.Min(waveform.Length, samplingInterval);
                Array.Copy(waveform, fixedWaveform, copyLen);

                var seriesWave = chartInputWave.Series[0];
                for (int i = 0; i < samplingInterval; i++)
                    seriesWave.Points[i].SetValueY(fixedWaveform[i]);
                chartInputWave.Invalidate();
            }

            // 最新ピッチが解析されたら描画
            if (pitchIndex == lastPitchIndex) return;
            lastPitchIndex = pitchIndex;


            // 再生位置を基準に
            double currentSec = 0;
            if (music != null)
                currentSec = music.CurrentTime.TotalSeconds;
            else
                currentSec = (5.0 * currentPage) +
                                ((double)pitchIndex / (samplingRate / samplingInterval));

            // ページ遷移判定
            if (currentSec >= pageEndSec)
                {
                    currentPage++;
                    if ((currentSong == null) || (currentPage < currentSong.Pages.Count))
                    {
                        chartPitch.Series["Singing"].Points.Clear();
                        LoadPage(currentPage);
                        UpdateGuideOverlay();
                    }
                    else
                    {
                        // 曲の終了なのでWaveOutを終わらせる（Stopボタンの挙動を呼ぶ）
                        buttonStop.PerformClick();
                        return;
                    }
                }

            // ピッチ折れ線描画
            var seriesPitch = chartPitch.Series["Singing"];
            // 差分だけ追加する
            for (int i = seriesPitch.Points.Count; i < pitchIndex; i++)
            {
                double val = pitchArray[i] + ((int)OctaveUpDown.Value * 12);
                double relTime = (double)i / (samplingRate / samplingInterval) - latency; // ページ内時間
                seriesPitch.Points.AddXY(relTime, val >= 0 ? val : double.NaN);
            }

            // 再生ラインの位置
            playLine.IntervalOffset = currentSec - pageStartSec;

            labelFreq.Text = freq.ToString();
        }

        private double freq = 0;


        /*
         ピッチ解析
         */
        // ピッチ解析用の別スレッド
        private async Task AnalysisLoop()
        {
            while (true)
            {
                if (analyzeQueue.TryDequeue(out var data))
                {
                    var pitch = DetectPitch(data);
                    if (pitchIndex >= pitchArray.Length)
                        pitchIndex = 0;

                    pitchArray[pitchIndex] = pitch;
                    pitchIndex++;
                }
                else
                {
                    await Task.Delay(1);
                }
            }
        }

        private double DetectPitch(double[] x)
        {
            // リングバッファでオーバーラップ
            int length = Math.Min(x.Length, ringBuffer_fft.Length - ringBuffer_fftIndex);
            Array.Copy(x, 0, ringBuffer_fft, ringBuffer_fftIndex, length);
            if (length < x.Length)
                Array.Copy(x, length, ringBuffer_fft, 0, x.Length - length);
            ringBuffer_fftIndex = (ringBuffer_fftIndex + x.Length) % ringBuffer_fft.Length;

            double energy = 0.0;
            for (int i = 0; i < x.Length; i++)
                energy += x[i] * x[i];

            if (energy < minEnergy)
                return -1;

            double frequency = 0;
            if (!isFourier.Checked)
                frequency = detector.YIN(x);
            else
            {
                if (isHPS.Checked)
                    frequency = detector.FourierTransformWithHPS(ringBuffer_fft, ringBuffer_fftIndex);
                    //frequency = detector.FourierTransformWithHPS(x, 0);
                else
                    frequency = detector.FourierTransform(ringBuffer_fft, ringBuffer_fftIndex);
                    //frequency = detector.FourierTransform(x, 0);
            }

            freq = frequency;

            // 無音判定。85Hz ~ 1000Hz を有効範囲とする。
            if (frequency < 85 || frequency > VALID_MAX_FREQ)
                return -1;

            // ピッチへの変換
            double midiNote = 69 + 12 * Math.Log(frequency / 440.0, 2);
            return midiNote;

            //double pitchClass = midiNote % 12;
            //return pitchClass;
        }
    }
}