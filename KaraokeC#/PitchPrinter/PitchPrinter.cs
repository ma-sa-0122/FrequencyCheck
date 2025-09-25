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
    public partial class PichPrinter : Form
    {
        private int samplingRate = 16384;
        private int samplingInterval = 512;
        private int arrayRange;
        private int window;
        const int VALID_MAX_FREQ = 1000;

        private WaveInEvent waveIn;
        private double[] hanningWindow;
        private double[] pitchArray;
        private int pitchIndex = 0;
        private int pitchOffset = 0;

        private int maxPowerSpeq = 0;
        private double minEnergy = 1e-4;

        private readonly Dictionary<string, int> noteOffsetMap = new Dictionary<string, int> {
            { "C# / A#m (#7)", 1 },
            { "F# / D#m (#6)", 6 },
            { "B  / G#m (#5)", 11 },
            { "E  / C#m (#4)", 4 },
            { "A  / F#m (#3)", 9 },
            { "D  / Bm  (#2)", 2 },
            { "G  / Em  (#1)", 7 },
            { "C  / Am", 0 },
            { "F  / Dm  (b1)", 5 },
            { "Bb / Gm  (b2)", 10 },
            { "Eb / Cm  (b3)", 3 },
            { "Ab / Fm  (b4)", 8 },
            { "Db / Bbm (b5)", 1 },
            { "Gb / Ebm (b6)", 6 },
            { "Cb / Abm (b7)", 11 },
        };

        public PichPrinter()
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

            chartCumAve.Series[0].Points.AddXY(1, 0);
            chartCumAve.Update();

            chartPitch.Series[0].Points.AddXY(1, 0);
            chartPitch.Update();
        }

        private void UpdateParameters()
        {
            arrayRange = (int)(samplingRate / samplingInterval) * 5;
            window = samplingInterval / 2;

            hanningWindow = new double[samplingInterval];
            pitchArray = new double[arrayRange];
            MakeHanningTable(); // ハニング窓再生成

            maxPowerSpeq = (int)(20.0 * Math.Log10(samplingRate) * 0.6); // 理論値は20*log10(N)。実現値はせいぜい6割程度
            minEnergy = (double)energyUpDown.Value;
        }


        private void SetupCharts()
        {
            // 波形チャート設定
            SetupChart(chartInputWave, -1, 1, samplingInterval, samplingInterval, "Waveform");

            // ピッチチャート設定
            SetupPitchChart(chartPitch);

            // 累積平均チャート設定
            SetupChart(chartCumAve, 0, 1, samplingInterval / 2, 50, "cumAve");
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
                int noteIndex = (i + pitchOffset) % 12;
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


        /*
         イベント
         */

        private void scale_SelectedItemChanged(object sender, EventArgs e)
        {
            string selected = scale.Text;
            if (noteOffsetMap.TryGetValue(selected, out int offset))
            {
                pitchOffset = offset;
                ChangePitchLabels(chartPitch);
            }
        }

        private void buttonDeviceUpdate_Click(object sender, EventArgs e)
        {
            SetDeviceList();
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

        private void buttonRec_Click(object sender, EventArgs e)
        {
            buttonRec.Enabled = false;
            buttonStop.Enabled = true;

            samplingRate = int.Parse(samprate.Text);
            samplingInterval = samplingRate / (int)mps.Value;
            UpdateParameters();

            var area = chartPitch.ChartAreas[0];
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = arrayRange;
            area.AxisX.Interval = arrayRange;

            waveIn = new WaveInEvent
            {
                DeviceNumber = listBoxDevices.SelectedIndex,
                WaveFormat = new WaveFormat(samplingRate, 16, 1),
                BufferMilliseconds = (int)((double)samplingInterval / samplingRate * 1000)
            };
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.StartRecording();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonRec.Enabled = true;
            buttonStop.Enabled = false;

            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
            }
        }


        /*
         音声受け取り・描画
         */

        // マイク入力用バッファ（リングバッファ）
        private readonly ConcurrentQueue<double[]> audioQueue = new ConcurrentQueue<double[]>();
        private readonly ConcurrentQueue<(double[],double[])> analysisQueue = new ConcurrentQueue<(double[], double[])>();

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
                    var (pitch, cuvAve) = DetectPitchAndCumAve(data);
                    if (pitch >= 0) { 
                        pitch = (pitch - pitchOffset + 12) % 12;
                    }
                    pitchArray[pitchIndex % arrayRange] = pitch;
                    pitchIndex++;
                    analysisQueue.Enqueue((data, cuvAve));
                }
                else {
                    await Task.Delay(1); // CPU過負荷防止
                }
            }
        }

        // UIスレッド側はタイマーで一定時間ごとに描画
        private void timerUI_Tick(object sender, EventArgs e)
        {
            while (analysisQueue.TryDequeue(out var result))
            {
                var (waveform, auxData) = result;

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
                    double val = pitchArray[(pitchIndex + i) % arrayRange];
                    seriesPitch.Points.AddXY(i, val >= 0 ? val : double.NaN);
                }

                // cumAveチャート描画
                var series = chartCumAve.Series[0];
                // Fourier時のみ軸と凡例を変更
                if (isFourier.Checked)
                {
                    var area = chartCumAve.ChartAreas[0];
                    area.AxisX.Minimum = 0;
                    area.AxisX.Maximum = VALID_MAX_FREQ;
                    area.AxisX.Interval = 200;
                    area.AxisY.Minimum = -10;
                    area.AxisY.Maximum = maxPowerSpeq;

                    series.Name = "Power (db)";
                }
                else
                {
                    var area = chartCumAve.ChartAreas[0];
                    area.AxisX.Minimum = 0;
                    area.AxisX.Maximum = samplingInterval / 2;
                    area.AxisX.Interval = 50;
                    area.AxisY.Minimum = 0;
                    area.AxisY.Maximum = 1;

                    series.Name = "cumAve";
                }

                series.Points.Clear();

                // Fourier の場合、横軸は周波数スケール。周波数分解能 df を求める
                // magnitude は既に左半分だけにしてるので、周波数も半分に合わせる -> SAMPLING_RATE / 2.0
                double df = (samplingRate / 2.0) / auxData.Length;
                int range = isFourier.Checked ? (int)(VALID_MAX_FREQ / df) : auxData.Length;
                for (int i = 0; i < range; i++)
                {
                    // Fourier の場合、横軸は周波数スケール
                    double xval = isFourier.Checked ? i * df : i;
                    series.Points.AddXY(xval, auxData[i]);
                }
            }
        }


        /*
         ピッチ解析
         */

        private (double pitch, double[] auxData) DetectPitchAndCumAve(double[] x)
        {
            double energy = 0.0;
            for (int i = 0; i < x.Length; i++)
                energy += x[i] * x[i];

            if (energy < minEnergy)
                return (-1, new double[samplingRate / 2]);

            (double frequency, double[] auxData) result;
            if (isFourier.Checked)
                result = FourierTransform(x);
            else
                result = YIN(x);

            double freq = result.frequency;

            // 無音判定。85Hz ~ 1000Hz を有効範囲とする。
            if (freq < 85 || freq > VALID_MAX_FREQ)
                return (-1, result.auxData);

            // ピッチへの変換
            double midiNote = 69 + 12 * Math.Log(freq / 440.0, 2);
            double pitchClass = midiNote % 12;

            return (pitchClass, result.auxData);
        }

        private (double pitch, double[] cumAve) YIN(double[] x)
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
                return (-1, cumAve);

            double betterFreq = lamda + (cumAve[lamda - 1] - cumAve[lamda + 1]) /
                                (2.0 * (cumAve[lamda - 1] - 2.0 * cumAve[lamda] + cumAve[lamda + 1]));

            double detectedFreq = samplingRate / betterFreq;

            return (detectedFreq, cumAve);
        }

        private (double pitch, double[] spectrum) FourierTransform(double[] x)
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

            // スペクトルのパワーを計算
            // 中央で対称になるので、左半分だけあれば十分 -> SAMPLING_RATE / 2
            double[] magnitude = new double[buffer.Length / 2];
            double maxPower = 0.0;
            int maxIndex = 0;
            for (int i = 1; i < magnitude.Length; i++)
            {
                double power = buffer[i].Magnitude * buffer[i].Magnitude;
                if (power > maxPower)
                {
                    maxPower = power;
                    maxIndex = i;
                }
                magnitude[i] = 10.0 * Math.Log10(power + 1e-12); // ゼロ除算防止のため微小値を加える
            }

            // 周波数 = index * Fs / N
            // 分解能[Hz]は 周波数 / データ点数。1000Hzの入力を500点で計算したら2Hz区切りでしか取れない感じ
            double dominantFreq = (double)maxIndex * samplingRate / (double)buffer.Length;

            return (dominantFreq, magnitude);
        }
    }
}