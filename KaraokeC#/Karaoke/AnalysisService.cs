using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Karaoke
{
    internal class AnalysisService
    {
        private readonly PitchDetector detector;
        private readonly int samplingInterval;
        private readonly int samplingRate;
        private readonly double minEnergy;

        private double[] ringBuffer_fft;
        private int ringBuffer_fftIndex;

        public AnalysisService(PitchDetector detector, int samplingInterval, int samplingRate, double minEnergy)
        {
            this.detector = detector;
            this.samplingInterval = samplingInterval;
            this.samplingRate = samplingRate;
            this.minEnergy = minEnergy;

            ringBuffer_fft = new double[samplingInterval * 4];
            ringBuffer_fftIndex = 0;
        }

        public async Task Run(ConcurrentQueue<double[]> analyzeQueue,
                             ConcurrentQueue<double[]> audioQueue,
                             ConcurrentQueue<double[]> drawQueue,
                             ConcurrentQueue<PitchResult> pitchQueue,
                             Func<double> getCurrentTime,
                             Func<double> getLatency,
                             Func<bool> getUseFourier,
                             Func<bool> getUseHPS,
                             CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                bool didWork = false;

                if (audioQueue.TryDequeue(out var audio))
                {
                    // 波形整形
                    double[] fixedWaveform = new double[samplingInterval];
                    int copyLen = Math.Min(audio.Length, samplingInterval);
                    Array.Copy(audio, fixedWaveform, copyLen);

                    drawQueue.Enqueue(fixedWaveform);
                    didWork = true;
                }

                if (analyzeQueue.TryDequeue(out var data))
                {
                    var pitch = DetectPitch(data, getUseFourier(), getUseHPS());

                    double rawTime = getCurrentTime() - (samplingInterval / (double)samplingRate / 2.0);
                    double latency = getLatency();
                    double timeSec = rawTime - latency;

                    pitchQueue.Enqueue(new PitchResult
                    {
                        TimeSec = timeSec,
                        RawTimeSec = rawTime,
                        Pitch = pitch
                    });
                    didWork = true;
                }

                if (!didWork)
                    await Task.Delay(5, token).ConfigureAwait(false);
            }
        }

        private double DetectPitch(double[] x, bool useFourier, bool useHPS)
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
            if (!useFourier)
                frequency = detector.YIN(x);
            else
            {
                if (useHPS)
                    frequency = detector.FourierTransformWithHPS(ringBuffer_fft, ringBuffer_fftIndex);
                else
                    frequency = detector.FourierTransform(ringBuffer_fft, ringBuffer_fftIndex);
            }

            // 無音判定。85Hz ~ 1000Hz を有効範囲とする。
            if (frequency < 85 || frequency > 1000)
                return -1;

            // 競合しないようにロック — AnalysisService は freq を直接更新しない。UI が必要とする場合は別途通知経路を実装してください.

            // ピッチへの変換
            double midiNote = 69 + 12 * Math.Log(frequency / 440.0, 2);
            return midiNote;
        }
    }
}
