namespace TCHRLibCLS2XCalibPlugin
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title3 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title4 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtConnect = new System.Windows.Forms.Button();
            this.RBAsyncConn = new System.Windows.Forms.RadioButton();
            this.RBSyncConn = new System.Windows.Forms.RadioButton();
            this.TBConnInfo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.BtPluginEnable = new System.Windows.Forms.Button();
            this.RTResponse = new System.Windows.Forms.RichTextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.TBPluginCmd = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.TBCommand = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.chartLower = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartUpper = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel6 = new System.Windows.Forms.Panel();
            this.BtZoomOut = new System.Windows.Forms.Button();
            this.BtCalibFile = new System.Windows.Forms.Button();
            this.TBCalibFile = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.TBSHZ = new System.Windows.Forms.TextBox();
            this.TBSODX = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.TimerProcess = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.LSampleNo = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartLower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartUpper)).BeginInit();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtConnect);
            this.panel1.Controls.Add(this.RBAsyncConn);
            this.panel1.Controls.Add(this.RBSyncConn);
            this.panel1.Controls.Add(this.TBConnInfo);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(934, 46);
            this.panel1.TabIndex = 0;
            // 
            // BtConnect
            // 
            this.BtConnect.Location = new System.Drawing.Point(675, 4);
            this.BtConnect.Margin = new System.Windows.Forms.Padding(2);
            this.BtConnect.Name = "BtConnect";
            this.BtConnect.Size = new System.Drawing.Size(248, 37);
            this.BtConnect.TabIndex = 5;
            this.BtConnect.Text = "Connect";
            this.BtConnect.UseVisualStyleBackColor = true;
            this.BtConnect.Click += new System.EventHandler(this.BtConnect_Click);
            // 
            // RBAsyncConn
            // 
            this.RBAsyncConn.AutoSize = true;
            this.RBAsyncConn.Location = new System.Drawing.Point(395, 16);
            this.RBAsyncConn.Margin = new System.Windows.Forms.Padding(2);
            this.RBAsyncConn.Name = "RBAsyncConn";
            this.RBAsyncConn.Size = new System.Drawing.Size(149, 17);
            this.RBAsyncConn.TabIndex = 4;
            this.RBAsyncConn.Text = "Asynchronous Connection";
            this.RBAsyncConn.UseVisualStyleBackColor = true;
            // 
            // RBSyncConn
            // 
            this.RBSyncConn.AutoSize = true;
            this.RBSyncConn.Checked = true;
            this.RBSyncConn.Location = new System.Drawing.Point(247, 16);
            this.RBSyncConn.Margin = new System.Windows.Forms.Padding(2);
            this.RBSyncConn.Name = "RBSyncConn";
            this.RBSyncConn.Size = new System.Drawing.Size(144, 17);
            this.RBSyncConn.TabIndex = 3;
            this.RBSyncConn.TabStop = true;
            this.RBSyncConn.Text = "Synchronous Connection";
            this.RBSyncConn.UseVisualStyleBackColor = true;
            // 
            // TBConnInfo
            // 
            this.TBConnInfo.Location = new System.Drawing.Point(91, 13);
            this.TBConnInfo.Margin = new System.Windows.Forms.Padding(2);
            this.TBConnInfo.Name = "TBConnInfo";
            this.TBConnInfo.Size = new System.Drawing.Size(137, 20);
            this.TBConnInfo.TabIndex = 1;
            this.TBConnInfo.Text = "192.168.170.3";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Connection Info:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.BtPluginEnable);
            this.panel2.Controls.Add(this.RTResponse);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(803, 46);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(131, 529);
            this.panel2.TabIndex = 1;
            // 
            // BtPluginEnable
            // 
            this.BtPluginEnable.Enabled = false;
            this.BtPluginEnable.Location = new System.Drawing.Point(11, 482);
            this.BtPluginEnable.Name = "BtPluginEnable";
            this.BtPluginEnable.Size = new System.Drawing.Size(115, 40);
            this.BtPluginEnable.TabIndex = 3;
            this.BtPluginEnable.Text = "Disable Correction";
            this.BtPluginEnable.UseVisualStyleBackColor = true;
            this.BtPluginEnable.Click += new System.EventHandler(this.BtPluginEnable_Click);
            // 
            // RTResponse
            // 
            this.RTResponse.Location = new System.Drawing.Point(-1, 107);
            this.RTResponse.Margin = new System.Windows.Forms.Padding(2);
            this.RTResponse.Name = "RTResponse";
            this.RTResponse.Size = new System.Drawing.Size(132, 370);
            this.RTResponse.TabIndex = 2;
            this.RTResponse.Text = "";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label3);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 90);
            this.panel5.Margin = new System.Windows.Forms.Padding(2);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(131, 18);
            this.panel5.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 2);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Response:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.TBPluginCmd);
            this.panel4.Controls.Add(this.label8);
            this.panel4.Controls.Add(this.TBCommand);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(131, 90);
            this.panel4.TabIndex = 0;
            // 
            // TBPluginCmd
            // 
            this.TBPluginCmd.Location = new System.Drawing.Point(1, 66);
            this.TBPluginCmd.Margin = new System.Windows.Forms.Padding(2);
            this.TBPluginCmd.Name = "TBPluginCmd";
            this.TBPluginCmd.Size = new System.Drawing.Size(128, 20);
            this.TBPluginCmd.TabIndex = 3;
            this.TBPluginCmd.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TBPluginCmd_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 49);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Plugin Command:";
            // 
            // TBCommand
            // 
            this.TBCommand.Location = new System.Drawing.Point(2, 19);
            this.TBCommand.Margin = new System.Windows.Forms.Padding(2);
            this.TBCommand.Name = "TBCommand";
            this.TBCommand.Size = new System.Drawing.Size(128, 20);
            this.TBCommand.TabIndex = 1;
            this.TBCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TBCommand_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 2);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Device Command:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.splitter1);
            this.panel3.Controls.Add(this.chartLower);
            this.panel3.Controls.Add(this.chartUpper);
            this.panel3.Controls.Add(this.panel6);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 46);
            this.panel3.Margin = new System.Windows.Forms.Padding(2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(803, 529);
            this.panel3.TabIndex = 2;
            // 
            // splitter1
            // 
            this.splitter1.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 252);
            this.splitter1.Margin = new System.Windows.Forms.Padding(2);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(803, 2);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // chartLower
            // 
            chartArea3.AxisX.IsLabelAutoFit = false;
            chartArea3.AxisX.LabelStyle.Font = new System.Drawing.Font("Modern No. 20", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea3.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea3.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea3.AxisX.MajorTickMark.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea3.AxisX.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea3.AxisX.MinorTickMark.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea3.AxisX.Title = "Channel";
            chartArea3.AxisY.IsLabelAutoFit = false;
            chartArea3.AxisY.IsStartedFromZero = false;
            chartArea3.AxisY.LabelStyle.Font = new System.Drawing.Font("Modern No. 20", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea3.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea3.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea3.AxisY.MajorTickMark.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea3.AxisY.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea3.AxisY.MinorTickMark.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea3.Name = "ChartArea1";
            this.chartLower.ChartAreas.Add(chartArea3);
            this.chartLower.Dock = System.Windows.Forms.DockStyle.Fill;
            legend3.Enabled = false;
            legend3.Name = "Legend1";
            this.chartLower.Legends.Add(legend3);
            this.chartLower.Location = new System.Drawing.Point(0, 252);
            this.chartLower.Margin = new System.Windows.Forms.Padding(1);
            this.chartLower.Name = "chartLower";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series3.IsVisibleInLegend = false;
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chartLower.Series.Add(series3);
            this.chartLower.Size = new System.Drawing.Size(803, 277);
            this.chartLower.TabIndex = 2;
            this.chartLower.Text = "chart1";
            title3.Name = "Title1";
            title3.Text = "First Peak Signal";
            this.chartLower.Titles.Add(title3);
            this.chartLower.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chartLower_MouseDown);
            this.chartLower.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chartLower_MouseUp);
            // 
            // chartUpper
            // 
            chartArea4.AxisX.IsLabelAutoFit = false;
            chartArea4.AxisX.LabelStyle.Font = new System.Drawing.Font("Modern No. 20", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea4.AxisX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea4.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea4.AxisX.MajorTickMark.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea4.AxisX.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea4.AxisX.MinorTickMark.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea4.AxisX.Title = "Sample Index";
            chartArea4.AxisY.IsLabelAutoFit = false;
            chartArea4.AxisY.IsStartedFromZero = false;
            chartArea4.AxisY.LabelStyle.Font = new System.Drawing.Font("Modern No. 20", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea4.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea4.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea4.AxisY.MajorTickMark.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea4.AxisY.MinorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea4.AxisY.MinorTickMark.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea4.Name = "ChartArea1";
            this.chartUpper.ChartAreas.Add(chartArea4);
            this.chartUpper.Dock = System.Windows.Forms.DockStyle.Top;
            legend4.Enabled = false;
            legend4.Name = "Legend1";
            this.chartUpper.Legends.Add(legend4);
            this.chartUpper.Location = new System.Drawing.Point(0, 50);
            this.chartUpper.Margin = new System.Windows.Forms.Padding(1);
            this.chartUpper.Name = "chartUpper";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series4.IsVisibleInLegend = false;
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chartUpper.Series.Add(series4);
            this.chartUpper.Size = new System.Drawing.Size(803, 202);
            this.chartUpper.TabIndex = 1;
            this.chartUpper.Text = "chart2";
            title4.Name = "Title1";
            title4.Text = "First Global Signal";
            this.chartUpper.Titles.Add(title4);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.LSampleNo);
            this.panel6.Controls.Add(this.BtZoomOut);
            this.panel6.Controls.Add(this.BtCalibFile);
            this.panel6.Controls.Add(this.TBCalibFile);
            this.panel6.Controls.Add(this.label6);
            this.panel6.Controls.Add(this.TBSHZ);
            this.panel6.Controls.Add(this.TBSODX);
            this.panel6.Controls.Add(this.label4);
            this.panel6.Controls.Add(this.label7);
            this.panel6.Controls.Add(this.label5);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(2);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(803, 50);
            this.panel6.TabIndex = 0;
            // 
            // BtZoomOut
            // 
            this.BtZoomOut.Location = new System.Drawing.Point(714, 27);
            this.BtZoomOut.Name = "BtZoomOut";
            this.BtZoomOut.Size = new System.Drawing.Size(86, 21);
            this.BtZoomOut.TabIndex = 11;
            this.BtZoomOut.Text = "Zoom Out";
            this.BtZoomOut.UseVisualStyleBackColor = true;
            this.BtZoomOut.Click += new System.EventHandler(this.BtZoomOut_Click);
            // 
            // BtCalibFile
            // 
            this.BtCalibFile.Enabled = false;
            this.BtCalibFile.Location = new System.Drawing.Point(714, 3);
            this.BtCalibFile.Name = "BtCalibFile";
            this.BtCalibFile.Size = new System.Drawing.Size(75, 23);
            this.BtCalibFile.TabIndex = 10;
            this.BtCalibFile.Text = "Browse";
            this.BtCalibFile.UseVisualStyleBackColor = true;
            this.BtCalibFile.Click += new System.EventHandler(this.BtCalibFile_Click);
            // 
            // TBCalibFile
            // 
            this.TBCalibFile.Location = new System.Drawing.Point(482, 3);
            this.TBCalibFile.Margin = new System.Windows.Forms.Padding(2);
            this.TBCalibFile.Name = "TBCalibFile";
            this.TBCalibFile.Size = new System.Drawing.Size(215, 20);
            this.TBCalibFile.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(424, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Calib. File";
            // 
            // TBSHZ
            // 
            this.TBSHZ.Location = new System.Drawing.Point(305, 2);
            this.TBSHZ.Margin = new System.Windows.Forms.Padding(2);
            this.TBSHZ.Name = "TBSHZ";
            this.TBSHZ.Size = new System.Drawing.Size(101, 20);
            this.TBSHZ.TabIndex = 7;
            this.TBSHZ.Text = "1000";
            this.TBSHZ.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TBSHZ_KeyPress);
            // 
            // TBSODX
            // 
            this.TBSODX.Location = new System.Drawing.Point(76, 2);
            this.TBSODX.Margin = new System.Windows.Forms.Padding(2);
            this.TBSODX.Name = "TBSODX";
            this.TBSODX.Size = new System.Drawing.Size(149, 20);
            this.TBSODX.TabIndex = 6;
            this.TBSODX.Text = "83, 16641, 16640";
            this.TBSODX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TBSODX_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(228, 2);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Sample Rate: ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 31);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(152, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Received Sample per Second:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 2);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Output Signal: ";
            // 
            // TimerProcess
            // 
            this.TimerProcess.Interval = 50;
            this.TimerProcess.Tick += new System.EventHandler(this.TimerProcess_Tick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Title = "Choose Calibration File";
            // 
            // LSampleNo
            // 
            this.LSampleNo.AutoSize = true;
            this.LSampleNo.Location = new System.Drawing.Point(161, 31);
            this.LSampleNo.Name = "LSampleNo";
            this.LSampleNo.Size = new System.Drawing.Size(0, 13);
            this.LSampleNo.TabIndex = 12;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 575);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartLower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartUpper)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BtConnect;
        private System.Windows.Forms.RadioButton RBAsyncConn;
        private System.Windows.Forms.RadioButton RBSyncConn;
        private System.Windows.Forms.TextBox TBConnInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RichTextBox RTResponse;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox TBCommand;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartLower;
        private System.Windows.Forms.TextBox TBSHZ;
        private System.Windows.Forms.TextBox TBSODX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer TimerProcess;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Button BtPluginEnable;
        private System.Windows.Forms.Button BtCalibFile;
        private System.Windows.Forms.TextBox TBCalibFile;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox TBPluginCmd;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button BtZoomOut;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartUpper;
        private System.Windows.Forms.Label LSampleNo;
    }
}

