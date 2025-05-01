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

namespace Karaoke
{
    public partial class FrequencyPrinter : Form
    {
        const int SAMPLING_RATE = 16384;
        const int SAMPLING_INTERVAL = 512;
        const int ARRAY_RANGE = (int)(SAMPLING_RATE / SAMPLING_INTERVAL) * 5;
        const int WINDOW = SAMPLING_INTERVAL / 2;
        const int VALID_MAX = 1500;

        private WaveInEvent waveIn;
        private double[] hanningWindow = new double[SAMPLING_INTERVAL];
        private double[] pitchArray = new double[ARRAY_RANGE];
        private int pitchIndex = 0;

        private int MAX_POWER_SPEQ = (int)(20.0 * Math.Log10(SAMPLING_RATE) * 0.6);  // 理論値は20*log10(N)。実現値はせいぜい6割程度

        public FrequencyPrinter()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            InitializeComponent();
            SetDeviceList();
            SetupCharts();
            MakeTable();
        }


        private void SetupCharts()
        {
            // 波形チャート設定
            SetupChart(chartInputWave, -1, 1, SAMPLING_INTERVAL, SAMPLING_INTERVAL, "Waveform");

            // ピッチチャート設定
            SetupFreqChart(chartFreq);

            // 累積平均チャート設定
            SetupChart(chartCumAve, 0, 1, SAMPLING_INTERVAL/2, 50, "cumAve");
        }

