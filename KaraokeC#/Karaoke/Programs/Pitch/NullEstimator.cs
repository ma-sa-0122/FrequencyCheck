using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaoke.Programs.Pitch
{
    internal class NullEstimator : IPitchEstimator
    {
        public double Estimate(double[] buffer)
        {
            return -1;
        }
    }
}
