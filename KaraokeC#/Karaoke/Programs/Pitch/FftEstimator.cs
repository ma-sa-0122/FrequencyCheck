using MathNet.Numerics.IntegralTransforms;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Karaoke.Programs.Pitch
{
    internal class FftEstimator : IPitchEstimator
    {
        private int sampleRate;
        private int fftSize;
        private int ringBuffer_index = 0;

        private Complex[] fftWindow;

        public FftEstimator(int sampleRate = 44100, int fftSize = 8192)
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

        public double Estimate(double[] x)
        {
            // 必要なサイズだけコピー（不足分は0）
            int length = Math.Min(x.Length, fftWindow.Length);
            for (int i = 0; i < length; i++)
            {
                // リングバッファなので、開始位置で補正をかける
                fftWindow[i] = new Complex(x[(i + ringBuffer_index) % x.Length], 0.0);
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
            double dominantFreq = (double)maxIndex * sampleRate / fftSize;

            return dominantFreq;
        }
    }
}
