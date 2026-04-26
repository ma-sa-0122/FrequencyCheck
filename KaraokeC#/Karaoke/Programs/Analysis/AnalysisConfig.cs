using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Karaoke.Programs.Analysis
{
    internal class AnalysisConfig
    {
        public volatile bool UseFourier;
        public volatile bool UseHPS;
        private double _MinEnergy;

        public double MinEnergy
        {
            get => Volatile.Read(ref _MinEnergy);
            set => Volatile.Write(ref _MinEnergy, value);
        }
    }
}
