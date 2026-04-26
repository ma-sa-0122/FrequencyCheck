using Karaoke.Programs.Pitch;
using Karaoke.Programs.TIme;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Karaoke.Programs.Analysis
{
    internal class AnalysisEngine
    {
        private readonly ConcurrentQueue<double[]> analyzeQueue;
        private readonly ConcurrentQueue<double[]> audioQueue;

        private readonly ConcurrentQueue<double[]> drawQueue;
        private readonly ConcurrentQueue<PitchResult> pitchQueue;

        private IPitchEstimator estimator;
        private TimeManager timeManager;

        private AnalysisConfig config;

        private int samplingRate;
        private int samplingInterval;
        private double latency;
        private double minEnergy;

        private double[] ringBuffer;
        private int ringIndex;

        public AnalysisEngine(
            ConcurrentQueue<double[]> analyzeQueue,
            ConcurrentQueue<double[]> audioQueue,
            ConcurrentQueue<double[]> drawQueue,
            ConcurrentQueue<PitchResult> pitchQueue,
            TimeManager timeManager,
            AnalysisConfig config,
            int samplingRate,
            int samplingInterval,
            double latency
            )
        {
            this.analyzeQueue = analyzeQueue;
            this.audioQueue = audioQueue;
            this.drawQueue = drawQueue;
            this.pitchQueue = pitchQueue;
            this.timeManager = timeManager;
            this.config = config;

            this.samplingRate = samplingRate;
            this.samplingInterval = samplingInterval;
            this.latency = latency;

            ringBuffer = new double[samplingInterval * 4];
            estimator = new NullEstimator();
        }

        public async Task Run(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (audioQueue.TryDequeue(out var audio))
                {
                    drawQueue.Enqueue(audio);
                }

                if (analyzeQueue.TryDequeue(out var data))
                {
                    double pitch = DetectPitch(data);

                    double timeSec = timeManager.GetAnalysisTime();


                    pitchQueue.Enqueue(new PitchResult
                    {
                        TimeSec = timeSec,
                        Pitch = pitch
                    });
                }
                else
                {
                    await Task.Delay(5);
                }
            }
        }

        private double DetectPitch(double[] x)
        {
            double energy = 0;
            for (int i = 0; i < x.Length; i++)
                energy += x[i] * x[i];

            if (energy < config.MinEnergy)
                return -1;

            double freq = estimator.Estimate(x);

            if (freq < 85 || freq > 1000)
                return -1;

            return 69 + 12 * Math.Log(freq / 440.0, 2);
        }

        public void SetEstimator(IPitchEstimator newEstimator)
        {
            estimator = newEstimator;
        }

    }
}
