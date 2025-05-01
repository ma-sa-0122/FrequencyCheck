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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
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
            ((System.ComponentModel.ISupportInitialize)(this.chartInputWave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartCumAve)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPitch)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chartInputWave
            // 
            chartArea1.Name = "ChartArea1";
            this.chartInputWave.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartInputWave.Legends.Add(legend1);
            this.chartInputWave.Location = new System.Drawing.Point(12, 12);
            this.chartInputWave.Name = "chartInputWave";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartInputWave.Series.Add(series1);
            this.chartInputWave.Size = new System.Drawing.Size(500, 450);
            this.chartInputWave.TabIndex = 0;
            this.chartInputWave.TabStop = false;
            this.chartInputWave.Text = "chart1";
            // 
            // chartCumAve
            // 
            chartArea2.Name = "ChartArea1";
            this.chartCumAve.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartCumAve.Legends.Add(legend2);
            this.chartCumAve.Location = new System.Drawing.Point(562, 12);
            this.chartCumAve.Name = "chartCumAve";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartCumAve.Series.Add(series2);
            this.chartCumAve.Size = new System.Drawing.Size(500, 450);
            this.chartCumAve.TabIndex = 1;
            this.chartCumAve.TabStop = false;
            this.chartCumAve.Text = "chart2";
            // 
            // chartPitch
            // 
            chartArea3.Name = "ChartArea1";
            this.chartPitch.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chartPitch.Legends.Add(legend3);
            this.chartPitch.Location = new System.Drawing.Point(562, 517);
            this.chartPitch.Name = "chartPitch";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chartPitch.Series.Add(series3);
            this.chartPitch.Size = new System.Drawing.Size(500, 500);
            this.chartPitch.TabIndex = 2;
            this.chartPitch.TabStop = false;
            this.chartPitch.Text = "chart3";
            // 
            // buttonRec
            // 
            this.buttonRec.Location = new System.Drawing.Point(65, 815);
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
            this.buttonStop.Location = new System.Drawing.Point(65, 916);
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
            this.panel1.Location = new System.Drawing.Point(65, 682);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(403, 108);
            this.panel1.TabIndex = 7;
            // 
            // listBoxDevices
            // 
            this.listBoxDevices.FormattingEnabled = true;
            this.listBoxDevices.ItemHeight = 24;
            this.listBoxDevices.Location = new System.Drawing.Point(65, 538);
            this.listBoxDevices.Name = "listBoxDevices";
            this.listBoxDevices.Size = new System.Drawing.Size(303, 124);
            this.listBoxDevices.TabIndex = 8;
            // 
            // buttonDeviceUpdate
            // 
            this.buttonDeviceUpdate.Location = new System.Drawing.Point(381, 563);
            this.buttonDeviceUpdate.Name = "buttonDeviceUpdate";
            this.buttonDeviceUpdate.Size = new System.Drawing.Size(87, 73);
            this.buttonDeviceUpdate.TabIndex = 9;
            this.buttonDeviceUpdate.Text = "update";
            this.buttonDeviceUpdate.UseVisualStyleBackColor = true;
            this.buttonDeviceUpdate.Click += new System.EventHandler(this.buttonDeviceUpdate_Click);
            // 
            // PichPrinter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1074, 1029);
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
            ((System.ComponentModel.ISupportInitialize)(this.chartInputWave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartCumAve)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPitch)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

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
    }
}

