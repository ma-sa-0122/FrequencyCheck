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

        private WaveInEvent waveIn;
        private WaveOutEvent waveOut;
        private AudioFileReader music;

        private StripLine playLine; // 再生位置のガイド線

        private SongData currentSong;  // 読み込んだSongData

        private int currentPage = 0;
        private int lastDrawnPage = -1;  // 最後に描画したページ番号

        private Bitmap noteOverlayBitmap = null;

        // 現ページの秒数範囲
        private double pageStartSec = 0;
        private double pageEndSec = 0;

        private double[] pitchArray;   // ページ単位のピッチ配列
        private int pitchIndex = 0;

        private int arrayRange = 1024;  // 1ページあたりのピッチ描画点数

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
        private void SetupWaveIn(int deviceNumber)
        {
            waveIn = new WaveInEvent
            {
                DeviceNumber = deviceNumber,
                WaveFormat = new WaveFormat(samplingRate, 16, 1),
                BufferMilliseconds = 1000 / measuresPerSec
            };
            waveIn.DataAvailable += WaveIn_DataAvailable;

            waveIn.StartRecording();
        }

        private void SetupWaveOut()
        {
            if (listBoxMusic.SelectedIndex < 0)
                LoadSong();
            else
            {
                /* WaveOut で楽曲再生 */
                string songName = listBoxMusic.SelectedItem as string;

                waveOut = new WaveOutEvent() { DesiredLatency = 50 };
                music = new AudioFileReader(NoteUtils.changeFIleNameToPath(songName));
                waveOut.Init(music);

                LoadSong(songName);

                waveOut.Play();
                timerPlay.Start();
            }
        }

        /*
         デバイス、楽曲リストの設定
         */
        private void SetDeviceList()
        {
            listBoxDevices.Items.Clear();
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var capabilities = WaveIn.GetCapabilities(i);
                listBoxDevices.Items.Add(capabilities.ProductName);
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

            /* チャートの再初期化 */
            Reset_chartInputWave(samplingInterval);
            chartPitch.Series[0].Points.Clear();

            SetupWaveIn(listBoxDevices.SelectedIndex);

            SetupWaveOut();

            Task.Run(AnalysisLoop);
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonPlay.Enabled = true;
            buttonStop.Enabled = false;

            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
            }
            if (waveOut != null)
            {
                waveOut.Pause();
                waveOut.Dispose();
                waveOut = null;

                music.Dispose();
                music = null;

                timerPlay.Stop();
            }
        }


        /*
         音声受け取り・描画
         */

        // マイク入力用バッファ（リングバッファ）
        private readonly ConcurrentQueue<double[]> analyzeQueue = new ConcurrentQueue<double[]>();
        private readonly ConcurrentQueue<double[]> audioQueue = new ConcurrentQueue<double[]>();

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            short[] buffer = new short[e.BytesRecorded / 2];
            Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesRecorded);

            double[] processed = new double[e.BytesRecorded / 2];
            for (int i = 0; i < buffer.Length; i++)
                processed[i] = buffer[i] / (double)short.MaxValue;

            analyzeQueue.Enqueue(processed);

            // 最新の音声データだけ積む
            while (audioQueue.TryDequeue(out _)) { }    // 最新以外は破棄
            audioQueue.Enqueue(processed);
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

                    using (var brush = new SolidBrush(Color.FromArgb(128, Color.LightGreen)))
                    using (var pen = new Pen(Color.Green, 1))
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
            int ct = (int)music.CurrentTime.TotalSeconds;
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
            while (audioQueue.TryDequeue(out var waveform))
            {
                // 波形描画
                var seriesWave = chartInputWave.Series[0];
                for (int i = 0; i < waveform.Length; i++)
                    seriesWave.Points[i].SetValueY(waveform[i]);
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
                double relTime = (double)i / (samplingRate / samplingInterval); // ページ内時間
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
            double energy = 0.0;
            for (int i = 0; i < x.Length; i++)
                energy += x[i] * x[i];

            if (energy < minEnergy)
                return -1;

            double frequency = 0;
            if (isFourier.Checked && isHPS.Checked)
                frequency = detector.FourierTransformWithHPS(x);
            else if (isFourier.Checked)
                frequency = detector.FourierTransform(x);
            else
                frequency = detector.YIN(x);

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