namespace CSharpAwaitProcess2
{
    partial class AsyncProcessDemo2
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.CBSensorAddress = new System.Windows.Forms.ComboBox();
            this.BDiscover = new System.Windows.Forms.Button();
            this.BConnect = new System.Windows.Forms.Button();
            this.ChartSpectrum = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.TimerDeviceSearch = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.TimerRefreshSpectrumChart = new System.Windows.Forms.Timer(this.components);
            this.BDarkCorrection = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ChartSpectrum)).BeginInit();
            this.SuspendLayout();
            // 
            // CBSensorAddress
            // 
            this.CBSensorAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CBSensorAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBSensorAddress.FormattingEnabled = true;
            this.CBSensorAddress.Location = new System.Drawing.Point(13, 11);
            this.CBSensorAddress.Margin = new System.Windows.Forms.Padding(4);
            this.CBSensorAddress.Name = "CBSensorAddress";
            this.CBSensorAddress.Size = new System.Drawing.Size(341, 32);
            this.CBSensorAddress.TabIndex = 0;
            this.toolTip.SetToolTip(this.CBSensorAddress, "Enter sensor IP address or click \"Discover\"\r\nto populate with discovered sensors." +
        "");
            // 
            // BDiscover
            // 
            this.BDiscover.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BDiscover.Location = new System.Drawing.Point(362, 10);
            this.BDiscover.Margin = new System.Windows.Forms.Padding(4);
            this.BDiscover.Name = "BDiscover";
            this.BDiscover.Size = new System.Drawing.Size(126, 39);
            this.BDiscover.TabIndex = 1;
            this.BDiscover.Text = "Discover";
            this.toolTip.SetToolTip(this.BDiscover, "Click to start sensor discovery.\r\nDiscovered sensors will be filled into\r\ncombo b" +
        "ox on the left.");
            this.BDiscover.UseVisualStyleBackColor = true;
            this.BDiscover.Click += new System.EventHandler(this.BDiscover_Click);
            // 
            // BConnect
            // 
            this.BConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BConnect.Enabled = false;
            this.BConnect.Location = new System.Drawing.Point(496, 10);
            this.BConnect.Margin = new System.Windows.Forms.Padding(4);
            this.BConnect.Name = "BConnect";
            this.BConnect.Size = new System.Drawing.Size(112, 39);
            this.BConnect.TabIndex = 2;
            this.BConnect.Text = "Connect";
            this.BConnect.UseVisualStyleBackColor = true;
            this.BConnect.Click += new System.EventHandler(this.BConnect_Click);
            // 
            // ChartSpectrum
            // 
            this.ChartSpectrum.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.ChartSpectrum.ChartAreas.Add(chartArea1);
            this.ChartSpectrum.Location = new System.Drawing.Point(13, 59);
            this.ChartSpectrum.Margin = new System.Windows.Forms.Padding(6);
            this.ChartSpectrum.Name = "ChartSpectrum";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Name = "SpectrumSeries";
            this.ChartSpectrum.Series.Add(series1);
            this.ChartSpectrum.Size = new System.Drawing.Size(717, 637);
            this.ChartSpectrum.TabIndex = 3;
            // 
            // TimerDeviceSearch
            // 
            this.TimerDeviceSearch.Interval = 300;
            this.TimerDeviceSearch.Tick += new System.EventHandler(this.TimerDeviceSearch_Tick);
            // 
            // TimerRefreshSpectrumChart
            // 
            this.TimerRefreshSpectrumChart.Interval = 50;
            this.TimerRefreshSpectrumChart.Tick += new System.EventHandler(this.TimerRefreshSpectrumChart_Tick);
            // 
            // BDarkCorrection
            // 
            this.BDarkCorrection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BDarkCorrection.Enabled = false;
            this.BDarkCorrection.Location = new System.Drawing.Point(616, 10);
            this.BDarkCorrection.Margin = new System.Windows.Forms.Padding(4);
            this.BDarkCorrection.Name = "BDarkCorrection";
            this.BDarkCorrection.Size = new System.Drawing.Size(112, 39);
            this.BDarkCorrection.TabIndex = 4;
            this.BDarkCorrection.Text = "Dark Corr.";
            this.BDarkCorrection.UseVisualStyleBackColor = true;
            this.BDarkCorrection.Click += new System.EventHandler(this.BDarkCorrection_ClickAsync);
            // 
            // AsyncProcessDemo2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 702);
            this.Controls.Add(this.BDarkCorrection);
            this.Controls.Add(this.ChartSpectrum);
            this.Controls.Add(this.BConnect);
            this.Controls.Add(this.BDiscover);
            this.Controls.Add(this.CBSensorAddress);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AsyncProcessDemo2";
            this.Text = "Async Demo 2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AsyncProcessDemo2_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.ChartSpectrum)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox CBSensorAddress;
        private System.Windows.Forms.Button BDiscover;
        private System.Windows.Forms.Button BConnect;
        private System.Windows.Forms.DataVisualization.Charting.Chart ChartSpectrum;
        private System.Windows.Forms.Timer TimerDeviceSearch;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer TimerRefreshSpectrumChart;
        private System.Windows.Forms.Button BDarkCorrection;
    }
}

