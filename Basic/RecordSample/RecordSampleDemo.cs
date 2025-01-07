/*
This demo shows how to use the recording mode of the synchronous connection to collect data
When the connection is in the recording modes, all the samples will be saved in a buffer. This buffer can be retrieved upon the calling of "StopRecording".
During recording, no commands can be executed. "GetNextSamples" will return newly recorded data since the last call of "GetNextSamples".
*/


using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using CHRocodileLib;
using System.Drawing;
using System.Collections.Generic;
using DATABUILDERAXLibLB;


namespace TCHRLibBasicRecordSample
{
    public partial class TRecordSample : Form
    {
        CHRocodileLib.SynchronousConnection Conn;

        //record 1000 samples
        int SampleCount;

        MeasurementMode MeasuringMethod = MeasurementMode.Confocal;
        int[] SignalIDs;
        float ScanRate;

        int CurrentDataPos;
        CHRocodileLib.Data RecordData = null;


        public TRecordSample()
        {
            InitializeComponent();
            axDBTriggerManager1.Triggers.FindByName("TriggerRC").Fire += RunScan_Fire;


            axDBTriggerManager1.Triggers.FindByName("TriggerStartRC").Fire += StartRecordingLine_Fire;
            axDBTriggerManager1.Triggers.FindByName("TriggerStopRC").Fire += StopRecordingLine_Fire;
            //for (int i = 0; i < sampleData.Length; i++)
            //{
            //    sampleData[i] = double.NaN;
            //}
        }



        private void BtConnect_Click(object sender, EventArgs e)
        {
            bool bConnect = false;

            //connect to device
            if (sender == BtConnect)
            {
                try
                {
                    axDBCommManager1.Connect();
                    axDBTriggerManager1.Active = true;
                    var DeviceType = CHRocodileLib.DeviceType.Chr1;
                    if (RBCHR2.Checked)
                        DeviceType = CHRocodileLib.DeviceType.Chr2;
                    else if (RBCLS.Checked)
                        DeviceType = CHRocodileLib.DeviceType.MultiChannel;
                    else if (RBCHRC.Checked)
                        DeviceType = CHRocodileLib.DeviceType.ChrCMini;
                    string strConInfo = TbConInfo.Text;
                    Conn = new CHRocodileLib.SynchronousConnection(strConInfo, DeviceType);
                    //set up device
                    SetupDevice();

                    bConnect = true;
                    labelRecordingHint.Visible = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //close connection to device
            else
            {
                StopRecording();
                Conn.Close();
                Conn = null;
            }
            EnableGui(bConnect);

        }



        private void axDBDeviceManager1_AfterRead(object sender, EventArgs e)
        {
            //dm100 = axDBDeviceManager1.Devices[3].ValueRead;

        }

        private void axDBDeviceManager1_BeforeRead(object sender, EventArgs e)
        {
            try
            {
                if (axDBCommManager1.ReadDevice(DBPlcDevice.DKVNano_MR, "105") != 0)
                {
                    isRunCW = true;
                }
                else
                {
                    isRunCW = false;
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Read Data Failed", "Error");
            }
        }

        private void SetupDevice()
        {
            //default signals are: Sample counter, peak 1 value, peak 1 quality/intensity
            //signal definition for CLS device, only 16bit integer signal for peak signal
            if (RBCLS.Checked)
                SignalIDs = new int[] { 83, 16640, 16641 };
            //other devices, float values are ordered
            else
                SignalIDs = new int[] { 83, 256, 257 };
            //Update TextBox
            TBSODX.Text = String.Join(",", SignalIDs.Select(p => p.ToString()).ToArray());
            ScanRate = 4000;
            //CLS device, normally maximum scan rate ist 2000
            //ScanRate = 2000;
            TBSHZ.Text = ScanRate.ToString();
            if (!RBCLS.Checked && !RBCHRC.Checked)
            {
                //set up measuring method (confocal or interferometric)
                SetUpMeasuringMethod();
            }
            SetUpMeasuringMethod();
            SetUpScanrate();
            SetUpOutputSignals();
        }

        private void SetUpMeasuringMethod()
        {
            try
            {
                MeasurementMode nMMD = MeasurementMode.Confocal;
                if (RBInterfero.Checked)
                    nMMD = MeasurementMode.Interferometric;
                var oRsp = Conn.Exec(CHRocodileLib.CmdID.MeasuringMethod, nMMD);
                MeasuringMethod = (MeasurementMode)oRsp.GetParam<int>(0);
            }
            catch
            {
                Debug.Fail("Cannot set measuring method");
            }
            if (MeasuringMethod == MeasurementMode.Confocal)
                RBConfocal.Checked = true;
            else
                RBInterfero.Checked = true;
        }

        private void SetUpOutputSignals()
        {
            try
            {
                //Set device output signals
                char[] delimiters = new char[] { ' ', ',', ';' };
                int[] signals = TBSODX.Text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).
                    Select(int.Parse).ToArray();
                var oRsp = Conn.Exec(CHRocodileLib.CmdID.OutputSignals, signals);
                SignalIDs = oRsp.GetParam<int[]>(0);
            }
            catch
            {
                Debug.Fail("Cannot set output signals");
            }
            TBSODX.Text = String.Join(",", SignalIDs.Select(p => p.ToString()).ToArray());
        }

        private void SetUpScanrate()
        {
            try
            {
                float nSHZ = float.Parse(TBSHZ.Text);
                var oRsp = Conn.Exec(CHRocodileLib.CmdID.ScanRate, nSHZ);
                ScanRate = oRsp.GetParam<float>(0);
            }
            catch
            {
                Debug.Fail("Cannot set scan rate");
            }
            TBSHZ.Text = ScanRate.ToString();
        }


        private void EnableGui(bool _bEnabled)
        {
            BtConnect.Enabled = !_bEnabled;
            BtDisCon.Enabled = _bEnabled;
            BtRecord.Enabled = _bEnabled;
            EnableSetting(_bEnabled);
        }

        private void EnableSetting(bool _bEnabled)
        {
            RBConfocal.Enabled = _bEnabled && (RBCHR1.Checked || RBCHR2.Checked);
            RBInterfero.Enabled = _bEnabled && (RBCHR1.Checked || RBCHR2.Checked);
            TBSHZ.Enabled = _bEnabled;
            TBSODX.Enabled = _bEnabled;
        }

        private void RBConfocal_Click(object sender, EventArgs e)
        {
            SetUpMeasuringMethod();
        }

        private void TBSHZ_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                SetUpScanrate();
        }

        private void TBSODX_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                SetUpOutputSignals();
        }

