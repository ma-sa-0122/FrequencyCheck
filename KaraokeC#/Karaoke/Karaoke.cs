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

namespace Karaoke
{
    public partial class Karaoke : Form
    {
        private int samplingRate = 16384;
        private int samplingInterval = 512;
        private int arrayRange;
        private int window;
        const int VALID_MAX_FREQ = 1000;

        private WaveInEvent waveIn;
        private WaveOutEvent waveOut;
        private AudioFileReader music;

        private double[] hanningWindow;
        private double[] pitchArray;
        private int pitchIndex = 0;

        private StripLine playLine; // 再生位置のガイド線


        public Karaoke()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            InitializeComponent();
            SetDeviceList();
            SetupCharts();

            Task.Run(AnalysisLoop);
            timerUI.Start();
        }

        private void PitchPrinter_Shown(object sender, EventArgs e)
        {
            // グラフにダミーデータを与えて描画。
            // 最初の描画にかかる初期化コストをここで受ける。
            chartInputWave.Series[0].Points.AddXY(1, 0);
            chartInputWave.Update();

            chartPitch.Series[0].Points.AddXY(1, 0);
            chartPitch.Update();
        }

        private void UpdateParameters()
        {
            window = samplingInterval / 2;
            hanningWindow = new double[samplingInterval];
            MakeHanningTable(); // ハニング窓再生成

            UpdatePhaseSec(5.0);
        }

        private void UpdatePhaseSec(double second)
        {
            arrayRange = (int)(samplingRate / samplingInterval * second);
            pitchArray = new double[arrayRange];
        }


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
            SetupChart(chartPitch, 0, 12, arrayRange, arrayRange, "Pitch");
            ChangePitchLabels(chartPitch);

