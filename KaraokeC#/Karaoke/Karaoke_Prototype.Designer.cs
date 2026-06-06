namespace Karaoke
{
    partial class Karaoke_Prototype
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonDeviceUpdate = new System.Windows.Forms.Button();
            this.energyUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxDeviceOut = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxDeviceIn = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.fftSizeUpDown = new System.Windows.Forms.DomainUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.samprate = new System.Windows.Forms.DomainUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.mps = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.isHPS = new System.Windows.Forms.RadioButton();
            this.isYIN = new System.Windows.Forms.RadioButton();
            this.isFourier = new System.Windows.Forms.RadioButton();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.buttonMusicUpdate = new System.Windows.Forms.Button();
            this.buttonDeSelect = new System.Windows.Forms.Button();
            this.listBoxMusic = new System.Windows.Forms.ListBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.labelFreq = new System.Windows.Forms.Label();
            this.OctaveUpDown = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonPageForward = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonPageBack = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.labelMusicLength = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.trackBarMusic = new System.Windows.Forms.TrackBar();
            this.labelCurrentTime = new System.Windows.Forms.Label();
            this.latencyUpDown = new System.Windows.Forms.NumericUpDown();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.chartInputWave = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chartPitch = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lyricsBox = new System.Windows.Forms.RichTextBox();
            this.timerUI = new System.Windows.Forms.Timer(this.components);
            this.timerPlay = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.energyUpDown)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mps)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OctaveUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMusic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.latencyUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartInputWave)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPitch)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chartInputWave);
            this.splitContainer1.Size = new System.Drawing.Size(1144, 275);
            this.splitContainer1.SplitterDistance = 750;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(744, 269);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.buttonDeviceUpdate);
            this.tabPage1.Controls.Add(this.energyUpDown);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.comboBoxDeviceOut);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.comboBoxDeviceIn);
            this.tabPage1.Location = new System.Drawing.Point(8, 39);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(728, 222);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Audio";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // buttonDeviceUpdate
            // 
            this.buttonDeviceUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDeviceUpdate.Location = new System.Drawing.Point(525, 159);
            this.buttonDeviceUpdate.Name = "buttonDeviceUpdate";
            this.buttonDeviceUpdate.Size = new System.Drawing.Size(197, 57);
            this.buttonDeviceUpdate.TabIndex = 35;
            this.buttonDeviceUpdate.Text = "Update Devices";
            this.buttonDeviceUpdate.UseVisualStyleBackColor = true;
            this.buttonDeviceUpdate.Click += new System.EventHandler(this.buttonDeviceUpdate_Click);
            // 
            // energyUpDown
            // 
            this.energyUpDown.DecimalPlaces = 8;
            this.energyUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.energyUpDown.Location = new System.Drawing.Point(52, 186);
            this.energyUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.energyUpDown.Name = "energyUpDown";
            this.energyUpDown.Size = new System.Drawing.Size(320, 31);
            this.energyUpDown.TabIndex = 9;
            this.energyUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 159);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(366, 24);
            this.label3.TabIndex = 34;
            this.label3.Text = "Noise Gate (Mic Energy under-limit)";
            // 
            // comboBoxDeviceOut
            // 
            this.comboBoxDeviceOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDeviceOut.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDeviceOut.FormattingEnabled = true;
            this.comboBoxDeviceOut.Location = new System.Drawing.Point(52, 114);
            this.comboBoxDeviceOut.Name = "comboBoxDeviceOut";
            this.comboBoxDeviceOut.Size = new System.Drawing.Size(670, 32);
            this.comboBoxDeviceOut.TabIndex = 33;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(255, 24);
            this.label2.TabIndex = 32;
            this.label2.Text = "Output Device (Speaker)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 24);
            this.label1.TabIndex = 31;
            this.label1.Text = "Input Device (Mic)";
            // 
            // comboBoxDeviceIn
            // 
            this.comboBoxDeviceIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDeviceIn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDeviceIn.FormattingEnabled = true;
            this.comboBoxDeviceIn.Location = new System.Drawing.Point(52, 40);
            this.comboBoxDeviceIn.Name = "comboBoxDeviceIn";
            this.comboBoxDeviceIn.Size = new System.Drawing.Size(670, 32);
            this.comboBoxDeviceIn.TabIndex = 30;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.fftSizeUpDown);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.samprate);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.mps);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Location = new System.Drawing.Point(8, 39);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(728, 222);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Analysis";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // fftSizeUpDown
            // 
            this.fftSizeUpDown.Location = new System.Drawing.Point(318, 181);
            this.fftSizeUpDown.Name = "fftSizeUpDown";
            this.fftSizeUpDown.ReadOnly = true;
            this.fftSizeUpDown.Size = new System.Drawing.Size(164, 31);
            this.fftSizeUpDown.TabIndex = 19;
            this.fftSizeUpDown.Text = "16384";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(151, 24);
            this.label7.TabIndex = 32;
            this.label7.Text = "Sampling Rate";
            // 
            // samprate
            // 
            this.samprate.Items.Add("32768");
            this.samprate.Items.Add("22100");
            this.samprate.Items.Add("16384");
            this.samprate.Items.Add("11052");
            this.samprate.Items.Add("8192");
            this.samprate.Location = new System.Drawing.Point(52, 40);
            this.samprate.Name = "samprate";
            this.samprate.ReadOnly = true;
            this.samprate.Size = new System.Drawing.Size(184, 31);
            this.samprate.TabIndex = 19;
            this.samprate.Text = "16384";
            this.samprate.SelectedItemChanged += new System.EventHandler(this.samprate_SelectedItemChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 24);
            this.label4.TabIndex = 18;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(286, 154);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(152, 24);
            this.label6.TabIndex = 20;
            this.label6.Text = "FFT point size";
            // 
            // mps
            // 
            this.mps.Location = new System.Drawing.Point(52, 114);
            this.mps.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mps.Name = "mps";
            this.mps.Size = new System.Drawing.Size(184, 31);
            this.mps.TabIndex = 17;
            this.mps.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 87);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(250, 24);
            this.label5.TabIndex = 16;
            this.label5.Text = "Measure times (per sec)";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.isHPS);
            this.panel1.Controls.Add(this.isYIN);
            this.panel1.Controls.Add(this.isFourier);
            this.panel1.Location = new System.Drawing.Point(284, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(438, 132);
            this.panel1.TabIndex = 15;
            // 
            // isHPS
            // 
            this.isHPS.AutoSize = true;
            this.isHPS.Location = new System.Drawing.Point(6, 49);
            this.isHPS.Name = "isHPS";
            this.isHPS.Size = new System.Drawing.Size(180, 28);
            this.isHPS.TabIndex = 7;
            this.isHPS.TabStop = true;
            this.isHPS.Text = "FFT with HPS";
            this.isHPS.UseVisualStyleBackColor = true;
            // 
            // isYIN
            // 
            this.isYIN.AutoSize = true;
            this.isYIN.Location = new System.Drawing.Point(265, 4);
            this.isYIN.Name = "isYIN";
            this.isYIN.Size = new System.Drawing.Size(76, 28);
            this.isYIN.TabIndex = 6;
            this.isYIN.Text = "YIN";
            this.isYIN.UseVisualStyleBackColor = true;
            // 
            // isFourier
            // 
            this.isFourier.AutoSize = true;
            this.isFourier.Checked = true;
            this.isFourier.Location = new System.Drawing.Point(6, 4);
            this.isFourier.Name = "isFourier";
            this.isFourier.Size = new System.Drawing.Size(128, 28);
            this.isFourier.TabIndex = 5;
            this.isFourier.TabStop = true;
            this.isFourier.Text = "FFT only";
            this.isFourier.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.buttonMusicUpdate);
            this.tabPage4.Controls.Add(this.buttonDeSelect);
            this.tabPage4.Controls.Add(this.listBoxMusic);
            this.tabPage4.Controls.Add(this.label12);
            this.tabPage4.Location = new System.Drawing.Point(8, 39);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(728, 222);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Music";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // buttonMusicUpdate
            // 
            this.buttonMusicUpdate.Location = new System.Drawing.Point(345, 3);
            this.buttonMusicUpdate.Name = "buttonMusicUpdate";
            this.buttonMusicUpdate.Size = new System.Drawing.Size(131, 44);
            this.buttonMusicUpdate.TabIndex = 22;
            this.buttonMusicUpdate.Text = "Update";
            this.buttonMusicUpdate.UseVisualStyleBackColor = true;
            this.buttonMusicUpdate.Click += new System.EventHandler(this.buttonMusicUpdate_Click);
            // 
            // buttonDeSelect
            // 
            this.buttonDeSelect.Location = new System.Drawing.Point(183, 3);
            this.buttonDeSelect.Name = "buttonDeSelect";
            this.buttonDeSelect.Size = new System.Drawing.Size(131, 44);
            this.buttonDeSelect.TabIndex = 21;
            this.buttonDeSelect.Text = "Reset";
            this.buttonDeSelect.UseVisualStyleBackColor = true;
            this.buttonDeSelect.Click += new System.EventHandler(this.buttonDeSelect_Click);
            // 
            // listBoxMusic
            // 
            this.listBoxMusic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxMusic.FormattingEnabled = true;
            this.listBoxMusic.ItemHeight = 24;
            this.listBoxMusic.Location = new System.Drawing.Point(10, 47);
            this.listBoxMusic.Name = "listBoxMusic";
            this.listBoxMusic.Size = new System.Drawing.Size(715, 172);
            this.listBoxMusic.TabIndex = 16;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 13);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(68, 24);
            this.label12.TabIndex = 0;
            this.label12.Text = "Music";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.labelFreq);
            this.tabPage3.Controls.Add(this.OctaveUpDown);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.buttonPageForward);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.buttonPageBack);
            this.tabPage3.Controls.Add(this.buttonPlay);
            this.tabPage3.Controls.Add(this.labelMusicLength);
            this.tabPage3.Controls.Add(this.buttonStop);
            this.tabPage3.Controls.Add(this.trackBarMusic);
            this.tabPage3.Controls.Add(this.labelCurrentTime);
            this.tabPage3.Controls.Add(this.latencyUpDown);
            this.tabPage3.Controls.Add(this.trackBarVolume);
            this.tabPage3.Location = new System.Drawing.Point(8, 39);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(728, 222);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Playback";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(236, 79);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 24);
            this.label11.TabIndex = 39;
            this.label11.Text = "Page";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(436, 132);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(84, 24);
            this.label10.TabIndex = 38;
            this.label10.Text = "Volume";
            // 
            // labelFreq
            // 
            this.labelFreq.AutoSize = true;
            this.labelFreq.Location = new System.Drawing.Point(660, 194);
            this.labelFreq.Name = "labelFreq";
            this.labelFreq.Size = new System.Drawing.Size(55, 24);
            this.labelFreq.TabIndex = 37;
            this.labelFreq.Text = "Freq";
            this.labelFreq.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // OctaveUpDown
            // 
            this.OctaveUpDown.Location = new System.Drawing.Point(556, 74);
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
            this.OctaveUpDown.Size = new System.Drawing.Size(159, 31);
            this.OctaveUpDown.TabIndex = 35;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(436, 76);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 24);
            this.label9.TabIndex = 36;
            this.label9.Text = "Octave";
            // 
            // buttonPageForward
            // 
            this.buttonPageForward.Location = new System.Drawing.Point(265, 106);
            this.buttonPageForward.Name = "buttonPageForward";
            this.buttonPageForward.Size = new System.Drawing.Size(50, 50);
            this.buttonPageForward.TabIndex = 34;
            this.buttonPageForward.Text = ">>";
            this.buttonPageForward.UseVisualStyleBackColor = true;
            this.buttonPageForward.Click += new System.EventHandler(this.buttonPageForward_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(436, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 24);
            this.label8.TabIndex = 33;
            this.label8.Text = "Latency";
            // 
            // buttonPageBack
            // 
            this.buttonPageBack.Location = new System.Drawing.Point(209, 106);
            this.buttonPageBack.Name = "buttonPageBack";
            this.buttonPageBack.Size = new System.Drawing.Size(50, 50);
            this.buttonPageBack.TabIndex = 33;
            this.buttonPageBack.Text = "<<";
            this.buttonPageBack.UseVisualStyleBackColor = true;
            this.buttonPageBack.Click += new System.EventHandler(this.buttonPageBack_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Location = new System.Drawing.Point(3, 3);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(155, 67);
            this.buttonPlay.TabIndex = 28;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // labelMusicLength
            // 
            this.labelMusicLength.AutoSize = true;
            this.labelMusicLength.Location = new System.Drawing.Point(346, 181);
            this.labelMusicLength.Name = "labelMusicLength";
            this.labelMusicLength.Size = new System.Drawing.Size(63, 24);
            this.labelMusicLength.TabIndex = 31;
            this.labelMusicLength.Text = "00:00";
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(3, 89);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(155, 67);
            this.buttonStop.TabIndex = 29;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // trackBarMusic
            // 
            this.trackBarMusic.Location = new System.Drawing.Point(84, 171);
            this.trackBarMusic.Name = "trackBarMusic";
            this.trackBarMusic.Size = new System.Drawing.Size(256, 90);
            this.trackBarMusic.TabIndex = 30;
            // 
            // labelCurrentTime
            // 
            this.labelCurrentTime.AutoSize = true;
            this.labelCurrentTime.Location = new System.Drawing.Point(15, 181);
            this.labelCurrentTime.Name = "labelCurrentTime";
            this.labelCurrentTime.Size = new System.Drawing.Size(63, 24);
            this.labelCurrentTime.TabIndex = 32;
            this.labelCurrentTime.Text = "00:00";
            // 
            // latencyUpDown
            // 
            this.latencyUpDown.DecimalPlaces = 3;
            this.latencyUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.latencyUpDown.Location = new System.Drawing.Point(556, 14);
            this.latencyUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.latencyUpDown.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            -2147483648});
            this.latencyUpDown.Name = "latencyUpDown";
            this.latencyUpDown.Size = new System.Drawing.Size(159, 31);
            this.latencyUpDown.TabIndex = 27;
            this.latencyUpDown.ValueChanged += new System.EventHandler(this.latencyUpDown_ValueChanged);
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.LargeChange = 10;
            this.trackBarVolume.Location = new System.Drawing.Point(556, 128);
            this.trackBarVolume.Maximum = 100;
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(159, 90);
            this.trackBarVolume.TabIndex = 26;
            this.trackBarVolume.TickFrequency = 10;
            this.trackBarVolume.Value = 50;
            this.trackBarVolume.ValueChanged += new System.EventHandler(this.trackBarVolume_ValueChanged);
            // 
            // chartInputWave
            // 
            chartArea3.Name = "ChartArea1";
            this.chartInputWave.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chartInputWave.Legends.Add(legend3);
            this.chartInputWave.Location = new System.Drawing.Point(3, 3);
            this.chartInputWave.Name = "chartInputWave";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chartInputWave.Series.Add(series3);
            this.chartInputWave.Size = new System.Drawing.Size(384, 269);
            this.chartInputWave.TabIndex = 1;
            this.chartInputWave.TabStop = false;
            this.chartInputWave.Text = "chart1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.chartPitch, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lyricsBox, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1150, 805);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // chartPitch
            // 
            chartArea4.Name = "ChartArea1";
            this.chartPitch.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.chartPitch.Legends.Add(legend4);
            this.chartPitch.Location = new System.Drawing.Point(3, 445);
            this.chartPitch.Name = "chartPitch";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chartPitch.Series.Add(series4);
            this.chartPitch.Size = new System.Drawing.Size(1144, 357);
            this.chartPitch.TabIndex = 32;
            this.chartPitch.TabStop = false;
            this.chartPitch.Text = "chart3";
            // 
            // lyricsBox
            // 
            this.lyricsBox.Font = new System.Drawing.Font("MS UI Gothic", 10.85F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lyricsBox.Location = new System.Drawing.Point(3, 284);
            this.lyricsBox.Name = "lyricsBox";
            this.lyricsBox.ReadOnly = true;
            this.lyricsBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.lyricsBox.Size = new System.Drawing.Size(1144, 155);
            this.lyricsBox.TabIndex = 31;
            this.lyricsBox.Text = "歌詞";
            // 
            // timerUI
            // 
            this.timerUI.Tick += new System.EventHandler(this.timerUI_Tick);
            // 
            // timerPlay
            // 
            this.timerPlay.Tick += new System.EventHandler(this.timerPlay_Tick);
            // 
            // Karaoke_Prototype
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1174, 829);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Karaoke_Prototype";
            this.Text = "Karaoke_Prototype";
            this.Shown += new System.EventHandler(this.Karaoke_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Karaoke_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.energyUpDown)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mps)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OctaveUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMusic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.latencyUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartInputWave)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartPitch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.NumericUpDown energyUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxDeviceOut;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxDeviceIn;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartInputWave;
        private System.Windows.Forms.DomainUpDown samprate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown mps;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton isYIN;
        private System.Windows.Forms.RadioButton isFourier;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DomainUpDown fftSizeUpDown;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton isHPS;
        private System.Windows.Forms.Button buttonDeviceUpdate;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button buttonPageForward;
        private System.Windows.Forms.Button buttonPageBack;
        private System.Windows.Forms.Label labelCurrentTime;
        private System.Windows.Forms.Label labelMusicLength;
        private System.Windows.Forms.TrackBar trackBarMusic;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.RichTextBox lyricsBox;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPitch;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown latencyUpDown;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.NumericUpDown OctaveUpDown;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label labelFreq;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ListBox listBoxMusic;
        private System.Windows.Forms.Button buttonMusicUpdate;
        private System.Windows.Forms.Button buttonDeSelect;
        private System.Windows.Forms.Timer timerUI;
        private System.Windows.Forms.Timer timerPlay;
    }
}