using Karaoke.Programs.Analysis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karaoke.Programs.Data
{
    internal class PitchBuffer
    {
        private PitchResult[] buffer;
        private int index;
        private int size;

        public PitchBuffer(int size = 1024)
        {
            this.size = size;
            buffer = new PitchResult[size];

            for (int i = 0; i < size; i++)
                buffer[i] = new PitchResult { TimeSec = 0, Pitch = double.NaN };
        }

        public void Add(PitchResult p)
        {
            buffer[index].TimeSec = p.TimeSec;
            buffer[index].Pitch = p.Pitch;

            index++;
            if (index >= size)
                index = 0;
        }

        public PitchResult[] GetAll()
        {
            return buffer;
        }

        public void Clear()
        {
            index = 0;

            for (int i = 0; i < size; i++)
            {
                buffer[i].TimeSec = 0;
                buffer[i].Pitch = double.NaN;
            }
        }
    }

}
