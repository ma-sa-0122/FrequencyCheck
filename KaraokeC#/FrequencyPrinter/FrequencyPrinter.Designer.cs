namespace Karaoke
{
    partial class FrequencyPrinter
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartInputWave = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartCumAve = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.buttonRec = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.isFourier = new System.Windows.Forms.RadioButton();
            this.isYIN = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listBoxDevices = new System.Windows.Forms.ListBox();
            this.buttonDeviceUpdate = new System.Windows.Forms.Button();
            this.labelFreq = new System.Windows.Forms.Label();
            this.chartFreq = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chartInputWave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartCumAve)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartFreq)).BeginInit();
            this.SuspendLayout();
            // 
            // chartInputWave
            // 
            chartArea4.Name = "ChartArea1";
            this.chartInputWave.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.chartInputWave.Legends.Add(legend4);
            this.chartInputWave.Location = new System.Drawing.Point(12, 12);
            this.chartInputWave.Name = "chartInputWave";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chartInputWave.Series.Add(series4);
            this.chartInputWave.Size = new System.Drawing.Size(500, 250);
            this.chartInputWave.TabIndex = 0;
            this.chartInputWave.TabStop = false;
            this.chartInputWave.Text = "chart1";
            // 
            // chartCumAve
            // 
            chartArea5.Name = "ChartArea1";
            this.chartCumAve.ChartAreas.Add(chartArea5);
            legend5.Name = "Legend1";
            this.chartCumAve.Legends.Add(legend5);
            this.chartCumAve.Location = new System.Drawing.Point(12, 288);
            this.chartCumAve.Name = "chartCumAve";
            series5.ChartArea = "ChartArea1";
            series5.Legend = "Legend1";
            series5.Name = "Series1";
            this.chartCumAve.Series.Add(series5);
            this.chartCumAve.Size = new System.Drawing.Size(500, 250);
            this.chartCumAve.TabIndex = 1;
            this.chartCumAve.TabStop = false;
            this.chartCumAve.Text = "chart2";
            // 
            // buttonRec
            // 
            this.buttonRec.Location = new System.Drawing.Point(65, 849);
            this.buttonRec.Name = "buttonRec";
            this.buttonRec.Size = new System.Drawing.Size(200, 81);
            this.buttonRec.TabIndex = 3;
            this.buttonRec.Text = "Start REC";
            this.buttonRec.UseVisualStyleBackColor = true;
            this.buttonRec.Click += new System.EventHandler(this.buttonRec_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(268, 849);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(200, 81);
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
            this.panel1.Location = new System.Drawing.Point(65, 716);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(403, 108);
            this.panel1.TabIndex = 7;
            // 
            // listBoxDevices
            // 
            this.listBoxDevices.FormattingEnabled = true;
            this.listBoxDevices.ItemHeight = 24;
            this.listBoxDevices.Location = new System.Drawing.Point(65, 572);
            this.listBoxDevices.Name = "listBoxDevices";
            this.listBoxDevices.Size = new System.Drawing.Size(303, 124);
            this.listBoxDevices.TabIndex = 8;
            // 
            // buttonDeviceUpdate
            // 
            this.buttonDeviceUpdate.Location = new System.Drawing.Point(381, 597);
            this.buttonDeviceUpdate.Name = "buttonDeviceUpdate";
            this.buttonDeviceUpdate.Size = new System.Drawing.Size(87, 73);
            this.buttonDeviceUpdate.TabIndex = 9;
            this.buttonDeviceUpdate.Text = "update";
            this.buttonDeviceUpdate.UseVisualStyleBackColor = true;
            this.buttonDeviceUpdate.Click += new System.EventHandler(this.buttonDeviceUpdate_Click);
            // 
            // labelFreq
            // 
            this.labelFreq.AutoSize = true;
            this.labelFreq.Location = new System.Drawing.Point(142, 961);
            this.labelFreq.Name = "labelFreq";
            this.labelFreq.Size = new System.Drawing.Size(123, 24);
            this.labelFreq.TabIndex = 10;
            this.labelFreq.Text = "Freq [Hz] : ";
            this.labelFreq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chartFreq
            // 
            chartArea6.Name = "ChartArea1";
            this.chartFreq.ChartAreas.Add(chartArea6);
            legend6.Name = "Legend1";
            this.chartFreq.Legends.Add(legend6);
            this.chartFreq.Location = new System.Drawing.Point(562, 12);
            this.chartFreq.Name = "chartFreq";
            series6.ChartArea = "ChartArea1";
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            this.chartFreq.Series.Add(series6);
            this.chartFreq.Size = new System.Drawing.Size(500, 1005);
            this.chartFreq.TabIndex = 2;
            this.chartFreq.TabStop = false;
            this.chartFreq.Text = "chart3";
            // 
            // FrequencyPrinter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1074, 1029);
            this.Controls.Add(this.labelFreq);
            this.Controls.Add(this.buttonDeviceUpdate);
            this.Controls.Add(this.listBoxDevices);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonRec);
            this.Controls.Add(this.chartFreq);
            this.Controls.Add(this.chartCumAve);
            this.Controls.Add(this.chartInputWave);
            this.MaximizeBox = false;
            this.Name = "FrequencyPrinter";
            this.Text = "FrequencyPrinter";
            ((System.ComponentModel.ISupportInitialize)(this.chartInputWave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartCumAve)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartFreq)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartInputWave;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartCumAve;
        private System.Windows.Forms.Button buttonRec;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.RadioButton isFourier;
        private System.Windows.Forms.RadioButton isYIN;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox listBoxDevices;
        private System.Windows.Forms.Button buttonDeviceUpdate;
        private System.Windows.Forms.Label labelFreq;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartFreq;
    }
}

