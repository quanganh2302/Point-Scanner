using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CHRocodileLib;

namespace CSharpAwaitProcess2
{
    public partial class AsyncProcessDemo2 : Form
    {
        private const int MaxSpectrumPixels = 512;
        private AsynchronousConnection _con = null;

        public AsyncProcessDemo2()
        {
            InitializeComponent();
            for (int i = 0; i < MaxSpectrumPixels; i++)
                ChartSpectrum.Series[0].Points.AddY(i);
        }

            private void StopDeviceSearch()
        {
            TimerDeviceSearch.Stop();
            DeviceSearch.Cancel(); // does not hurt if search not active
            BDiscover.Text = "Discover";
        }

        private void BDiscover_Click(object sender, EventArgs e)
        {
            if (BDiscover.Text == "Discover")
            {
                DeviceSearch.Start();
                BDiscover.Text = "Cancel";
                TimerDeviceSearch.Start();
            }
            else
            {
                StopDeviceSearch();
            }
        }

        private void TimerDeviceSearch_Tick(object sender, EventArgs e)
        {
            if (DeviceSearch.DiscoveredDevices().Count > CBSensorAddress.Items.Count)
            {
                CBSensorAddress.Items.Clear();
                foreach (var device in DeviceSearch.DiscoveredDevices())
                {
                    string entry = $"{device.IPAddr}: {device.DevType.ToString()}";
                    CBSensorAddress.Items.Add(entry);
                    CBSensorAddress.SelectedIndex = 0;
                    BConnect.Enabled = true;
                }
            }
            if (!DeviceSearch.IsFinished())
                return;

            StopDeviceSearch();
            if (DeviceSearch.DiscoveredDevices().Count() > 0)
            {
                BConnect.Enabled = true;
            }
            else
            {
                CloseConnection();
                MessageBox.Show("No sensor found.", "Sensor Discovery", MessageBoxButtons.OK);
                BConnect.Enabled = false;
            }
        }

        private void BConnect_Click(object sender, EventArgs e)
        {
            StopDeviceSearch();
            OpenConnection();
        }

        private async void BDarkCorrection_ClickAsync(object sender, EventArgs e)
        {
            var rsp = await _con.ExecAsync(Cmd.Command(CmdID.DarkReference));
            MessageBox.Show($"Dark level = {rsp.Params[0]}", "Error", MessageBoxButtons.OK);
        }

        private void RefreshSpectrumChart(Response rsp)
        {
            var aBytes = rsp.GetParam<byte[]>(rsp.ParamCount - 1);
            //convert to 16bit data
            Int16[] SpecData = new Int16[aBytes.Length / 2];
            Buffer.BlockCopy(aBytes, 0, SpecData, 0, aBytes.Length);
            int len = Math.Min(MaxSpectrumPixels, SpecData.Length);
            var series = ChartSpectrum.Series[0];
            for (int i = 0; i < len; i++)
                series.Points[i].YValues[0] = SpecData[i];
            ChartSpectrum.ChartAreas[0].RecalculateAxesScale();
            ChartSpectrum.Invalidate();
        }

        private async void TimerRefreshSpectrumChart_Tick(object sender, EventArgs e)
        {
            var rsp = await _con.ExecAsync(Cmd.Command(CmdID.DownloadSpectrum, 0));
            if (this.Visible) // this function and Form.close can overlap
                RefreshSpectrumChart(rsp);
        }

        private void OpenConnection()
        {
            StopDeviceSearch();
            if (CBSensorAddress.Items.Count > 0)
            {
                var dev = DeviceSearch.DiscoveredDevices()[CBSensorAddress.SelectedIndex];
                _con = new AsynchronousConnection(dev.IPAddr, dev.DevType);
                _con.AutomaticMode = true; // do not forget
                BConnect.Enabled = false;
                BDarkCorrection.Enabled = true;
                TimerRefreshSpectrumChart.Start();
            }
        }

        private void CloseConnection()
        {
            TimerRefreshSpectrumChart.Enabled = false;
            BDarkCorrection.Enabled = false;
            if (_con != null)
            {
                _con.Close();
                _con = null;
            }
            BConnect.Enabled = true;
        }
        public static void DlgException(object sender, ThreadExceptionEventArgs t)
        {
            var dlg = (AsyncProcessDemo2)Application.OpenForms[0];
            dlg.CloseConnection();
            string message = $"An error occurred: {t.Exception.Message}";
            MessageBox.Show(message, "Error", MessageBoxButtons.OK);
        }

        private void AsyncProcessDemo2_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseConnection();
        }
    }
}
