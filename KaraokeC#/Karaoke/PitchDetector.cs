using MathNet.Numerics.IntegralTransforms;
using System;
using System.Buffers;
using System.Numerics;

namespace Karaoke
{
    internal class PitchDetector
    {
        private int samplingRate;
        private int fftSize;
        private int samplingInterval;

        private int window;
        private double[] hanningWindow;

        private Complex[] fftWindow;

        public PitchDetector(int samplingRate, int fftSize, int measurePerSecond = 30)
        {
            this.samplingRate = samplingRate;
            this.fftSize = fftSize;

            // measure/sec からサンプリング間隔を計算
            samplingInterval = samplingRate / measurePerSecond;
            window = samplingInterval / 2;
            hanningWindow = new double[samplingInterval];
            MakeHanningTable();

            // ガベージコレクション（GC）対策に、プールで fftSize 長の領域を"借りる"
            fftWindow = ArrayPool<Complex>.Shared.Rent(fftSize);
        }

        private void MakeHanningTable()
        {
            for (int i = 0; i < samplingInterval; i++)
            {
                hanningWindow[i] = 0.5 - 0.5 * Math.Cos(2.0 * Math.PI * i / (samplingInterval - 1));
            }
        }

        public void DisposeFFTWindow()
        {
            // 領域を返す。Stop押下時に呼ぶ
            ArrayPool<Complex>.Shared.Return(fftWindow);
        }


        public double YIN(double[] x)
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

        public double FourierTransform(double[] x, int index)
        {
            // 必要なサイズだけコピー（不足分は0）
            int length = Math.Min(x.Length, fftWindow.Length);
            for (int i = 0; i < length; i++)
            {
                fftWindow[i] = new Complex(x[(i + index) % x.Length], 0.0);
            }
            // 余りをゼロ埋め
            Array.Clear(fftWindow, length, fftSize - length);

            // FFT 実行（破壊的に変換）
            Fourier.Forward(fftWindow, FourierOptions.Matlab); // Matlab形式は実数信号に適している

            // 最もスペクトルの強い index を調べる
            double maxPower = 0.0;
            int maxIndex = 0;
            for (int i = 1; i < (length / 2); i++)
            {
                double power = fftWindow[i].Magnitude * fftWindow[i].Magnitude;
                if (power > maxPower)
                {
                    maxPower = power;
                    maxIndex = i;
                }
            }

            // 周波数 = index * Fs / N
            // 分解能[Hz]は 周波数 / データ点数。1000Hzの入力を500点で計算したら2Hz区切りでしか取れない感じ
            double dominantFreq = (double)maxIndex * samplingRate / fftSize;

            return dominantFreq;
        }



        // HPSによる基音推定
        public double FourierTransformWithHPS(double[] x, int index, int harmonics = 4)
        {
            int length = Math.Min(x.Length, fftWindow.Length);
            for (int i = 0; i < length; i++)
            {
                fftWindow[i] = new Complex(x[(i + index) % x.Length], 0.0);
            }
            // 余りをゼロ埋め
            Array.Clear(fftWindow, length, fftSize - length);

            // FFT 実行（破壊的に変換）
            Fourier.Forward(fftWindow, FourierOptions.Matlab); // Matlab形式は実数信号に適している

            // 振幅スペクトル
            int nyquist = fftSize / 2;
            double[] power = new double[nyquist];
            for (int i = 0; i < nyquist; i++)
            {
                double mag = fftWindow[i].Magnitude;
                power[i] = mag * mag;
            }

            // HPS を log-domain で作る（和を取る）
            // hpsLog[i] = sum_{h=1..harmonics} log( power[i*h] + eps )
            double[] hpsLog = new double[nyquist];
            for (int i = 0; i < nyquist; i++) hpsLog[i] = 0.0;

            for (int h = 1; h <= harmonics; h++)
            {
                int limit = nyquist / h;
                for (int i = 0; i < limit; i++)
                {
                    hpsLog[i] += (1.0 / h) * Math.Log(power[i * h] + 1e-10);    // ゼロ対策
                }
                // beyond limit: can't use (out of range)
            }

            // ピーク検出（最低50Hz以上を対象にする）
            int minIndex = (int)(80.0 * fftSize / samplingRate);
            int maxIndex = (int)(1000.0 * fftSize / samplingRate);
            if (maxIndex >= hpsLog.Length) maxIndex = hpsLog.Length - 1;

            int peakIndex = minIndex;
            double peakValue = 0;

            for (int i = minIndex; i < maxIndex; i++)
            {
                if (hpsLog[i] > hpsLog[i - 1] && hpsLog[i] > hpsLog[i + 1]  // ピーク
                     && hpsLog[i] > peakValue)
                {
                    peakValue = hpsLog[i];
                    peakIndex = i;
                }
            }

            // 周波数に変換
            double fundamentalFreq = (double)peakIndex * samplingRate / fftSize;
            return fundamentalFreq;
        }
    }
}
