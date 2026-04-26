using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using NAudio.Dsp;
using NAudio.Wave;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;
using Series = System.Windows.Forms.DataVisualization.Charting.Series;
using Complex = System.Numerics.Complex;
using System.Collections.Concurrent;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using NAudio.CoreAudioApi;
using System.Threading;
using Karaoke.Programs.Controller;
using Karaoke.Programs.UI;
using Karaoke.Programs.Analysis;

namespace Karaoke
{
    public partial class Karaoke : Form
    {
        private KaraokeController controller;
        private ChartRenderer renderer;

        private int samplingRate = 16384;
        private int samplingInterval = 512;
        private int fftSize = 2048;

        private double latency = 0;

        private StripLine playLine; // 再生位置のガイド線

        public Karaoke()
        {
            InitializeComponent();

            controller = new KaraokeController(samplingRate, samplingInterval, fftSize);
            renderer = new ChartRenderer();

            controller.OnPageViewNeedsUpdate += RefreshPageView;
            controller.OnLyricsChanged += lyrics =>
            {
                labelLyrics.Text = lyrics;
            };

            SetDeviceList();
            SetMusicList();

            controller.UpdateAnalysisConfig(false, false, 1e-4);
            //timerUI.Start();
        }

        private void Karaoke_Shown(object sender, EventArgs e)
        {
            /* 見た目をよくするために、ダミーデータを用意 */
            chartInputWave.Series[0].Points.AddXY(0, 0);
            chartPitch.Series[0].Points.AddXY(0, 0);

            // x領域の最大値が小数、整数に依らず固定させる
            var area = chartPitch.ChartAreas[0];
            area.InnerPlotPosition.Auto = false;
            area.InnerPlotPosition.Width = 100; // 100%
            area.InnerPlotPosition.Height = 90; // 90%(横軸のメモリラベル印字分の余裕を設ける)
            area.InnerPlotPosition.X = 11;
            area.InnerPlotPosition.Y = 0;
        }


        /*
         イベント
         */
        private void buttonPlay_Click(object sender, EventArgs e)
        {
            buttonPlay.Enabled = false;
            buttonStop.Enabled = true;

            labelLyrics.Text = "（セットアップ中・・・）";

            controller.SetSamplingRate(samplingRate);
            controller.SetSamplingInterval(samplingInterval);
            controller.SetFftSize(fftSize);
            controller.SetLatency(latency);

            controller.LoadMusic(
                NoteUtils.changeFIleNameToPath(listBoxMusic.SelectedItem as string),
                (int)(latency * 1000)
            );

            controller.PlayMusic();

            controller.Start(
                controller.GetMusicTime,
                latency,
                (double)energyUpDown.Value
            );
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonPlay.Enabled = true;
            buttonStop.Enabled = false;

            controller.Stop();
            controller.StopMusic();
        }

        private void buttonDeviceUpdate_Click(object sender, EventArgs e)
        {
            SetDeviceList();
            SetMusicList();
        }

        private void buttonDeSelect_Click(object sender, EventArgs e)
        {
            listBoxMusic.SelectedIndex = -1;

            controller.LoadSong(null);
        }

        
        private void trackBarVolume_ValueChanged(object sender, EventArgs e)
        {
            controller.SetVolume(trackBarVolume.Value / 100.0f);
        }

        private void trackBarMusic_Scroll(object sender, EventArgs e)
        {
            controller.Seek(trackBarMusic.Value);
        }

        // ページ遷移ボタン
        private void buttonPageBack_Click(object sender, EventArgs e)
        {
            controller.MovePage(-1);
        }

        private void buttonPageForward_Click(object sender, EventArgs e)
        {
            controller.MovePage(1);
        }


        private void chartPitch_PrePaint(object sender, ChartPaintEventArgs e)
        {
            var bmp = controller.GetGuideBitmap();

            if (bmp != null)
            {
                e.ChartGraphics.Graphics.DrawImage(bmp, 0, 0);
            }
        }

