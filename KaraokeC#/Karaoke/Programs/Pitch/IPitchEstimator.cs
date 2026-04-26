
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaoke.Programs.Pitch
{
    internal interface IPitchEstimator
    {
        double Estimate(double[] frame);
    }
}
