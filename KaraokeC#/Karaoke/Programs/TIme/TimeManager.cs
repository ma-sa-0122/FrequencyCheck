using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaoke.Programs.TIme
{
    internal class TimeManager
    {
        private Stopwatch stopwatch = new Stopwatch();

        private Func<double> musicTimeProvider;
        private double latency;

        private int samplingRate;
        private int frameSize;

        public TimeManager(int samplingRate, int frameSize)
        {
            this.samplingRate = samplingRate;
            this.frameSize = frameSize;
        }

        public void SetMusicTimeProvider(Func<double> provider)
        {
            musicTimeProvider = provider;
        }

        public void SetLatency(double latencySec)
        {
            latency = latencySec;
        }

        public void Start()
        {
            stopwatch.Restart();
        }

        public void Stop()
        {
            stopwatch.Stop();
        }

        /// <summary>
        /// UI表示・ガイド用の「現在時刻」
        /// </summary>
        public double GetCurrentTime()
        {
            if (musicTimeProvider != null)
                return musicTimeProvider();

            return stopwatch.Elapsed.TotalSeconds;
        }

        /// <summary>
        /// ピッチ解析結果に付与する時間（補正込み）
        /// </summary>
        public double GetAnalysisTime()
        {
            double baseTime = GetCurrentTime();

            // フレーム中心補正
            double frameOffset = (frameSize / (double)samplingRate) / 2.0;

            return baseTime - latency - frameOffset;
        }
    }
}