        private void StartRecording()
        {
            DrawArea.Width = textBox1.Text != "" ? int.Parse(textBox1.Text) : 500;
            DrawArea.Height = textBox2.Text != "" ? int.Parse(textBox2.Text) : 500;
            labelRecordingHint.Visible = false;
            //throw away the old data
            if (CBFlush.Checked)
                Conn.FlushConnectionBuffer();
            //recording sample count
            SampleCount = int.Parse(TBSampleCount.Text);
            //start recording, enter recording modes
            Conn.StartRecording(SampleCount);
            initDataChart();
            CurrentDataPos = 0;
            timerData.Enabled = true;
            EnableSetting(false);
            BtRecord.Text = "Stop Recording";
            BtRecord.Tag = 1;
            BtSave.Enabled = false;
            recordedPoints.Clear(); // Clear the list at the start of each recording
        }


        private void initDataChart()
        {
            chart1.Series[0].Points.Clear();
            chart2.Series[0].Points.Clear();
            chart3.Series[0].Points.Clear();
            for (int i = 0; i < SampleCount; i++)
            {
                chart1.Series[0].Points.AddY(0);
                chart2.Series[0].Points.AddY(0);
                chart3.Series[0].Points.AddY(0);
            }
        }


        private void StopRecording()
        {
            Console.WriteLine("StopRecording");
            timerData.Enabled = false;
            //stop recording, get recorded data buffer/object
            RecordData = Conn.StopRecording();
            EnableSetting(true);
            BtRecord.Text = "Start Recording";
            BtRecord.Tag = 0;
            BtSave.Enabled = true;
        }

        double Map(double value, double inMin, double inMax, double outMin, double outMax)
        {
            return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }

