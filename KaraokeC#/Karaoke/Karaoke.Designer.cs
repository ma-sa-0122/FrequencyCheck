namespace Karaoke
{
    partial class Karaoke
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartInputWave = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartPitch = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.buttonPlay = new System.Windows.Forms.Button();
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
            this.listBoxMusic = new System.Windows.Forms.ListBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.labelLyrics = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartInputWave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPitch)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // chartInputWave
            // 
            chartArea1.Name = "ChartArea1";
            this.chartInputWave.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartInputWave.Legends.Add(legend1);
            this.chartInputWave.Location = new System.Drawing.Point(562, 12);
            this.chartInputWave.Name = "chartInputWave";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartInputWave.Series.Add(series1);
            this.chartInputWave.Size = new System.Drawing.Size(500, 452);
            this.chartInputWave.TabIndex = 0;
            this.chartInputWave.TabStop = false;
            this.chartInputWave.Text = "chart1";
            // 
            // chartPitch
            // 
            chartArea2.Name = "ChartArea1";
            this.chartPitch.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartPitch.Legends.Add(legend2);
            this.chartPitch.Location = new System.Drawing.Point(12, 517);
            this.chartPitch.Name = "chartPitch";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartPitch.Series.Add(series2);
            this.chartPitch.Size = new System.Drawing.Size(1050, 500);
            this.chartPitch.TabIndex = 2;
            this.chartPitch.TabStop = false;
            this.chartPitch.Text = "chart3";
            // 
            // buttonPlay
            // 
            this.buttonPlay.Location = new System.Drawing.Point(12, 360);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(245, 56);
            this.buttonPlay.TabIndex = 3;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(263, 360);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(244, 56);
            this.buttonStop.TabIndex = 4;
            this.buttonStop.Text = "Stop";
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
            this.panel1.Location = new System.Drawing.Point(12, 232);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(495, 108);
            this.panel1.TabIndex = 7;
            // 
            // listBoxDevices
            // 
            this.listBoxDevices.FormattingEnabled = true;
            this.listBoxDevices.ItemHeight = 24;
            this.listBoxDevices.Location = new System.Drawing.Point(12, 62);
            this.listBoxDevices.Name = "listBoxDevices";
            this.listBoxDevices.Size = new System.Drawing.Size(245, 76);
            this.listBoxDevices.TabIndex = 8;
            // 
            // buttonDeviceUpdate
            // 
            this.buttonDeviceUpdate.Location = new System.Drawing.Point(12, 12);
            this.buttonDeviceUpdate.Name = "buttonDeviceUpdate";
            this.buttonDeviceUpdate.Size = new System.Drawing.Size(86, 44);
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
            this.label1.Location = new System.Drawing.Point(8, 197);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(249, 24);
            this.label1.TabIndex = 11;
            this.label1.Text = "num of measure / sec : ";
            // 
            // mps
            // 
            this.mps.Location = new System.Drawing.Point(387, 195);
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
            this.label2.Location = new System.Drawing.Point(8, 159);
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
            this.samprate.Location = new System.Drawing.Point(387, 157);
            this.samprate.Name = "samprate";
            this.samprate.Size = new System.Drawing.Size(120, 31);
            this.samprate.TabIndex = 14;
            this.samprate.Text = "16384";
            // 
            // listBoxMusic
            // 
            this.listBoxMusic.FormattingEnabled = true;
            this.listBoxMusic.ItemHeight = 24;
            this.listBoxMusic.Location = new System.Drawing.Point(263, 62);
            this.listBoxMusic.Name = "listBoxMusic";
            this.listBoxMusic.Size = new System.Drawing.Size(244, 76);
            this.listBoxMusic.TabIndex = 15;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(12, 422);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(495, 90);
            this.trackBar1.TabIndex = 16;
            // 
            // labelLyrics
            // 
            this.labelLyrics.AutoSize = true;
            this.labelLyrics.Font = new System.Drawing.Font("MS UI Gothic", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelLyrics.Location = new System.Drawing.Point(12, 483);
            this.labelLyrics.Name = "labelLyrics";
            this.labelLyrics.Size = new System.Drawing.Size(71, 29);
            this.labelLyrics.TabIndex = 17;
            this.labelLyrics.Text = "歌詞";
            // 
            // Karaoke
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1074, 1029);
            this.Controls.Add(this.labelLyrics);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.listBoxMusic);
            this.Controls.Add(this.samprate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mps);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonDeviceUpdate);
            this.Controls.Add(this.listBoxDevices);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.chartPitch);
            this.Controls.Add(this.chartInputWave);
            this.MaximizeBox = false;
            this.Name = "Karaoke";
            this.Text = "Karaoke";
            this.Shown += new System.EventHandler(this.PitchPrinter_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.chartInputWave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPitch)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartInputWave;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPitch;
        private System.Windows.Forms.Button buttonPlay;
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
        private System.Windows.Forms.ListBox listBoxMusic;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label labelLyrics;
    }
}

