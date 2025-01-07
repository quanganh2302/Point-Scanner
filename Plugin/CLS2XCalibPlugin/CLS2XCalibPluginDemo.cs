/*
This demo shows how to use CLS2 calibration plugin under both synchronous and asynchronous modes.
After opening up the connection, plugin is directly added into the connection based on plugin name. 
Then the calibration file which is used in the plugin is set.
User can also enable and disable the calibration correction through sending command to the plugin.
*/


using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using CHRocodileLib;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace TCHRLibCLS2XCalibPlugin
{
    public partial class MainForm : Form
    {
        SynchronousConnection ConnSync = null;
        SynchronousConnection.Plugin CalibPluginSync = null;

        AsynchronousConnection ConnAsync = null;
        AsynchronousConnection.Plugin CalibPluginAsync = null;


        bool Connected = false;
        //Flag: whether connection is synchronous or asynchronous
        bool isSynConn;
        //Flag: whether calibration correction is active
        bool UseCorrectionEnable;

        const Int32 Data_Length = 1024;
        double[] GlobalData;
        Int16[] PeakData1, PeakData2;
        Int32 CurrentDataPos;

        public MainForm()
        {
            InitializeComponent();
            //save one global signal to display in upper chart, x-axis sample index
            GlobalData = new double[Data_Length];
            // peak data of all the channels to display in the two lower charts, x-axis channel index
            PeakData1 = new Int16[0];
            PeakData2 = new Int16[0];



            for (Int32 i = 0; i < Data_Length; i++)
                chart1.Series[0].Points.AddY(i);

            //set logging property
            Lib.SetLibLogFileDirectory(".", 500, 20);
        }

        private void BtConnect_Click(object sender, EventArgs e)
        {
            if (Connected)
                CloseConnection();
            else
            {
                try
                {
                    isSynConn = RBSyncConn.Checked;
                    if (isSynConn)
                    {
                        //Open connection in synchronous mode
                        ConnSync = new SynchronousConnection(TBConnInfo.Text, DeviceType.MultiChannel, 256 * 1024 * 1024);
                        //Add calibration plugin
                        CalibPluginSync = ConnSync.InsertPlugin(CHRLibPlugin.CLS2_X_Calib_Plugin_Name);
                        // use original data format instead of double to save data size
                        Lib.SetOutputDataFormatMode(ConnSync.Handle, OutputDataFormat.Raw);
                    }
                    else
                    {
                        //Open connection in asynchronous mode
                        ConnAsync = new AsynchronousConnection(TBConnInfo.Text, DeviceType.MultiChannel, 256 * 1024 * 1024);
                        //Add calibration plugin
                        CalibPluginAsync = ConnAsync.InsertPlugin(CHRLibPlugin.CLS2_X_Calib_Plugin_Name);
                        // use original data format instead of double to save data size
                        Lib.SetOutputDataFormatMode(ConnAsync.Handle, OutputDataFormat.Raw);
                        SetupAsyncConn();
                    }
                       
                    //set up device
                    SetupDevice();
                    //set up plugin
                    SetupPlugin();
                    CurrentDataPos = 0;
                    //timer call GetNextSamples to read data
                    TimerProcess.Enabled = isSynConn;                 
                    Connected = true;
                    UpdateGUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message} - closing connection.");
                    CloseConnection();
                }
            }
        }

        private void CloseConnection()
        {
            TimerProcess.Enabled = false;
            if (isSynConn)
            {
                ConnSync?.Close();
                CalibPluginSync = null;
                ConnSync= null;
            }
            else
            {

                ConnAsync?.Close();
                CalibPluginAsync = null;
                ConnAsync = null;
            }
                
                
            Connected = false;
            UpdateGUI();
        }



        private void SetupDevice()
        {
            updateSODX();
            updateSHZ();
        }


        private void SetupPlugin()
        {
            //By default, plugin calibration is active
            UseCorrectionEnable = true;
            setCalibMode();
            setProcessMode();
            //set external calibration file if it has been selected
            if (CBExternalFile.Checked)
                setExternCalibFile(TBCalibFile.Text);

        }

        //according to the response to update GUI interface
        private void UpdateResponse(Response _oRsp)
        {
            if (_oRsp == null)
                return;

            //check whether the response is from calibration plugin
            if ((isSynConn && _oRsp.Source == CalibPluginSync.Handle) || (!isSynConn && _oRsp.Source == CalibPluginAsync.Handle))
            {
                //plugin response
                //if it is correction active command response, update corresponding GUI element.
                if ((uint)_oRsp.Info.CmdID == CHRLibPlugin.CmdID_Calib_Active)
                {
                    Int32 bActive = _oRsp.GetParam<int>(0);
                    UseCorrectionEnable = (bActive != 0);
                    if (UseCorrectionEnable)
                        BtPluginEnable.Text = "Disable Correction";
                    else
                        BtPluginEnable.Text = "Enable Correction";
                }
                else if ((uint)_oRsp.Info.CmdID == CHRLibPlugin.CmdID_Calib_Mode)
                {
                    Int32 bMode = _oRsp.GetParam<int>(0);
                    CBExternalFile.Checked = bMode == 0;
                }
                else if ((uint)_oRsp.Info.CmdID == CHRLibPlugin.CmdID_X_Calib_Process_Mode)
                {
                    Int32 bProcessMode = _oRsp.GetParam<int>(0);
                    CBAddXPos.Checked = bProcessMode == 1;
                }
            }
            else
            {
                //device response
                if (_oRsp.Info.CmdID == CmdID.OutputSignals)
                {
                    int[] aSigIDs;
                    if (_oRsp.ParamCount > 0)
                        aSigIDs = _oRsp.GetParam<int[]>(0);
                    else
                        aSigIDs = new int[0];
                    TBSODX.Text = String.Join(",", aSigIDs.Select(p => p.ToString()).ToArray());

                }
                else if (_oRsp.Info.CmdID == CmdID.ScanRate)
                {
                    TBSHZ.Text = _oRsp.GetParam<float>(0).ToString();
                }
            }

            var strRsp = _oRsp.ToString();
            if (RTResponse.Lines.Length>0)
                RTResponse.AppendText(Environment.NewLine);
            RTResponse.AppendText(strRsp);
        }

        //command response callback
        private void GenCmdCbFct(Response _Rsp)
        {
            this.BeginInvoke((Action <Response>)delegate (Response _oRsp)
            {
                UpdateResponse(_oRsp);
            }, _Rsp);
        }

        //Data callback
        private void ReceiveSample(AsyncDataStatus _status, Data _oData)
        {
            if (_oData.NumSamples > 0)
            {
                ProcessData(_oData);
                if (_oData.NumSamples > 0)
                {
                    this.BeginInvoke((Action)delegate ()
                    {
                        DisplayData();
                    });
                }
            }

            if (_oData.Status == DataStatus.Error)
            {
                Console.WriteLine("Error in processing device output!");
            }
        }



        //Setup asynchronous callback functins and start device output processing
        private void SetupAsyncConn()
        {
            //register callback function 
            ConnAsync.SetDataCallback(ReceiveSample);
            ConnAsync.SetGeneralResponseCallback(GenCmdCbFct);
            //set CHRocodileLib to automatically process device output, 
            //i.e. let CHRocodileLib to create an internal thread for output processing
            //all the reponses and data are delivered through callback function withing CHRocodileLib internal thread 
            ConnAsync.AutomaticMode= true;
        }

        //needed in synchronous mode to constantly call GetNextSamples to read data
        private void TimerProcess_Tick(object sender, EventArgs e)
        {
            try
            {

                var oData = ConnSync.GetNextSamples();
                ProcessData(oData);
                if (oData.NumSamples > 0)
                    DisplayData();
            }
            catch
            {

            }

        }


        //function to process data
        private void ProcessData(Data _oData)
        {
            if (_oData.NumSamples == 0)
                return;
            var itSamples = _oData.Samples();
            foreach (var s in itSamples)
            {
                //Upper chart always displays the first global signal, time based
                if (_oData.Info.SignalGenInfo.GlobalSignalCount > 0)
                {
                    GlobalData[CurrentDataPos] = s.Get(0);
                    CurrentDataPos++;
                    if (CurrentDataPos >= Data_Length)
                        CurrentDataPos = 0;
                }
            }

            
            //lower chart always displays the first two peak signals of all the channels of the last sample
            //if only peak distance is ordered from the device, when the plugin is set to correct distance,
            // there is only one peak signal, which is the distance signal
            // however if the plugin is set to add X position, there are two peak signals, which are the distance and X position signals
            if (_oData.Info.SignalGenInfo.PeakSignalCount > 0)
            {
                PeakData1 = new Int16[_oData.Info.SignalGenInfo.ChannelCount];

                var sample = itSamples.Last();
                for (int i = 0; i < _oData.Info.SignalGenInfo.ChannelCount; i++)
                {
                    Int32 temp = (Int32)sample.Get(_oData.Info.SignalGenInfo.GlobalSignalCount, i);
                    PeakData1[i] = (Int16) (temp & 0x7fff);
                }
            }
            else
                PeakData1 = new Int16[0];

            if (_oData.Info.SignalGenInfo.PeakSignalCount > 1)
            {
                PeakData2 = new Int16[_oData.Info.SignalGenInfo.ChannelCount];

                var sample = itSamples.Last();
                for (int i = 0; i < _oData.Info.SignalGenInfo.ChannelCount; i++)
                {
                    PeakData2[i] = (Int16)sample.Get(_oData.Info.SignalGenInfo.GlobalSignalCount + 1, i);
                }
            }
            else
                PeakData2 = new Int16[0];
        }


        void DisplayData()
        {
            for (Int32 i = 0; i < Data_Length; i++)
                chart1.Series[0].Points[i].YValues[0] = GlobalData[i];
            Int32 nOldCount = chart2.Series[0].Points.Count;
            for (Int32 i = nOldCount; i < PeakData1.Length; i++)
                chart2.Series[0].Points.AddY(0);
            for (Int32 i = nOldCount - 1; i >= PeakData1.Length; i--)
                chart2.Series[0].Points.RemoveAt(i);
            for (Int32 i = 0; i < PeakData1.Length; i++)
                chart2.Series[0].Points[i].YValues[0] = PeakData1[i];
            nOldCount = chart3.Series[0].Points.Count;
            for (Int32 i = nOldCount; i < PeakData2.Length; i++)
                chart3.Series[0].Points.AddY(0);
            for (Int32 i = nOldCount - 1; i >= PeakData2.Length; i--)
                chart3.Series[0].Points.RemoveAt(i);
            for (Int32 i = 0; i < PeakData2.Length; i++)
                chart3.Series[0].Points[i].YValues[0] = PeakData2[i];
            chart1.ChartAreas[0].RecalculateAxesScale();
            chart1.Invalidate();
            chart2.ChartAreas[0].RecalculateAxesScale();
            chart2.Invalidate();
            chart3.ChartAreas[0].RecalculateAxesScale();
            chart3.Invalidate();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseConnection();
        }

        //execute string command to device
        private void TBCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Return)
                return;

            string strCmd = TBCommand.Text;
            try
            {

                if (isSynConn)
                {
                    var oRsp = ConnSync.ExecString(strCmd);
                    UpdateResponse(oRsp);
                }
                else
                {
                    ConnAsync.ExecString(strCmd);
                }
            }
            catch
            {

            }
        }

        // send to order signals from device
        private void updateSODX()
        {
            try
            {
                char[] delimiters = new char[] { ' ', ',', ';' };
                var parts = TBSODX.Text.Split(delimiters).
                  Select(part => part.Trim()).
                  Where(part => !string.IsNullOrEmpty(part));
                int[] aSigs = Array.ConvertAll<string, int>(parts.ToArray<string>(), int.Parse);
                if (isSynConn)
                {
                    var rsp = ConnSync.Exec(CmdID.OutputSignals, aSigs);
                    UpdateResponse(rsp);
                }
                else
                    ConnAsync.Exec(CmdID.OutputSignals, null, aSigs);
            }
            catch
            {

            }
        }

        private void TBSODX_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Return)
                return;

            updateSODX();
        }

        // set scan rate
        private void updateSHZ()
        {
            try
            {
                float nSHZ = float.Parse(TBSHZ.Text);
                if (isSynConn)
                {
                    var rsp = ConnSync.Exec(CmdID.ScanRate, nSHZ);
                    UpdateResponse(rsp);
                }
                else
                    ConnAsync.Exec(CmdID.ScanRate, null, nSHZ);
            }
            catch
            {
            }
        }

        private void TBSHZ_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Return)
                return;
            updateSHZ();
        }


        private void UpdateGUI()
        {
            if (Connected)
            {
                BtConnect.Text = "Disconnect";
                BtConnect.BackColor = Color.LightGreen;
            }
            else
            {
                BtConnect.Text = "Connect";
                BtConnect.BackColor = SystemColors.Control;
            }
            TBConnInfo.Enabled = !Connected;
            RBSyncConn.Enabled = !Connected;
            RBAsyncConn.Enabled = !Connected;
            BtPluginEnable.Enabled = Connected;

        }

        private void ChangeCalibEnable()
        {
            try
            {
                //Set calibration file
                Int32 bActive = UseCorrectionEnable ? 0 : 1;
                if (isSynConn)
                {
                    var rsp = CalibPluginSync.Exec((CmdID)CHRLibPlugin.CmdID_Calib_Active, bActive);
                    UpdateResponse(rsp);
                }
                else
                    CalibPluginAsync.Exec((CmdID)CHRLibPlugin.CmdID_Calib_Active, null, bActive);
            }
            catch
            {
            }
        }

        //enable or disable calibration correction, send CmdID_Calib_Active command to calibration plugin
        private void BtPluginEnable_Click(object sender, EventArgs e)
        {
            ChangeCalibEnable();
        }

        // set external calibration file
        private void setExternCalibFile(string _strFile)
        {
            TBCalibFile.Text = _strFile;
            if (TBCalibFile.Text == "")
                return;
            if (isSynConn)
            {
                var rsp = CalibPluginSync.Exec((CmdID)CHRLibPlugin.CmdID_CalibFile_Name, _strFile);
                UpdateResponse(rsp);
            }
            else
                CalibPluginAsync.Exec((CmdID)CHRLibPlugin.CmdID_CalibFile_Name, null, _strFile);
            CBExternalFile.Checked = true;
        }

        private void BtCalibFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                setExternCalibFile(openFileDialog1.FileName);
            }
        }

        //set plugin calibration mode to use external file or device internal calibration data
        private void setCalibMode()
        {
            if (isSynConn)
            {
                var rsp = CalibPluginSync.Exec((CmdID)CHRLibPlugin.CmdID_Calib_Mode, CBExternalFile.Checked ? 0 : 1);
                UpdateResponse(rsp);
            }
            else
                CalibPluginAsync.Exec((CmdID)CHRLibPlugin.CmdID_Calib_Mode, null, CBExternalFile.Checked ? 0 : 1);
        }

        private void CBExternalFile_CheckedChanged(object sender, EventArgs e)
        {
            setCalibMode();
        }

        // set plugin to correct distance (intensity) data or add X position data
        private void setProcessMode()
        {
            if (isSynConn)
            {
                var rsp = CalibPluginSync.Exec((CmdID)CHRLibPlugin.CmdID_X_Calib_Process_Mode, CBAddXPos.Checked ? 1 : 0);
                UpdateResponse(rsp);
            }
            else
                CalibPluginAsync.Exec((CmdID)CHRLibPlugin.CmdID_X_Calib_Process_Mode, null, CBAddXPos.Checked ? 1 : 0);
        }

        private void CBAddXPos_CheckedChanged(object sender, EventArgs e)
        {
            setProcessMode();
        }

        // send to command to plugin
        private void TBPluginCmd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Return)
                return;

            string strCmd = TBPluginCmd.Text;
            try
            {

                if (isSynConn)
                {
                    var oRsp = CalibPluginSync.ExecString(strCmd);
                    UpdateResponse(oRsp);
                }
                else
                {
                    CalibPluginAsync.ExecString(strCmd);
                }
            }
            catch
            {

            }
        }
    }
}
