using Karaoke.Programs.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Karaoke.Programs.UI
{
    internal class ChartRenderer
    {
        public void DrawWave(Chart chart, double[] data)
        {
            var s = chart.Series[0];

            int count = Math.Min(s.Points.Count, data.Length);

            for (int i = 0; i < data.Length; i++)
                s.Points[i].SetValueY(data[i]);
        }

        public void AddPitch(Series series, PitchResult p)
        {
            series.Points.AddXY(p.TimeSec, p.Pitch > 0 ? p.Pitch : double.NaN);
        }
    }
}