        private void SetupChart(Chart chart, double ymin, double ymax, double xmax, int interval, string seriesName)
        {
            chart.Series.Clear();
            var series = new Series(seriesName)
            {
                ChartType = SeriesChartType.Line
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

        private void SetupFreqChart(Chart chartFreq)
        {
            chartFreq.SuppressExceptions = true;
            SetupChart(chartFreq, 0, VALID_MAX, ARRAY_RANGE, ARRAY_RANGE, "Frequency");
            var area = chartFreq.ChartAreas[0];

            area.AxisY2.Enabled = AxisEnabled.True;
            area.AxisY2.Minimum = 0;
            area.AxisY2.Maximum = VALID_MAX;

            //area.AxisY.IsLogarithmic = true;
            //area.AxisY.LogarithmBase = 2;

            //area.AxisY2.IsLogarithmic = true;
            //area.AxisY2.LogarithmBase = 2;

            // 音階ラベル設定
            string[] noteNames = {"A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#"};
            int i = 0;
            area.AxisY.CustomLabels.Clear();
            area.AxisY2.CustomLabels.Clear();
            while (true)
            {
                double value = 440 * Math.Pow(2, ((i + 1) - 49) / 12.0);
                if (value <= 0 || value > VALID_MAX) break;

                double offset = value * 0.05;
                CustomLabel val   = new CustomLabel(value - offset, value + offset, value.ToString("F2"), 0, LabelMarkStyle.None);
                CustomLabel label = new CustomLabel(value - offset, value + offset, noteNames[i % 12] + (i / 12).ToString(), 0, LabelMarkStyle.None);
                area.AxisY.CustomLabels.Add(val);
                area.AxisY2.CustomLabels.Add(label);
                i++;
            }
        }

        private void MakeTable()
        {
            for (int i = 0; i < SAMPLING_INTERVAL; i++)
            {
                hanningWindow[i] = 0.5 - 0.5 * Math.Cos(2.0 * Math.PI * i / (SAMPLING_INTERVAL - 1));
            }
        }

        private void buttonRec_Click(object sender, EventArgs e)
        {
            buttonRec.Enabled = false;
            buttonStop.Enabled = true;

            waveIn = new WaveInEvent
            {
                DeviceNumber = listBoxDevices.SelectedIndex,
                WaveFormat = new WaveFormat(SAMPLING_RATE, 16, 1),
                BufferMilliseconds = (int)((double)SAMPLING_INTERVAL / SAMPLING_RATE * 1000)
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


        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            short[] samples = new short[e.BytesRecorded / 2];
            Buffer.BlockCopy(e.Buffer, 0, samples, 0, e.BytesRecorded);

            double[] processed = new double[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                processed[i] = samples[i] / (double)short.MaxValue;
            }

            // 波形描画
            Invoke((Action)(() =>
            {
                var series = chartInputWave.Series[0];
                series.Points.Clear();
                for (int i = 0; i < processed.Length; i++)
                {
                    series.Points.AddXY(i, processed[i]);
                }
            }));

            // ピッチとcumAve検出
            (double freq, double[] auxData) = DetectPitchAndCumAve(processed);

            // pitchArray更新
            pitchArray[pitchIndex % ARRAY_RANGE] = freq;
            pitchIndex++;

            // ピッチチャート描画
            Invoke((Action)(() =>
            {
                var series = chartFreq.Series[0];
                series.Points.Clear();
                for (int i = 0; i < ARRAY_RANGE; i++)
                {
                    double val = pitchArray[(pitchIndex + i) % ARRAY_RANGE];
                    series.Points.AddXY(i, val >= 0 ? val : double.NaN);
                }

                labelFreq.Text = "Freq [Hz] : " + freq.ToString();
            }));

            // cumAveチャート描画
            Invoke((Action)(() =>
            {
                var series = chartCumAve.Series[0];

                // Fourier時のみX軸と凡例を変更
                if (isFourier.Checked)
                {
                    var area = chartCumAve.ChartAreas[0];
                    area.AxisX.Minimum = 0;
                    area.AxisX.Maximum = VALID_MAX;
                    area.AxisX.Interval = 200;
                    area.AxisY.Minimum = -10;
                    area.AxisY.Maximum = MAX_POWER_SPEQ;

                    series.Name = "Power (db)";
                }
                else
                {
                    var area = chartCumAve.ChartAreas[0];
                    area.AxisX.Minimum = 0;
                    area.AxisX.Maximum = SAMPLING_INTERVAL / 2;
                    area.AxisX.Interval = 50;
                    area.AxisY.Minimum = 0;
                    area.AxisY.Maximum = 1;

                    series.Name = "cumAve";
                }

                series.Points.Clear();

                // Fourier の場合、横軸は周波数スケール。周波数分解能 df を求める
                // magnitude は既に左半分だけにしてるので、周波数も半分に合わせる -> SAMPLING_RATE / 2.0
                double df = (SAMPLING_RATE / 2.0) / auxData.Length;
                int range = isFourier.Checked ? (int)(VALID_MAX / df) : auxData.Length;
                for (int i = 0; i < range; i++)
                {
                    // Fourier の場合、横軸は周波数スケール
                    double xval = isFourier.Checked ? i * df : i;
                    series.Points.AddXY(xval, auxData[i]);
                }
            }));
        }

        private (double pitch, double[] auxData) DetectPitchAndCumAve(double[] x)
        {
            double energy = 0.0;
            for (int i = 0; i < x.Length; i++)
                energy += x[i] * x[i];

            if (energy < 1e-4)
                return (-1, new double[SAMPLING_INTERVAL/2]);

            (double frequency, double[] auxData) result;
            if (isFourier.Checked)
                result = FourierTransform(x);
            else
                result = YIN(x);

            double freq = result.frequency;

            return (freq, result.auxData);
        }

        private (double pitch, double[] cumAve) YIN(double[] x)
        {
            double[] YIN = new double[WINDOW];
            double[] cumAve = new double[WINDOW];
            double sum = 0;
            int lamda = 0;

            for (int tau = 0; tau < WINDOW; tau++)
            {
                for (int j = 0; j < WINDOW - tau; j++)
                {
                    double diff = x[j] - x[j + tau];
                    YIN[tau] += diff * diff;
                }
            }

            cumAve[0] = 1;
            bool flag = false;
            for (int tau = 1; tau < WINDOW; tau++)
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

            double detectedFreq = SAMPLING_RATE / betterFreq;

            return (detectedFreq, cumAve);
        }

        private (double pitch, double[] spectrum) FourierTransform(double[] x)
        {
            // 必要なサイズだけコピー（不足分は0）
            Complex[] buffer = new Complex[SAMPLING_RATE];
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
            double dominantFreq = (double)maxIndex * SAMPLING_RATE / (double)buffer.Length;

            return (dominantFreq, magnitude);
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
    }
}