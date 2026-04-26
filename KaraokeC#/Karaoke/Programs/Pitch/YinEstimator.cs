using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaoke.Programs.Pitch
{
    internal class YinEstimator : IPitchEstimator
    {
        private int sampleRate;

        public YinEstimator(int sampleRate = 44100)
        {
            this.sampleRate = sampleRate;
        }

        public double Estimate(double[] x)
        {
            int window = x.Length / 2;

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

            double detectedFreq = sampleRate / betterFreq;

            return detectedFreq;
        }
    }
}