            playLine = new StripLine();
            playLine.Interval = 0;
            playLine.StripWidth = 0;              // 幅0で「線」
            playLine.BorderColor = Color.Red;
            playLine.BorderWidth = 2;
            chartPitch.ChartAreas[0].AxisX.StripLines.Add(playLine);
        }


        private void ChangePitchLabels(Chart chartPitch)
        {
            var area = chartPitch.ChartAreas[0];
            area.AxisY.Interval = 1;

            // 音階ラベル設定
            string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            area.AxisY.CustomLabels.Clear();
            for (int i = 0; i < 12; i++)
            {
                int noteIndex = i % 12;
                CustomLabel label = new CustomLabel(i - 0.5, i + 0.5, noteNames[noteIndex], 0, LabelMarkStyle.None);
                area.AxisY.CustomLabels.Add(label);
            }
        }

        private void MakeHanningTable()
        {
            for (int i = 0; i < samplingInterval; i++)
            {
                hanningWindow[i] = 0.5 - 0.5 * Math.Cos(2.0 * Math.PI * i / (samplingInterval - 1));
            }
        }


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

        }


        /*
         イベント
         */

        private void buttonDeviceUpdate_Click(object sender, EventArgs e)
        {
            SetDeviceList();
            SetMusicList();
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            buttonPlay.Enabled = false;
            buttonStop.Enabled = true;

            /* 変数の設定 */
            samplingRate = int.Parse(samprate.Text);
            samplingInterval = samplingRate / (int)mps.Value;
            UpdateParameters();

            /* ピッチチャートの幅を調整 */
            var area = chartPitch.ChartAreas[0];
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = arrayRange;
            area.AxisX.Interval = arrayRange;

            /* WaveIn でマイク入力 */
            waveIn = new WaveInEvent
            {
                DeviceNumber = listBoxDevices.SelectedIndex,
                WaveFormat = new WaveFormat(samplingRate, 16, 1),
                BufferMilliseconds = (int)((double)samplingInterval / samplingRate * 1000)
            };
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.StartRecording();

            /* WaveOut で音楽再生 */
            //waveOut = new WaveOutEvent();
            //music = new AudioFileReader(listBoxMusic.Text);
            //waveOut.Init(music);
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
            //if (waveOut != null)
            //{
            //    waveOut.Pause();
            //    waveOut.Dispose();
            //    waveOut = null;
            //}
        }


        /*
         音声受け取り・描画
         */

        // マイク入力用バッファ（リングバッファ）
        private readonly ConcurrentQueue<double[]> audioQueue = new ConcurrentQueue<double[]>();
        private readonly ConcurrentQueue<double[]> analysisQueue = new ConcurrentQueue<double[]>();

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            short[] buffer = new short[e.BytesRecorded / 2];
            Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesRecorded);

            double[] processed = new double[e.BytesRecorded / 2];
            for (int i = 0; i < buffer.Length; i++)
                processed[i] = buffer[i] / (double)short.MaxValue;

            audioQueue.Enqueue(processed); // 音声データだけ積む（UI更新しない）
        }

        // 別スレッドで解析
        private async Task AnalysisLoop()
        {
            while (true)
            {
                if (audioQueue.TryDequeue(out var data))
                {
                    var pitch = DetectPitch(data);
                    if (pitchIndex >= pitchArray.Length) {
                        pitchIndex = 0;
                        Array.Clear(pitchArray, 0, pitchArray.Length);
                    }
                    pitchArray[pitchIndex] = pitch;
                    pitchIndex++;
                    analysisQueue.Enqueue(data);
                }
                else {
                    await Task.Delay(1); // CPU過負荷防止
                }
            }
        }

        // UIスレッド側はタイマーで一定時間ごとに描画
        private void timerUI_Tick(object sender, EventArgs e)
        {
            while (analysisQueue.TryDequeue(out var waveform))
            {
                // 波形描画
                var seriesWave = chartInputWave.Series[0];
                seriesWave.Points.Clear();
                for (int i = 0; i < waveform.Length; i++)
                    seriesWave.Points.AddXY(i, waveform[i]);

                // ピッチ描画
                var seriesPitch = chartPitch.Series[0];
                seriesPitch.Points.Clear();
                for (int i = 0; i < arrayRange; i++)
                {
                    double val = pitchArray[i];
                    seriesPitch.Points.AddXY(i, val >= 0 ? val : double.NaN);
                }
                playLine.IntervalOffset = (pitchIndex);
            }
        }


        /*
         ピッチ解析
         */

        private double DetectPitch(double[] x)
        {
            double energy = 0.0;
            for (int i = 0; i < x.Length; i++)
                energy += x[i] * x[i];

            if (energy < 1e-4)
                return -1;

            double frequency = 0;
            if (isFourier.Checked)
                frequency = FourierTransform(x);
            else
                frequency = YIN(x);

            // 無音判定。85Hz ~ 1000Hz を有効範囲とする。
            if (frequency < 85 || frequency > VALID_MAX_FREQ)
                return -1;

            // ピッチへの変換
            double midiNote = 69 + 12 * Math.Log(frequency / 440.0, 2);
            double pitchClass = midiNote % 12;

            return pitchClass;
        }

        private double YIN(double[] x)
        {
            double[] YIN = new double[window];
            double[] cumAve = new double[window];
            double sum = 0;
            int lamda = 0;

            for (int tau = 0; tau < window; tau++)
            {
                for (int j = 0; j < window - tau; j++)
                {
                    double diff = x[j] - x[j + tau];
                    YIN[tau] += diff * diff;
                }
            }

            cumAve[0] = 1;
            bool flag = false;
            for (int tau = 1; tau < window; tau++)
            {
                sum += YIN[tau];
                cumAve[tau] = YIN[tau] / (sum / tau);

                if (!flag) 
                {
                    if (cumAve[tau - 1] < 0.15 && cumAve[tau - 1] < cumAve[tau])
                    {
                        lamda = tau - 1;
                        flag = true;
                    }
                }
            }

            if (lamda == 0)
                return -1;

            double betterFreq = lamda + (cumAve[lamda - 1] - cumAve[lamda + 1]) /
                                (2.0 * (cumAve[lamda - 1] - 2.0 * cumAve[lamda] + cumAve[lamda + 1]));

            double detectedFreq = samplingRate / betterFreq;

            return detectedFreq;
        }

        private double FourierTransform(double[] x)
        {
            // 必要なサイズだけコピー（不足分は0）
            Complex[] buffer = new Complex[samplingRate];
            int length = Math.Min(x.Length, buffer.Length);
            for (int i = 0; i < length; i++)
            {
                buffer[i] = new Complex(x[i], 0.0);
            }

            // FFT 実行（破壊的に変換）
            Fourier.Forward(buffer, FourierOptions.Matlab); // Matlab形式は実数信号に適している

            // 最もスペクトルの強い index を調べる
            double maxPower = 0.0;
            int maxIndex = 0;
            for (int i = 1; i < (buffer.Length / 2); i++)
            {
                double power = buffer[i].Magnitude * buffer[i].Magnitude;
                if (power > maxPower)
                {
                    maxPower = power;
                    maxIndex = i;
                }
            }

            // 周波数 = index * Fs / N
            // 分解能[Hz]は 周波数 / データ点数。1000Hzの入力を500点で計算したら2Hz区切りでしか取れない感じ
            double dominantFreq = (double)maxIndex * samplingRate / (double)buffer.Length;

            return dominantFreq;
        }
    }
}