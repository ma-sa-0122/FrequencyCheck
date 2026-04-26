using MathNet.Numerics.IntegralTransforms;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Karaoke.Programs.Pitch
{
    internal class HpsEstimator : IPitchEstimator
    {
        private int sampleRate;
        private int fftSize;
        private int ringBuffer_index = 0;
        private int harmonics = 4;

        private Complex[] fftWindow;

        public HpsEstimator(int sampleRate = 44100, int fftSize = 8192)
        {
            this.sampleRate = sampleRate;
            this.fftSize = fftSize;

            // ガベージコレクション（GC）対策に、プールで fftSize 長の領域を"借りる"
            fftWindow = ArrayPool<Complex>.Shared.Rent(fftSize);
        }

        public void DisposeFFTWindow()
        {
            // 領域を返す。Stop押下時に呼ぶ
            ArrayPool<Complex>.Shared.Return(fftWindow);
        }

        public void SetRingBuffer_index(int index)
        {
            this.ringBuffer_index = index;
        }
        public void SetHarmonics(int Harmonics)
        {
            this.harmonics = Harmonics;
        }

        public double Estimate(double[] x)
        {
            int length = Math.Min(x.Length, fftWindow.Length);
            for (int i = 0; i < length; i++)
            {
                fftWindow[i] = new Complex(x[(i + ringBuffer_index) % x.Length], 0.0);
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
            int minIndex = (int)(80.0 * fftSize / sampleRate);
            int maxIndex = (int)(1000.0 * fftSize / sampleRate);
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
            double fundamentalFreq = (double)peakIndex * sampleRate / fftSize;
            return fundamentalFreq;
        }
    }
}
