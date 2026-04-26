using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using Karaoke.Programs.Song;

namespace Karaoke.Programs.UI
{
    internal class GuideRenderer
    {
        public Bitmap RenderPage(Chart chart, SongData song, int pageIndex)
        {
            if (song == null) return null;

            var page = song.Pages[pageIndex];
            var ca = chart.ChartAreas[0];

            Bitmap bmp = new Bitmap(chart.Width, chart.Height);

            double pageSec = page.EndSec - page.StartSec;

            float chartWidth = chart.Width;
            float chartHeight = chart.Height;

            var pos = ca.Position;
            var inner = ca.InnerPlotPosition;

            // ChartArea
            float areaX = chartWidth * pos.X / 100f;
            float areaY = chartHeight * pos.Y / 100f;
            float areaW = chartWidth * pos.Width / 100f;
            float areaH = chartHeight * pos.Height / 100f;

            // Plot領域
            float plotX = areaX + areaW * inner.X / 100f;
            float plotY = areaY + areaH * inner.Y / 100f;
            float plotW = areaW * inner.Width / 100f;
            float plotH = areaH * inner.Height / 100f;

            double xScale = plotW / pageSec;

            double yMin = ca.AxisY.Minimum;
            double yMax = ca.AxisY.Maximum;
            double yScale = plotH / (yMax - yMin);

            using (var g = Graphics.FromImage(bmp))
            {
                foreach (var note in page.Notes)
                {
                    double x1 = note.StartSec - page.StartSec;
                    double x2 = x1 + note.Duration;
                    double y = note.MidiNote;

                    float px1 = plotX + (float)(x1 * xScale);
                    float px2 = plotX + (float)(x2 * xScale);

                    float pyTop = plotY + (float)((yMax - (y + 0.5)) * yScale);
                    float pyBottom = plotY + (float)((yMax - (y - 0.5)) * yScale);

                    RectangleF rect = new RectangleF(px1, pyTop, px2 - px1, pyBottom - pyTop);

                    Color baseColor = ColorTranslator.FromHtml(note.Color);

                    using (var brush = new SolidBrush(Color.FromArgb(128, baseColor)))
                    using (var pen = new Pen(ControlPaint.Dark(baseColor), 1))
                    {
                        g.FillRectangle(brush, rect);
                        g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                    }
                }
            }

            return bmp;
        }


    }
}
