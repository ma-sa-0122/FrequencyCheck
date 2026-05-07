using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Karaoke
{
    internal static class LatencyCalibrator
    {
        public static double CalibrateLatency(int samplingRate)
        {
            var waveFormat = new WaveFormat(samplingRate, 1);
            int len = waveFormat.SampleRate / 100;
            float[] noise = new float[len];
            Random rnd = new Random();
            for (int i = 0; i < len; i++) noise[i] = (float)(rnd.NextDouble() * 2 - 1);

            byte[] noiseBytes = noise.SelectMany(v => BitConverter.GetBytes((short)(v * short.MaxValue))).ToArray();

            var buffer = new BufferedWaveProvider(waveFormat);
            buffer.AddSamples(noiseBytes, 0, noiseBytes.Length);
            var output = new WasapiOut(AudioClientShareMode.Shared, true, 50);
            output.Init(buffer);

            var recorded = new List<byte>();
            AutoResetEvent done = new AutoResetEvent(false);

            var enumerator = new MMDeviceEnumerator();
            var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
            using (var capture = new WasapiCapture(device))
            {
                capture.DataAvailable += (s, e) =>
                {
                    recorded.AddRange(e.Buffer.Take(e.BytesRecorded));
                    if (recorded.Count >= waveFormat.SampleRate * 2)
                        done.Set();
                };

                capture.StartRecording();
                output.Play();
                done.WaitOne(1000);
                capture.StopRecording();
            }

            short[] refSignal = new short[noise.Length];
            Buffer.BlockCopy(noiseBytes, 0, refSignal, 0, noiseBytes.Length);

            short[] micSignal = new short[recorded.Count / 2];
            Buffer.BlockCopy(recorded.ToArray(), 0, micSignal, 0, recorded.Count);

            int lag = FindLagByCrossCorrelation(refSignal, micSignal, samplingRate);

            output.Dispose();
            return (double)lag / waveFormat.SampleRate;
        }

        private static int FindLagByCrossCorrelation(short[] refSignal, short[] micSignal, int samplingRate)
        {
            int maxLag = samplingRate / 2;
            double bestCorr = double.MinValue;
            int bestLag = 0;

            for (int lag = 0; lag < maxLag; lag++)
            {
                double corr = 0;
                for (int i = 0; i < refSignal.Length; i++)
                {
                    int j = i + lag;
                    if (j >= micSignal.Length) break;
                    corr += refSignal[i] * micSignal[j];
                }
                if (corr > bestCorr)
                {
                    bestCorr = corr;
                    bestLag = lag;
                }
            }
            return bestLag;
        }
    }
}
