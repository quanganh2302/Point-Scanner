/*
This console demo is to demonstrate, how to create a asynchronous connection with CHRocodile² device
and then send commands (either using command ID or pure command string) and collecting data.
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CHRocodileLib;
using static CHRLibPlugin;


namespace TDFFSDemo1Console
{
    //struct ScanData
    //{
    //    public CHRLibPlugin.FSS_PluginShape shape1;
    //    public CHRLibPlugin.FSS_PluginShape shape2;
    //};

    internal class SmallRectContinuousScan
    {
        private const string SCAN_SCRIPT = "FreeRun_Top_small_area_10x10points_25x25um_raster.rs";

        private AsynchronousConnection.ResponseAndUpdateCallback ScanProgramCallback = null;

        private static double Average(FSS_PluginShape shape1)
        {
            double v = 0.0;
            foreach (var s in shape1.Samples())
                v += s.Get(5); // must be adapted to SODX order
            v /= shape1.NumSamples;
            return v;
        }

        private void OnScanProgramCallback(Response rsp)
        {
            try
            {
                if (rsp.Info.CmdID == (CmdID)CHRLibPlugin.CmdID_DFSS_Scan
                    && rsp.ParamCount >= 2)
                {
                    if (rsp.TryGetParam(0, out byte[] blob1) && rsp.TryGetParam(1, out byte[] blob2))
                    {
                        var shape1 = new CHRLibPlugin.FSS_PluginShape(blob1);
                        var shape2 = new CHRLibPlugin.FSS_PluginShape(blob2);
                        double avr1 = Average(shape1);
                        double avr2 = Average(shape2);
                        // TODO: process the data
                        Console.WriteLine($"data: {avr1}, {avr2}");
                    }
                    else
                    {
                        Console.WriteLine("Error: could not get DFSS shape data.");
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private string _script;

        public SmallRectContinuousScan()
        {
            _script = File.ReadAllText(SCAN_SCRIPT);
        }

        public void RunScan(AsynchronousMultiConnectionPlugin dfss)
        {
            Lib.LogUserMsg("***Starting small rect program...", 0, 0xFFFFFFFF);
            ScanProgramCallback = OnScanProgramCallback;
            dfss.ExecWithUserResponseDelegate(
                Cmd.Command((CmdID)CHRLibPlugin.CmdID_DFSS_Scan, CHRLibPlugin.FSS_PROG_InputString, _script),
                ScanProgramCallback);

            Console.WriteLine("Now running test for 10 sec...");
            Thread.Sleep(2000); // this determines the time and so the number of scans
            // foreach (var conn in dfss.Parents)
                // conn.ExecStringAsync("SCAN 0"); // stop scan on both scanners

            Console.WriteLine("DONE.");
        }

    }
}
