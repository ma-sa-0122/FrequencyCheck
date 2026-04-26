using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaoke.Programs.Pitch
{
    internal class PitchEstimatorFactory
    {
        public static IPitchEstimator Create(bool useFourier, bool useHps, int sampleRate, int fftSize)
        {
            if (!useFourier)
                return new YinEstimator(sampleRate);

            if (useHps)
                return new HpsEstimator(sampleRate, fftSize);

            return new FftEstimator(sampleRate, fftSize);
        }
    }
}