        // チェックボックス
        private void isFourier_CheckedChanged(object sender, EventArgs e)
        {
            isHPS.Enabled = isFourier.Checked;

            if (!isFourier.Checked)
                isHPS.Checked = false;

            controller.UpdateAnalysisConfig(
                isFourier.Checked,
                isHPS.Checked,
                (double)energyUpDown.Value
            );
        }

        private void isHPS_CheckedChanged(object sender, EventArgs e)
        {
            controller.UpdateAnalysisConfig(
                isFourier.Checked,
                isHPS.Checked,
                (double)energyUpDown.Value
            );
        }

        // アップダウンリスト
        private void energyUpDown_ValueChanged(object sender, EventArgs e)
        {
            controller.UpdateAnalysisConfig(
                isFourier.Checked,
                isHPS.Checked,
                (double)energyUpDown.Value
            );
        }

        private void latencyUpDown_ValueChanged(object sender, EventArgs e)
        {
            latency = (double)latencyUpDown.Value;

            controller?.SetLatency(latency);
        }


        private void samprate_SelectedItemChanged(object sender, EventArgs e)
        {
            int rate = int.Parse(samprate.Text);
            int minThreshold = rate / (int)mps.Value;

            fftSizeUpDown.SelectedItem = null;
            fftSizeUpDown.Items.Clear();

            for (int n = 16; n >= 1; n--)
            {
                int size = 1 << n;

                if (size > rate) continue;
                if (size < minThreshold) break;

                fftSizeUpDown.Items.Add(size);
            }

            if (fftSizeUpDown.Items.Count > 0)
            {
                fftSizeUpDown.SelectedItem = fftSizeUpDown.Items[0];
            }
        }

        // キー操作
        private void Karaoke_KeyDown(object sender, KeyEventArgs e)
        {
            bool handled = false;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (OctaveUpDown.Value < OctaveUpDown.Maximum)
                    {
                        OctaveUpDown.Value += 1;
                        handled = true;
                    }
                    break;

                case Keys.Down:
                    if (OctaveUpDown.Value > OctaveUpDown.Minimum)
                    {
                        OctaveUpDown.Value -= 1;
                        handled = true;
                    }
                    break;

                case Keys.Left:
                    if (latencyUpDown.Value < latencyUpDown.Maximum)
                    {
                        latencyUpDown.Value += 0.01m;
                        handled = true;
                    }
                    break;

                case Keys.Right:
                    if (latencyUpDown.Value > latencyUpDown.Minimum)
                    {
                        latencyUpDown.Value -= 0.01m;
                        handled = true;
                    }
                    break;
            }

            if (handled)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }


        /*
         タイマー
         */
        private void timerPlay_Tick(object sender, EventArgs e)
        {
            double t = controller.GetMusicTime();

            trackBarMusic.Value = (int)t;

            int m = (int)t / 60;
            int s = (int)t % 60;

            labelCurrentTime.Text = $"{m:D2}:{s:D2}";
        }

        private void timerUI_Tick(object sender, EventArgs e)
        {
            double currentTime = controller.GetCurrentTime();

            controller.Update(currentTime);

            UpdatePlaybackLine(currentTime);

            DrawWaveform();
            DrawPitch();
        }


        /*
         処理関数群
         */
        private void SetDeviceList()
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            // データソースに直接バインド
            comboBoxDeviceIn.DataSource = devices.ToList();
            comboBoxDeviceIn.DisplayMember = "FriendlyName"; // 表示用
            comboBoxDeviceIn.ValueMember = null;
            comboBoxDeviceIn.SelectedIndex = 0;

            devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            comboBoxDeviceOut.DataSource = devices.ToList();
            comboBoxDeviceOut.DisplayMember = "FriendlyName"; // 表示用
            comboBoxDeviceOut.ValueMember = null;
            comboBoxDeviceOut.SelectedIndex = 0;
        }

        private void SetMusicList()
        {
            List<string> songNames = NoteUtils.getMusicList();
            listBoxMusic.Items.Clear();
            foreach (string name in songNames)
            {
                listBoxMusic.Items.Add(name);
            }
        }

        private void RefreshPageView()
        {
            chartPitch.Series["Singing"].Points.Clear();

            controller.ClearPitchBuffer();

            controller.SetupXAxis(chartPitch);
            controller.SetupYAxis(chartPitch);
            controller.RenderGuide(chartPitch);

            chartPitch.Invalidate();
        }

        private void UpdatePlaybackLine(double currentSec)
        {
            double pageStart = controller.GetPageStart();
            playLine.IntervalOffset = currentSec - pageStart;
        }



        private void DrawWaveform()
        {
            if (controller.TryGetWave(out var waveform))
            {
                renderer.DrawWave(chartInputWave, waveform);
                chartInputWave.Invalidate();
            }
        }

        private void DrawPitch()
        {
            var series = chartPitch.Series["Singing"];
            double pageStart = controller.GetPageStart();

            while (controller.TryGetPitch(out var p))
            {
                double relTime = p.TimeSec - pageStart;

                renderer.AddPitch(series, new PitchResult
                {
                    TimeSec = relTime,
                    Pitch = p.Pitch
                });
            }
        }


        /*
         チャート関係
         */
        private void SetupCharts()
        {
            // 波形チャート設定
            SetupChart(chartInputWave, -1, 1, samplingInterval, samplingInterval, "Waveform");

            // ピッチチャート設定
            SetupPitchChart(chartPitch);
        }

        private void SetupChart(Chart chart, double ymin, double ymax, double xmax, int interval, string seriesName)
        {
            chart.Series.Clear();
            var series = new Series(seriesName)
            {
                ChartType = SeriesChartType.FastLine
            };
            chart.Series.Add(series);

            // ChartArea設定
            var area = chart.ChartAreas[0];
            area.AxisY.Minimum = ymin;
            area.AxisY.Maximum = ymax;
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = xmax;
            area.AxisX.Interval = interval;

            // 凡例設定
            chart.Legends.Add(new Legend("Default"));
            chart.Legends[0].Docking = Docking.Top;
            chart.Legends[0].Alignment = StringAlignment.Far;
            chart.Legends[0].IsDockedInsideChartArea = true;

            // 余白調整（例: Chart全体内の描画位置調整）
            area.Position.Auto = false;
            area.Position.X = 10;
            area.Position.Y = 10;
            area.Position.Width = 100;
            area.Position.Height = 90;
        }

        private void SetupPitchChart(Chart chartPitch)
        {
            chartPitch.Series.Clear();

            // 歌声用の折れ線
            var seriesPitch = new Series("Singing")
            {
                ChartType = SeriesChartType.FastLine,
                Color = Color.Blue,
                BorderWidth = 2,
                YAxisType = AxisType.Primary,
                IsVisibleInLegend = false
            };
            chartPitch.Series.Add(seriesPitch);

            var area = chartPitch.ChartAreas[0];
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = 5;
            area.AxisY.Minimum = 48;  // C3くらい
            area.AxisY.Maximum = 72;  // C5くらい
            area.AxisY.Interval = 1;

            // 再生位置を示す赤線
            playLine = new StripLine
            {
                Interval = 0,
                StripWidth = 0,     // 幅 0 で線になる
                BorderColor = Color.Red,
                BorderWidth = 2
            };
            area.AxisX.StripLines.Add(playLine);

            // ガイド描画を PrePaint で行う。歌声を上に表示したいので、ガイドが先
            // PostPaint を使用すると、ガイドが折れ線の上に表示される。
            chartPitch.PrePaint += chartPitch_PrePaint;
        }

        private void Reset_chartInputWave(double xmax)
        {
            var area = chartInputWave.ChartAreas[0];
            area.AxisX.Maximum = xmax;
            area.AxisX.Interval = xmax;

            /* ダミーデータを描画。再描画時は Clear ではなく SetValueY で */
            var waveform = chartInputWave.Series[0];
            waveform.Points.Clear();
            for (int i = 0; i < samplingInterval; i++)
                waveform.Points.AddXY(i, 0);
        }
    }
}