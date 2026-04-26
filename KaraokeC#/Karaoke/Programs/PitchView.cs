using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using Karaoke.Programs.Song;

namespace Karaoke
{
    internal class PitchView : Control
    {
        public PitchView() {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
        }

        public SongData Song;
        public int CurrentPage;
        public double CurrentTime; // 再生位置

        public double[] PitchArray;

        float marginL = 50, marginR = 10, marginT = 10, marginB = 20;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Song == null || Song.Pages.Count == 0) return;

            var page = Song.Pages[CurrentPage];
            DrawPage(e.Graphics, page);
            DrawPitch(e.Graphics, page);
            DrawPlayLine(e.Graphics, page);
            DrawYAxis(e.Graphics, page);
        }

        private void DrawPage(Graphics g, PageInfo page)
        {
            var (plotX, plotY, plotW, plotH) = GetPlotRect();

            double pageSec = page.EndSec - page.StartSec;

            double xScale = plotW / pageSec;

            int minNote = page.Notes.Count > 0 ? page.Notes.Min(n => n.MidiNote) : 60;
            int maxNote = page.Notes.Count > 0 ? page.Notes.Max(n => n.MidiNote) : 60;

            double yScale = plotH / (maxNote - minNote + 1);

            foreach (var note in page.Notes)
            {
                double x1 = note.StartSec - page.StartSec;
                double x2 = x1 + note.Duration;

                float px1 = plotX + (float)(x1 * xScale);
                float px2 = plotX + (float)(x2 * xScale);

                float py = plotY + (float)((maxNote - note.MidiNote) * yScale);

                RectangleF rect = new RectangleF(px1, py, px2 - px1, (float)yScale);

                Color baseColor = ColorTranslator.FromHtml(note.Color);

                using (var brush = new SolidBrush(Color.FromArgb(128, baseColor)))
                using (var pen = new Pen(ControlPaint.Dark(baseColor)))
                {
                    g.FillRectangle(brush, rect);
                    g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                }
            }
        }


        private void DrawPitch(Graphics g, PageInfo page)
        {
            if (PitchArray == null || PitchArray.Length < 2) return;

            var (plotX, plotY, plotW, plotH) = GetPlotRect();

            double pageSec = page.EndSec - page.StartSec;

            int minNote = page.Notes.Min(n => n.MidiNote);
            int maxNote = page.Notes.Max(n => n.MidiNote);

            double yScale = plotH / (maxNote - minNote + 1);

            // 1px に書く点数を削減（軽量化）
            int step = Math.Max(1, PitchArray.Length / (int)plotW);

            using (var pen = new Pen(Color.Blue, 2))
            {
                for (int i = step; i < PitchArray.Length; i += step)
                {
                    double p1 = PitchArray[i - step];
                    double p2 = PitchArray[i];

                    if (p1 <= 0 || p2 <= 0) continue;

                    double t1 = (double)(i - step) / PitchArray.Length * pageSec;
                    double t2 = (double)i / PitchArray.Length * pageSec;

                    float x1 = plotX + (float)(t1 / pageSec * plotW);
                    float x2 = plotX + (float)(t2 / pageSec * plotW);

                    float y1 = plotY + (float)((maxNote - p1) * yScale);
                    float y2 = plotY + (float)((maxNote - p2) * yScale);

                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
        }


        private void DrawPlayLine(Graphics g, PageInfo page)
        {
            var (plotX, _, plotW, _) = GetPlotRect();

            double pageSec = page.EndSec - page.StartSec;
            double x = CurrentTime - page.StartSec;

            float px = plotX + (float)(x / pageSec * plotW);

            using (var pen = new Pen(Color.Red, 2))
            {
                g.DrawLine(pen, px, 0, px, Height);
            }
        }


        private void DrawYAxis(Graphics g, PageInfo page)
        {
            var (_, plotY, _, plotH) = GetPlotRect();

            int minNote = page.Notes.Count > 0 ? page.Notes.Min(n => n.MidiNote) : 60;
            int maxNote = page.Notes.Count > 0 ? page.Notes.Max(n => n.MidiNote) : 60;

            double yScale = plotH / (maxNote - minNote + 1);

            string[] noteNames = { "C", "C#", "D", "D#", "E", "F",
                           "F#", "G", "G#", "A", "A#", "B" };

            for (int m = minNote; m <= maxNote; m++)
            {
                float y = plotY + (float)((maxNote - m) * yScale);
                g.DrawString(noteNames[m % 12], Font, Brushes.Black, 5, y);
            }
        }



        private (float x, float y, float w, float h) GetPlotRect()
        {
            float w = Width - marginL - marginR;
            float h = Height - marginT - marginB;
            return (marginL, marginT, w, h);
        }
    }
}
