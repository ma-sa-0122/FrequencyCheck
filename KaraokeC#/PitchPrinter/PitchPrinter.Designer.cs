namespace Karaoke
{
    partial class PichPrinter
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea13 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend13 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series13 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea14 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend14 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series14 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea15 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend15 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series15 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartInputWave = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartCumAve = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartPitch = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.buttonRec = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.isFourier = new System.Windows.Forms.RadioButton();
            this.isYIN = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listBoxDevices = new System.Windows.Forms.ListBox();
            this.buttonDeviceUpdate = new System.Windows.Forms.Button();
            this.timerUI = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.mps = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.samprate = new System.Windows.Forms.DomainUpDown();
            this.scale = new System.Windows.Forms.DomainUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartInputWave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartCumAve)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPitch)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mps)).BeginInit();
            this.SuspendLayout();
            // 
            // chartInputWave
            // 
            chartArea13.Name = "ChartArea1";
            this.chartInputWave.ChartAreas.Add(chartArea13);
            legend13.Name = "Legend1";
            this.chartInputWave.Legends.Add(legend13);
            this.chartInputWave.Location = new System.Drawing.Point(12, 12);
            this.chartInputWave.Name = "chartInputWave";
            series13.ChartArea = "ChartArea1";
            series13.Legend = "Legend1";
            series13.Name = "Series1";
            this.chartInputWave.Series.Add(series13);
            this.chartInputWave.Size = new System.Drawing.Size(500, 450);
            this.chartInputWave.TabIndex = 0;
            this.chartInputWave.TabStop = false;
            this.chartInputWave.Text = "chart1";
            // 
            // chartCumAve
            // 
            chartArea14.Name = "ChartArea1";
            this.chartCumAve.ChartAreas.Add(chartArea14);
            legend14.Name = "Legend1";
            this.chartCumAve.Legends.Add(legend14);
            this.chartCumAve.Location = new System.Drawing.Point(562, 12);
            this.chartCumAve.Name = "chartCumAve";
            series14.ChartArea = "ChartArea1";
            series14.Legend = "Legend1";
            series14.Name = "Series1";
            this.chartCumAve.Series.Add(series14);
            this.chartCumAve.Size = new System.Drawing.Size(500, 450);
            this.chartCumAve.TabIndex = 1;
            this.chartCumAve.TabStop = false;
            this.chartCumAve.Text = "chart2";
            // 
            // chartPitch
            // 
            chartArea15.Name = "ChartArea1";
            this.chartPitch.ChartAreas.Add(chartArea15);
            legend15.Name = "Legend1";
            this.chartPitch.Legends.Add(legend15);
            this.chartPitch.Location = new System.Drawing.Point(562, 517);
            this.chartPitch.Name = "chartPitch";
            series15.ChartArea = "ChartArea1";
            series15.Legend = "Legend1";
            series15.Name = "Series1";
            this.chartPitch.Series.Add(series15);
            this.chartPitch.Size = new System.Drawing.Size(500, 500);
            this.chartPitch.TabIndex = 2;
            this.chartPitch.TabStop = false;
            this.chartPitch.Text = "chart3";
            // 
            // buttonRec
            // 
            this.buttonRec.Location = new System.Drawing.Point(65, 833);
            this.buttonRec.Name = "buttonRec";
            this.buttonRec.Size = new System.Drawing.Size(403, 81);
            this.buttonRec.TabIndex = 3;
            this.buttonRec.Text = "Start REC";
            this.buttonRec.UseVisualStyleBackColor = true;
            this.buttonRec.Click += new System.EventHandler(this.buttonRec_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(65, 936);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(403, 81);
            this.buttonStop.TabIndex = 4;
            this.buttonStop.Text = "Stop REC";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // isFourier
            // 
            this.isFourier.AutoSize = true;
            this.isFourier.Checked = true;
            this.isFourier.Location = new System.Drawing.Point(47, 20);
            this.isFourier.Name = "isFourier";
            this.isFourier.Size = new System.Drawing.Size(219, 28);
            this.isFourier.TabIndex = 5;
            this.isFourier.TabStop = true;
            this.isFourier.Text = "Fourier Transform";
            this.isFourier.UseVisualStyleBackColor = true;
            // 
            // isYIN
            // 
            this.isYIN.AutoSize = true;
            this.isYIN.Location = new System.Drawing.Point(47, 68);
            this.isYIN.Name = "isYIN";
            this.isYIN.Size = new System.Drawing.Size(76, 28);
            this.isYIN.TabIndex = 6;
            this.isYIN.Text = "YIN";
            this.isYIN.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.isYIN);
            this.panel1.Controls.Add(this.isFourier);
            this.panel1.Location = new System.Drawing.Point(65, 702);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(403, 108);
            this.panel1.TabIndex = 7;
            // 
            // listBoxDevices
            // 
            this.listBoxDevices.FormattingEnabled = true;
            this.listBoxDevices.ItemHeight = 24;
            this.listBoxDevices.Location = new System.Drawing.Point(65, 517);
            this.listBoxDevices.Name = "listBoxDevices";
            this.listBoxDevices.Size = new System.Drawing.Size(303, 76);
            this.listBoxDevices.TabIndex = 8;
            // 
            // buttonDeviceUpdate
            // 
            this.buttonDeviceUpdate.Location = new System.Drawing.Point(381, 517);
            this.buttonDeviceUpdate.Name = "buttonDeviceUpdate";
            this.buttonDeviceUpdate.Size = new System.Drawing.Size(87, 76);
            this.buttonDeviceUpdate.TabIndex = 9;
            this.buttonDeviceUpdate.Text = "update";
            this.buttonDeviceUpdate.UseVisualStyleBackColor = true;
            this.buttonDeviceUpdate.Click += new System.EventHandler(this.buttonDeviceUpdate_Click);
            // 
            // timerUI
            // 
            this.timerUI.Interval = 20;
            this.timerUI.Tick += new System.EventHandler(this.timerUI_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 652);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(249, 24);
            this.label1.TabIndex = 11;
            this.label1.Text = "num of measure / sec : ";
            // 
            // mps
            // 
            this.mps.Location = new System.Drawing.Point(348, 650);
            this.mps.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mps.Name = "mps";
            this.mps.Size = new System.Drawing.Size(120, 31);
            this.mps.TabIndex = 12;
            this.mps.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 614);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 24);
            this.label2.TabIndex = 13;
            this.label2.Text = "sampling rate : ";
            // 
            // samprate
            // 
            this.samprate.Items.Add("32768");
            this.samprate.Items.Add("22100");
            this.samprate.Items.Add("16384");
            this.samprate.Items.Add("11052");
            this.samprate.Items.Add("8192");
            this.samprate.Location = new System.Drawing.Point(348, 612);
            this.samprate.Name = "samprate";
            this.samprate.Size = new System.Drawing.Size(120, 31);
            this.samprate.TabIndex = 14;
            this.samprate.Text = "16384";
            // 
            // scale
            // 
            this.scale.Items.Add("B / G#m (#5)");
            this.scale.Items.Add("E / C#m (#4)");
            this.scale.Items.Add("A / F#m (#3)");
            this.scale.Items.Add("D / Bm (#2)");
            this.scale.Items.Add("G / Em (#1)");
            this.scale.Items.Add("C / Am");
            this.scale.Items.Add("F / Dm (b1)");
            this.scale.Items.Add("Bb / Gm (b2)");
            this.scale.Items.Add("Eb / Cm (b3)");
            this.scale.Items.Add("Ab / Fm (b4)");
            this.scale.Items.Add("Db / Bbm (b5)");
            this.scale.Items.Add("Gb / Ebm (b6)");
            this.scale.Location = new System.Drawing.Point(647, 480);
            this.scale.Name = "scale";
            this.scale.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.scale.Size = new System.Drawing.Size(415, 31);
            this.scale.TabIndex = 15;
            this.scale.Text = "C / Am";
            this.scale.SelectedItemChanged += new System.EventHandler(this.scale_SelectedItemChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(558, 482);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 24);
            this.label3.TabIndex = 16;
            this.label3.Text = "Scale : ";
            // 
            // PichPrinter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1074, 1029);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.scale);
            this.Controls.Add(this.samprate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mps);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonDeviceUpdate);
            this.Controls.Add(this.listBoxDevices);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonRec);
            this.Controls.Add(this.chartPitch);
            this.Controls.Add(this.chartCumAve);
            this.Controls.Add(this.chartInputWave);
            this.MaximizeBox = false;
            this.Name = "PichPrinter";
            this.Text = "PitchPrinter";
            this.Shown += new System.EventHandler(this.PitchPrinter_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.chartInputWave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartCumAve)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPitch)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mps)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartInputWave;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartCumAve;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPitch;
        private System.Windows.Forms.Button buttonRec;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.RadioButton isFourier;
        private System.Windows.Forms.RadioButton isYIN;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox listBoxDevices;
        private System.Windows.Forms.Button buttonDeviceUpdate;
        private System.Windows.Forms.Timer timerUI;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown mps;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DomainUpDown samprate;
        private System.Windows.Forms.DomainUpDown scale;
        private System.Windows.Forms.Label label3;
    }
}

