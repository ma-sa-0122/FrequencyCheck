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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartInputWave = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartPitch = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.isFourier = new System.Windows.Forms.RadioButton();
            this.isYIN = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fftSizeUpDown = new System.Windows.Forms.DomainUpDown();
            this.isHPS = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.energyUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.listBoxDevices = new System.Windows.Forms.ListBox();
            this.buttonDeviceUpdate = new System.Windows.Forms.Button();
            this.timerUI = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.mps = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.samprate = new System.Windows.Forms.DomainUpDown();
            this.listBoxMusic = new System.Windows.Forms.ListBox();
            this.trackBarMusic = new System.Windows.Forms.TrackBar();
            this.labelLyrics = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.OctaveUpDown = new System.Windows.Forms.NumericUpDown();
            this.labelFreq = new System.Windows.Forms.Label();
            this.timerPlay = new System.Windows.Forms.Timer(this.components);
            this.buttonDeSelect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chartInputWave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPitch)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.energyUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMusic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OctaveUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // chartInputWave
            // 
            chartArea3.Name = "ChartArea1";
            this.chartInputWave.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chartInputWave.Legends.Add(legend3);
            this.chartInputWave.Location = new System.Drawing.Point(562, 12);
            this.chartInputWave.Name = "chartInputWave";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chartInputWave.Series.Add(series3);
            this.chartInputWave.Size = new System.Drawing.Size(500, 452);
            this.chartInputWave.TabIndex = 0;
            this.chartInputWave.TabStop = false;
            this.chartInputWave.Text = "chart1";
            // 
            // chartPitch
            // 
            chartArea4.Name = "ChartArea1";
            this.chartPitch.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.chartPitch.Legends.Add(legend4);
            this.chartPitch.Location = new System.Drawing.Point(12, 517);
            this.chartPitch.Name = "chartPitch";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chartPitch.Series.Add(series4);
            this.chartPitch.Size = new System.Drawing.Size(1050, 500);
            this.chartPitch.TabIndex = 2;
            this.chartPitch.TabStop = false;
            this.chartPitch.Text = "chart3";
            // 
            // buttonPlay
            // 
            this.buttonPlay.Location = new System.Drawing.Point(12, 338);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(272, 49);
            this.buttonPlay.TabIndex = 3;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(290, 337);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(253, 50);
            this.buttonStop.TabIndex = 4;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // isFourier
            // 
            this.isFourier.AutoSize = true;
            this.isFourier.Checked = true;
            this.isFourier.Location = new System.Drawing.Point(5, 3);
            this.isFourier.Name = "isFourier";
            this.isFourier.Size = new System.Drawing.Size(219, 28);
            this.isFourier.TabIndex = 5;
            this.isFourier.TabStop = true;
            this.isFourier.Text = "Fourier Transform";
            this.isFourier.UseVisualStyleBackColor = true;
            this.isFourier.CheckedChanged += new System.EventHandler(this.isFourier_CheckedChanged);
            // 
            // isYIN
            // 
            this.isYIN.AutoSize = true;
            this.isYIN.Location = new System.Drawing.Point(6, 45);
            this.isYIN.Name = "isYIN";
            this.isYIN.Size = new System.Drawing.Size(76, 28);
            this.isYIN.TabIndex = 6;
            this.isYIN.Text = "YIN";
            this.isYIN.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fftSizeUpDown);
            this.panel1.Controls.Add(this.isHPS);
            this.panel1.Controls.Add(this.isYIN);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.isFourier);
            this.panel1.Location = new System.Drawing.Point(9, 256);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(547, 76);
            this.panel1.TabIndex = 7;
            // 
            // fftSizeUpDown
            // 
            this.fftSizeUpDown.Location = new System.Drawing.Point(370, 32);
            this.fftSizeUpDown.Name = "fftSizeUpDown";
            this.fftSizeUpDown.ReadOnly = true;
            this.fftSizeUpDown.Size = new System.Drawing.Size(164, 31);
            this.fftSizeUpDown.TabIndex = 19;
            this.fftSizeUpDown.Text = "16384";
            // 
            // isHPS
            // 
            this.isHPS.AutoSize = true;
            this.isHPS.Location = new System.Drawing.Point(240, 4);
            this.isHPS.Name = "isHPS";
            this.isHPS.Size = new System.Drawing.Size(86, 28);
            this.isHPS.TabIndex = 7;
            this.isHPS.Text = "HPS";
            this.isHPS.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(394, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(115, 24);
            this.label6.TabIndex = 20;
            this.label6.Text = "FFT size : ";
            // 
            // energyUpDown
            // 
            this.energyUpDown.DecimalPlaces = 8;
            this.energyUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.energyUpDown.Location = new System.Drawing.Point(379, 219);
            this.energyUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.energyUpDown.Name = "energyUpDown";
            this.energyUpDown.Size = new System.Drawing.Size(164, 31);
            this.energyUpDown.TabIndex = 8;
            this.energyUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 221);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(368, 24);
            this.label3.TabIndex = 7;
            this.label3.Text = "energy under limit (mic sensitivity) : ";
            // 
            // listBoxDevices
            // 
            this.listBoxDevices.FormattingEnabled = true;
            this.listBoxDevices.ItemHeight = 24;
            this.listBoxDevices.Location = new System.Drawing.Point(12, 62);
            this.listBoxDevices.Name = "listBoxDevices";
            this.listBoxDevices.Size = new System.Drawing.Size(272, 76);
            this.listBoxDevices.TabIndex = 8;
            // 
            // buttonDeviceUpdate
            // 
            this.buttonDeviceUpdate.Location = new System.Drawing.Point(12, 12);
            this.buttonDeviceUpdate.Name = "buttonDeviceUpdate";
            this.buttonDeviceUpdate.Size = new System.Drawing.Size(123, 44);
            this.buttonDeviceUpdate.TabIndex = 9;
            this.buttonDeviceUpdate.Text = "update";
            this.buttonDeviceUpdate.UseVisualStyleBackColor = true;
            this.buttonDeviceUpdate.Click += new System.EventHandler(this.buttonDeviceUpdate_Click);
            // 
            // timerUI
            // 
            this.timerUI.Interval = 40;
            this.timerUI.Tick += new System.EventHandler(this.timerUI_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 184);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 24);
            this.label1.TabIndex = 11;
            this.label1.Text = "measures / sec : ";
            // 
            // mps
            // 
            this.mps.Location = new System.Drawing.Point(379, 182);
            this.mps.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mps.Name = "mps";
            this.mps.Size = new System.Drawing.Size(164, 31);
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
            this.label2.Location = new System.Drawing.Point(11, 146);
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
            this.samprate.Location = new System.Drawing.Point(379, 144);
            this.samprate.Name = "samprate";
            this.samprate.ReadOnly = true;
            this.samprate.Size = new System.Drawing.Size(164, 31);
            this.samprate.TabIndex = 14;
            this.samprate.Text = "16384";
            this.samprate.SelectedItemChanged += new System.EventHandler(this.samprate_SelectedItemChanged);
            // 
            // listBoxMusic
            // 
            this.listBoxMusic.FormattingEnabled = true;
            this.listBoxMusic.ItemHeight = 24;
            this.listBoxMusic.Location = new System.Drawing.Point(290, 62);
            this.listBoxMusic.Name = "listBoxMusic";
            this.listBoxMusic.Size = new System.Drawing.Size(253, 76);
            this.listBoxMusic.TabIndex = 15;
            // 
            // trackBarMusic
            // 
            this.trackBarMusic.Location = new System.Drawing.Point(12, 422);
            this.trackBarMusic.Name = "trackBarMusic";
            this.trackBarMusic.Size = new System.Drawing.Size(544, 90);
            this.trackBarMusic.TabIndex = 16;
            // 
            // labelLyrics
            // 
            this.labelLyrics.Font = new System.Drawing.Font("MS UI Gothic", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelLyrics.Location = new System.Drawing.Point(12, 476);
            this.labelLyrics.Name = "labelLyrics";
            this.labelLyrics.Size = new System.Drawing.Size(1050, 35);
            this.labelLyrics.TabIndex = 17;
            this.labelLyrics.Text = "歌詞";
            this.labelLyrics.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(161, 395);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 24);
            this.label4.TabIndex = 18;
            this.label4.Text = "Ovtave : ";
            // 
            // OctaveUpDown
            // 
            this.OctaveUpDown.Location = new System.Drawing.Point(266, 393);
            this.OctaveUpDown.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.OctaveUpDown.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            -2147483648});
            this.OctaveUpDown.Name = "OctaveUpDown";
            this.OctaveUpDown.Size = new System.Drawing.Size(111, 31);
            this.OctaveUpDown.TabIndex = 9;
            // 
            // labelFreq
            // 
            this.labelFreq.AutoSize = true;
            this.labelFreq.Location = new System.Drawing.Point(1007, 487);
            this.labelFreq.Name = "labelFreq";
            this.labelFreq.Size = new System.Drawing.Size(55, 24);
            this.labelFreq.TabIndex = 19;
            this.labelFreq.Text = "Freq";
            // 
            // timerPlay
            // 
            this.timerPlay.Interval = 1000;
            this.timerPlay.Tick += new System.EventHandler(this.timerPlay_Tick);
            // 
            // buttonDeSelect
            // 
            this.buttonDeSelect.Location = new System.Drawing.Point(290, 12);
            this.buttonDeSelect.Name = "buttonDeSelect";
            this.buttonDeSelect.Size = new System.Drawing.Size(131, 44);
            this.buttonDeSelect.TabIndex = 20;
            this.buttonDeSelect.Text = "de-select";
            this.buttonDeSelect.UseVisualStyleBackColor = true;
            this.buttonDeSelect.Click += new System.EventHandler(this.buttonDeSelect_Click);
            // 
            // Karaoke
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1074, 1029);
            this.Controls.Add(this.buttonDeSelect);
            this.Controls.Add(this.labelFreq);
            this.Controls.Add(this.energyUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.OctaveUpDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelLyrics);
            this.Controls.Add(this.trackBarMusic);
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
            this.Shown += new System.EventHandler(this.Karaoke_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.chartInputWave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPitch)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.energyUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMusic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OctaveUpDown)).EndInit();
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
        private System.Windows.Forms.TrackBar trackBarMusic;
        private System.Windows.Forms.Label labelLyrics;
        private System.Windows.Forms.NumericUpDown energyUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown OctaveUpDown;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DomainUpDown fftSizeUpDown;
        private System.Windows.Forms.CheckBox isHPS;
        private System.Windows.Forms.Label labelFreq;
        private System.Windows.Forms.Timer timerPlay;
        private System.Windows.Forms.Button buttonDeSelect;
    }
}

