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
    internal class TriggeredNormalRectScan
    {
        private const string SCAN_SCRIPT = "AutoRun_Top_normal_area_scan.rs";

        private AsynchronousConnection.ResponseAndUpdateCallback ScanProgramCallback = null;
        private readonly AutoResetEvent _scanStartedEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _loopIterationEvent = new AutoResetEvent(false);
        private bool _errorOccurred = false;
        private void OnScanProgramCallback(Response rsp)
        {
            bool isScan = rsp.Info.CmdID == (CmdID)CHRLibPlugin.CmdID_DFSS_Scan;
            bool isUpdate = rsp.IsUpdate();

            if (!isScan)
                return;

            if (rsp.IsError())
            {
                Console.WriteLine("Error");
                _errorOccurred = true;
                _loopIterationEvent.Set();
                return;
            }

            try
            {

                if (!isUpdate)
                {
                    // This is the initial response that indicates the scan has started.
                    _scanStartedEvent.Set();
                    return;
                }


                if (rsp.ParamCount < 2)
                {
                    _errorOccurred = true;
                    Console.WriteLine("Error: could not get DFSS shape data.");
                    return;
                }

                if (rsp.TryGetParam(0, out byte[] blob1) && rsp.TryGetParam(1, out byte[] blob2))
                {
                    var shape1 = new CHRLibPlugin.FSS_PluginShape(blob1);
                    var shape2 = new CHRLibPlugin.FSS_PluginShape(blob2);
                    Console.WriteLine("Received DFSS shape data.");
                    _loopIterationEvent.Set();
                    // TODO: process the data
                }
                else
                {
                    Console.WriteLine("Error: could not get DFSS shape data.");
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private string _script;

        public TriggeredNormalRectScan()
        {
            _script = File.ReadAllText(SCAN_SCRIPT);
        }

        public void RunScan(AsynchronousMultiConnectionPlugin dfss)
        {
            Lib.LogUserMsg("***Starting auto rect program...", 0, 0xFFFFFFFF);

            ScanProgramCallback = OnScanProgramCallback;
            dfss.ExecWithUserResponseDelegate(
                Cmd.Command((CmdID)CHRLibPlugin.CmdID_DFSS_Scan, CHRLibPlugin.FSS_PROG_InputString, _script),
                ScanProgramCallback);

            _scanStartedEvent.WaitOne();

            Console.WriteLine("Now running test for 100 sec...");

            Thread.Sleep(2000);

            SynchronousCommandGroup cmdSlave = new SynchronousCommandGroup(dfss.Parents[1]);
            SynchronousCommandGroup cmdMaster = new SynchronousCommandGroup(dfss.Parents[0]);

            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine($"Triggering scan {i + 1}...");
                
                // First slave:
                cmdSlave.Add(Cmd.FromStr("STR"));
                cmdSlave.WaitAndThrowOnError();

                // ...then master:
                cmdMaster.Add(Cmd.FromStr("STR"));
                cmdMaster.WaitAndThrowOnError();

                // wait for the scan to complete before triggering the next one
                _loopIterationEvent.WaitOne();
                if (_errorOccurred)
                {
                    Console.WriteLine("Error occurred, stopping.");
                    break;
                }

                // TODO: post-process data, move axes to next wafer position, etc.
            }

            Console.WriteLine("DONE.");
        }

    }
}