        public static Color HslToRgb(double h, double s, double l)
        {

            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = l - c / 2;

            double r = 0, g = 0, b = 0;
            if (h >= 0 && h < 60) { r = c; g = x; b = 0; }
            else if (h >= 60 && h < 120) { r = x; g = c; b = 0; }
            else if (h >= 120 && h < 180) { r = 0; g = c; b = x; }
            else if (h >= 180 && h < 240) { r = 0; g = x; b = c; }
            else if (h >= 240 && h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            // Adjust the values
            r = (r + m) * 255;
            g = (g + m) * 255;
            b = (b + m) * 255;

            // Return the RGB color
            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        public Color GetColorForValue(double val, double minDist, double maxDist)
        {

            double m_invRange = maxDist - minDist;

            if (m_invRange < 0)
                return Color.Gray;

            if (double.IsNaN(val) || double.IsNegativeInfinity(val))
                val = minDist;

            if (double.IsPositiveInfinity(val))
                val = maxDist;

            byte r = 0, g = 0, b = 0;

            var gray = Math.Min(val, maxDist);
            gray = 8.0 * (Math.Max(gray, minDist) - minDist) / m_invRange;
            const double s = 255.0 / 2.0;

            if (gray <= 1)
            {
                b = (byte)((gray + 1) * s);
            }
            else if (gray <= 3)
            {
                g = (byte)((gray - 1) * s);
                b = 255;
            }
            else if (gray <= 5)
            {
                r = (byte)((gray - 3) * s);
                g = 255;
                b = (byte)((5 - gray) * s);
            }
            else if (gray <= 7)
            {
                r = 255;
                g = (byte)((7 - gray) * s);
            }
            else
            {
                r = (byte)((9 - gray) * s);
            }

            return Color.FromArgb(255, r, g, b);
        }


        public class Point3D
        {
            public int X { get; set; }
            public int Y { get; set; }
            public double Z { get; set; } // Z will hold the signal value (e.g., s.Get(1))

            public Point3D(int x, int y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }
        private List<Point3D> recordedPoints = new List<Point3D>(); // Store points

        private bool isRunCW = true;

        double minDist = 0.0;
        double maxDist = 4000.0;
        int yCoordinate = 0;

        private void StartRecordingLine_Fire(DBTrigger pTrigger)
        {
            //yCoordinate = axDBCommManager1.ReadDevice(DBPlcDevice.DKVNano_DM, "100");
            //Console.WriteLine("StartRecordingLine_Fire + ycoordinate" + yCoordinate);
            //labelRecordingHint.Visible = false;
            ////throw away the old data
            //if (CBFlush.Checked)
            //    Conn.FlushConnectionBuffer();
            ////recording sample count
            //SampleCount = int.Parse(TBSampleCount.Text);
            ////start recording, enter recording modes
            //Conn.StartRecording(SampleCount);
            //CurrentDataPos = 0;
            ////Console.WriteLine("StartRecordingLine_Fire");
            //timerData.Enabled = true;
            //EnableSetting(false);
            //BtRecord.Text = "Stop Recording";
            //BtRecord.Tag = 1;
            //BtSave.Enabled = false;
            //recordedPoints.Clear(); // Clear the list at the start of each recording
        }

        private void StopRecordingLine_Fire(DBTrigger pTrigger)
        {
            //Console.WriteLine("StopRecordingLine_Fire");
            //timerData.Enabled = false;
            //stop recording, get recorded data buffer / object
            //RecordData = Conn.StopRecording();
            //EnableSetting(true);
            //BtRecord.Text = "Start Recording";
            //BtRecord.Tag = 0;
            //BtSave.Enabled = true;
        }

        int numberOfSample;
        List<double> pointInLine = new List<double>();
        List<double> dataPrint = new List<double>();
        double[] sampleData = new double[800];

        bool isFisrtTimeRun = true;
        private void RunScan_Fire(DBTrigger pTrigger)
        {
            try
            {

                if (axDBCommManager1.ReadDevice(DBPlcDevice.DKVNano_MR, "105") != 0)
                {
                    pointInLine.Clear();
                    dataPrint.Clear();
                    labelRecordingHint.Visible = false;
                    //throw away the old data
                    if (CBFlush.Checked)
                        Conn.FlushConnectionBuffer();
                    //recording sample count
                    SampleCount = int.Parse(TBSampleCount.Text);
                    //start recording, enter recording modes
                    Conn.StartRecording(SampleCount);
                    CurrentDataPos = 0;
                    timerData.Enabled = true;
                    EnableSetting(false);
                    BtRecord.Text = "Stop Recording";
                    BtRecord.Tag = 1;
                    BtSave.Enabled = false;
                    yCoordinate++;
                }
                else
                {
                    timerData.Enabled = false;
                    #region Print data logic

                    #region add point logic
                    if (isFisrtTimeRun)
                    {
                        if (pointInLine.Count % 2 != 0)
                            pointInLine.Insert(0, double.NaN);
                        numberOfSample = pointInLine.Count;
                        isFisrtTimeRun = false;
                        Console.WriteLine("isFisrtTimeRun: " + isFisrtTimeRun);
                    }
                    if (pointInLine.Count % 2 != 0)
                    {
                        if (pointInLine.Count < numberOfSample)
                            pointInLine.RemoveAt(0);
                        else
                            pointInLine.Insert(0, double.NaN);
                    }
                    #endregion
                    //if (pointInLine.Count > 20)
                    //{
                    //    pointInLine.RemoveRange(0, 20); // Remove first 20 elements
                    //    pointInLine.RemoveRange(pointInLine.Count - 20, 20); // Remove last 20 elements
                    //}
                    //else
                    //{
                    //    Console.WriteLine("dataPrint" + dataPrint.Count);

                    //}
                    int j = 0;
                    for (int i = (sampleData.Length / 2) - (pointInLine.Count / 2); i < sampleData.Length / 2; i++)
                    {
                        Console.WriteLine("sampleData[j]: " + sampleData[j] + " / " + "j: " + j);

                        sampleData[i] = pointInLine[j];
                        j++;
                    }
                    for (int i = (sampleData.Length / 2); i < (sampleData.Length / 2) + (pointInLine.Count / 2); i++)
                    {
                        Console.WriteLine("sampleData[j]: " + pointInLine[j] + " / " + "j: " + j);

                        sampleData[i] = pointInLine[j];
                        j++;
                    }
                    //Console.WriteLine("pointInLine: " + pointInLine.Count);

                    //Console.WriteLine("sampleData" + sampleData.Length);
                    double[] trimmedArray = RemoveFirstAndLastElements(sampleData, 20);


                    //for (int i = (pointInLine.Count / 2) - 150; i < pointInLine.Count / 2; i++)
                    //{
                    //    dataPrint.Add(pointInLine[i]);
                    //}
                    //for (int i = pointInLine.Count / 2; i < (pointInLine.Count / 2) + 150; i++)
                    //{
                    //    dataPrint.Add(pointInLine[i]);
                    //}

                    //int middle = pointInLine.Count / 2;
                    //int range = 200;

                    //for (int i = middle - range; i < middle + range; i++)
                    //{
                    //    if (i >= 0 && i < pointInLine.Count) // Ensure the index is within the valid range
                    //    {
                    //        dataPrint.Add(pointInLine[i]);
                    //    }
                    //}



                    //List<double> result = SubArrayAfterZeroes(pointInLine);
                    //int middlePonint = pointInLine.Count / 2;

                    //int middlePonint = result.Count / 2;
                    Graphics g = DrawArea.CreateGraphics(); // Get Graphics object for the panel

                    for (int i = 0; i < trimmedArray.Length; i++)

                    {
                        // Calculate the position of the point based on the total number of points processed

                        // Map signal to color
                        Color color2 = GetColorForValue(trimmedArray[i], minDist, maxDist);
                        Brush brush = new SolidBrush(color2);

                        recordedPoints.Add(new Point3D(i, yCoordinate, trimmedArray[i]));
                        // Draw the point

                        g.FillRectangle(brush, i, yCoordinate, 1, 1); // Fill a 1x1 rectangle to draw the point
                        brush.Dispose();

                    }

                    g.Dispose();
                    #endregion
                    //stop recording, get recorded data buffer / object
                    RecordData = Conn.StopRecording();
                    EnableSetting(true);
                    BtRecord.Text = "Start Recording";
                    BtRecord.Tag = 0;
                    BtSave.Enabled = true;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        static double[] RemoveFirstAndLastElements(double[] inputArray, int count)
        {
            // Check if the array has enough elements to trim
            if (inputArray.Length <= 2 * count)
            {
                throw new ArgumentException("Array does not have enough elements to remove.");
            }

            // Calculate the length of the new array
            int newLength = inputArray.Length - 2 * count;

            // Create a new array for the middle elements
            double[] trimmedArray = new double[newLength];

            // Copy the middle elements to the new array
            Array.Copy(inputArray, count, trimmedArray, 0, newLength);

            return trimmedArray;
        }

        public static List<double> SubArrayAfterZeroes(List<double> inputList)
        {
            // Check if the list is empty
            if (inputList == null || inputList.Count == 0)
                return new List<double>(); // Return an empty list

            // Find the start index: the first non-zero element after at least one zero
            int startIndex = -1;
            bool hasZeroBefore = false;

            for (int i = 0; i < inputList.Count; i++)
            {
                if (inputList[i] == 0)
                {
                    hasZeroBefore = true; // Mark that we've encountered zeroes
                }
                else if (hasZeroBefore)
                {
                    startIndex = i; // Found the first non-zero element after zeroes
                    break;
                }
            }

            // If no valid start index was found, return an empty list
            if (startIndex == -1)
                return new List<double>();

            // Return the sublist starting from the identified index
            return inputList.GetRange(startIndex, inputList.Count - startIndex);
        }
        int count = 0;
        private void timerData_Tick(object sender, EventArgs e)
        {
            // Read in newly recorded samples
            var oData = Conn.GetNextSamples();
            if (oData.NumSamples > 0 && isRunCW)
            {
                foreach (var s in oData.Samples())
                {
                    // Get the second signal value
                    double signalValue = s.Get(1); // Second signal

                    // Calculate the position of the point based on the total number of points processed

                    pointInLine.Add(signalValue);

                    // Map signal to color
                    CurrentDataPos++;
                }
            }

        }

        #region Demo
        //private void timerData_Tick(object sender, EventArgs e)
        //{
        // Read in newly recorded samples
        //    var oData = Conn.GetNextSamples();
        //    if (oData.NumSamples > 0 && isRunCW)
        //    {

        //        Graphics g = DrawArea.CreateGraphics(); // Get Graphics object for the panel

        //        foreach (var s in oData.Samples())
        //        {
        //            // Get the second signal value
        //            double signalValue = s.Get(1); // Second signal

        //            // Calculate the position of the point based on the total number of points processed

        //            // Map signal to color
        //            CurrentDataPos++;
        //            Color color2 = GetColorForValue(signalValue, minDist, maxDist);
        //            Brush brush = new SolidBrush(color2);
        //            #region Zig zac
        //            //if (x == panelWidth - 1)
        //            //{
        //            //    return;
        //            //}

        //            //int totalPoints = CurrentDataPos;    // Total points processed so far
        //            //int y = totalPoints / panelWidth;    // Y-coordinate increases as rows fill
        //            //int x;

        //            //if (y % 2 == 0)
        //            //{
        //            //    // Even row: left-to-right
        //            //    x = totalPoints % panelWidth;
        //            //}
        //            //else
        //            //{
        //            //    // Odd row: right-to-left
        //            //    x = panelWidth - 1 - (totalPoints % panelWidth);
        //            //}
        //            #endregion
        //            recordedPoints.Add(new Point3D(CurrentDataPos, yCoordinate, signalValue));
        //            // Draw the point

        //            g.FillRectangle(brush, CurrentDataPos, yCoordinate, 1, 1); // Fill a 1x1 rectangle to draw the point
        //            brush.Dispose();

        //        }


        //        g.Dispose();
        //    }
        //    // Stop recording if enough samples have been acquired
        //    if (CurrentDataPos >= SampleCount)
        //    {
        //        Console.WriteLine("CurrentDataPos: " + CurrentDataPos + " / " + "SampleCount: " + SampleCount);
        //        //timerData.Enabled = false;

        //        StopRecording();
        //    }

        //}
        #endregion


        private void SavePointsToCsv(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("X,Y,Z"); // Add a header row
                foreach (var point in recordedPoints)
                {
                    writer.WriteLine($"{point.X},{point.Y},{point.Z}");
                }
            }
            MessageBox.Show("CSV file saved successfully!");
        }

        private void btnTch_Click(object sender, EventArgs e)
        {
            axDBCommManager1.WriteDevice(DATABUILDERAXLibLB.DBPlcDevice.DKVNano_MR, "108", 1);
        }

        private void RTH_Click(object sender, EventArgs e)
        {
            recordedPoints.Clear(); // Clear the list at the start of each recording
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SavePointsToCsvWithDialog();
        }
        private void SavePointsToCsvWithDialog()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                saveFileDialog.Title = "Save Points to CSV File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SavePointsToCsv(saveFileDialog.FileName); // Pass the chosen file path to the method
                }
            }
        }

        private void BtRecord_Click(object sender, EventArgs e)
        {


            if (Convert.ToInt32((sender as Button).Tag) == 0)
            {
                DrawArea.Refresh();
                StartRecording();
            }
            else
                StopRecording();
        }

        //here save the recorded data into a file 
        private void BtSave_Click(object sender, EventArgs e)
        {
            if (SaveDlg.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(SaveDlg.OpenFile());
                var nSigCount = RecordData.Info.SignalGenInfo.GlobalSignalCount
                    + RecordData.Info.SignalGenInfo.PeakSignalCount;

                //reread all the samples, save...
                RecordData.Rewind();
                foreach (var s in RecordData.Samples())
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < nSigCount; j++)
                    {
                        if (j < RecordData.Info.SignalGenInfo.GlobalSignalCount)
                            sb.Append(s.Get(j) + ", ");
                        else
                        {
                            for (int k = 0; k < RecordData.Info.SignalGenInfo.ChannelCount; k++)
                                sb.Append(s.Get(j, k) + ", ");
                        }
                    }
                    writer.WriteLine(sb.ToString());
                }
                writer.Dispose();
            }
        }

        private void DrawArea_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

        }

        private void axDBDeviceManager2_BeforeRead(object sender, EventArgs e)
        {

        }
    }
}
