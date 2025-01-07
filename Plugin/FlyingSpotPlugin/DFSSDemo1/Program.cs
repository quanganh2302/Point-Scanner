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
    struct ScanData
    {
        public CHRLibPlugin.FSS_PluginShape shape1;
        public CHRLibPlugin.FSS_PluginShape shape2;
    };



    internal class Program
    {
        private const string SCANNER1_IP = "192.168.170.2"; // is "master", i. e. the one that triggers
        private const string SCANNER2_IP = "192.168.170.3"; // is "slave", i. e. the one that receives the trigger signal

        private const string SCANNER1_CFG = "ScannerGlobalConfigMaster.cfg";
        private const string SCANNER2_CFG = "ScannerGlobalConfigSlave.cfg";
        private const string SCAN_SCRIPT = "FreeRun_Top_small_area_10x10points_25x25um_raster.rs";

        private bool runDFSSCalibration = false; // set to true on first run, then set to false to safe time

        private AsynchronousConnection.ResponseAndUpdateCallback ScanProgramCallback = null;
        private readonly AutoResetEvent _scanFinishedEvent = new AutoResetEvent(false);
        private readonly List<ScanData> _scanData = new List<ScanData>();

        private void OnGeneralCommandResponse(Response rsp)
        {
            // Console.WriteLine($"ScanCallback: {rsp.ToString()}");
        }

        private string ScanScript()
        {
            string s = "";

            s += "init {\n";
            s += "	$SHZ 30000;\n";
            s += "	$LAI 90;\n";
            s += "	$SVF 0 257;\n";
            s += "	$SODX 64 65 66 67 68 69 82;\n";
            s += "}\n";
            s += "\n";
            s += "fn rect_line() {\n";
            s += "	selfTrigger(600)\n";
            s += "	line(x1 = 0, y1 = 0, x2 = 50, y2 = 0, numPts = 4000, waitAtBegin = 1)\n";
            s += "}\n";
            s += "\n";
            s += "fn main(scanFreq= 30000, markerDelay= 300) {\n";
            s += "	startDFSSScan()\n";
            s += "	shape() {\n";
            s += "		moveTo(x = -25, y = 0)\n";
            s += "		waitUsec(10000)\n";
            s += "	}\n";
            s += "\n";
            s += "	for i in range(1, 5) {\n";
            s += "		selfTrigger(600)\n";
            s += "		 line(x1 = -25, y1 = 0, x2 = 25, y2 = 0, numPts = 4240, waitAtBegin = 1)\n";
            s += "		 line(x1 = 25, y1 = 0, x2 = -25, y2 = 0, numPts = 4240, waitAtBegin = 1)\n";
            s += "	}\n";
            s += "	\n";
            s += "	let radius = 25 // mm\n";
            s += "	let step = 0.1 // mm increase of radius per turn\n";
            s += "	let turns = radius / step\n";
            s += "	let slope = (radius - 1) / (2 * M_PI * turns)\n";
            s += "\n";
            s += "	selfTrigger(600)\n";
            s += "	interp2D(x0 = -25, y0 = -25, x1 = 25, y1 = 25, nCols = 4000, nRows = 100, innerRad = 5, outerRad = 5, label = \"area\") {\n";
            s += "		spiral(x0=0, y0=0, a=0.1, b=slope, nTurns=turns, numPts=400000, label=\"spiral\")\n";
            s += "	}\n";
            s += "}\n";
            s += "\n";

            return s;
        }

        private void WriteBitmapData(StreamWriter file, CHRLibPlugin.FSS_PluginShape shape, int sigIndex)
        {
            var dtype = shape.SignalInfos[sigIndex].DataType;
            bool isFloat = dtype == DataType.Float;
            bool isDouble = dtype == DataType.Double;
            FSS_PluginShape.BitmapType bm = shape.Bitmap;

            unsafe
            {
                // get the type data pointer for raw/double data mode
                float* srcF = isFloat ? bm.GetScanLineAs<float>(sigIndex) : null;
                double* srcD = !isFloat ? bm.GetScanLineAs<double>(sigIndex) : null;

                // go over the whole bitmap and convert floating-point values to colors
                for (int y = 0; y < bm.Height; y++, srcF += bm.Width, srcD += bm.Width)
                {
                    for (int x = 0; x < bm.Width; x++)
                    {
                        double val = isFloat ? (double)srcF[x] : srcD[x];
                        file.Write(val.ToString());
                        if (x < bm.Width - 1)
                            file.Write(";");
                    }
                    file.Write('\n');
                }
            } // unsafe
        }
        private void WriteShapeData(StreamWriter file, CHRLibPlugin.FSS_PluginShape shape)
        {
            foreach (var s in shape.Samples())
            {
                for (int i = 0; i < shape.SignalInfos.Length; i++)
                {
                    file.Write($"{s.Get(i)}");
                    if (i < shape.SignalInfos.Length - 1)
                        file.Write(";");
                }
                file.Write("\n");
            }
        }
        private void SaveData()
        {
            int num = 0;
            foreach (var d in _scanData)
            {
                var shapes = new CHRLibPlugin.FSS_PluginShape[2] { d.shape1, d.shape2 };
                for (int i = 0; i < shapes.Length; i++)
                {
                    var s = shapes[i];
                    bool isBitmap = s.Bitmap != null;
                    string bm = isBitmap ? "_bm" : "";
                    string name = $"{num}_{s.Label}_{s.ShapeIndex}{i}{bm}_cs.csv";
                    using (var writer = new StreamWriter(name))
                    {
                        if (isBitmap)
                            // bitmaps do not contain timestamp or encoder values, intensity is here the only signal
                            WriteBitmapData(writer, s, 0);
                        else
                            WriteShapeData(writer, s);
                    }
                    num++;
                }
            }
        }

        private void OnScanProgramCallback(Response rsp)
        {
            try
            {
                Console.WriteLine($"Scan program callback called {rsp}");
                if (rsp.Info.CmdID == (CmdID)CHRLibPlugin.CmdID_DFSS_Scan
                    && rsp.ParamCount >= 2)
                {
                    if (rsp.TryGetParam(0, out byte[] blob1) && rsp.TryGetParam(1, out byte[] blob2))
                    {
                        var shape1 = new CHRLibPlugin.FSS_PluginShape(blob1);
                        shape1.Detach(); // copy data to local storage
                        var shape2 = new CHRLibPlugin.FSS_PluginShape(blob2);
                        shape2.Detach();

                        if (shape1.Type == CHRLibPlugin.FSS_PluginDataType.RecipeTerminate)
                        {
                            Console.WriteLine("ScanCallback: scan finished.");
                            _scanFinishedEvent.Set();
                            return;
                        }

                        Console.WriteLine($"receiving data of shape {shape1.Label} and {shape2.Label}");
                        _scanData.Add(new ScanData { shape1 = shape1, shape2 = shape2 });
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

        private void RunScan(AsynchronousMultiConnectionPlugin dffs, string script)
        {
            Console.WriteLine("Now running test script...");
            dffs.ExecWithUserResponseDelegate(
                Cmd.Command((CmdID)CHRLibPlugin.CmdID_DFSS_Scan, CHRLibPlugin.FSS_PROG_InputString, script),
                ScanProgramCallback);
            _scanFinishedEvent.WaitOne(); // waits for all shape data to be received
            Console.WriteLine("DONE.");
        }

        AsynchronousConnection _scanner1;
        AsynchronousConnection _scanner2;
        AsynchronousConnection.Plugin _fss1;
        AsynchronousConnection.Plugin _fss2;
        AsynchronousMultiConnectionPlugin _dfss;

        private void Initialize(bool runDFSSCalibration)
        {
            Console.WriteLine($"setting up connections {SCANNER1_IP} and {SCANNER2_IP}...");

            _scanner1 = new AsynchronousConnection(SCANNER1_IP, DeviceType.Chr2);
            _scanner2 = new AsynchronousConnection(SCANNER2_IP, DeviceType.Chr2);
            _fss1 = _scanner1.InsertPlugin("FlyingSpotPlugin");
            _fss2 = _scanner2.InsertPlugin("FlyingSpotPlugin");

            _dfss = AsynchronousMultiConnectionPlugin.Setup(new AsynchronousConnection[] { _scanner1, _scanner2 }, "DFSSPlugin");
            _dfss.SetGeneralResponseCallback(OnGeneralCommandResponse);
            ScanProgramCallback = OnScanProgramCallback;
            _dfss.AutomaticMode = true;
            Console.WriteLine($"configuring using files {SCANNER1_CFG} and {SCANNER2_CFG}...");

            SynchronousCommandGroup cmds = new(_dfss);
            cmds.Add(Cmd.Command((CmdID)CHRLibPlugin.CmdID_DFSS_Set_Config, SCANNER1_CFG, SCANNER2_CFG));
            cmds.WaitAndThrowOnError();

            if (runDFSSCalibration)
            {
                Console.WriteLine("DONE. Now Calibrating DFSS alignment...");
                // run alignment calibration:
                // move 30mm from center in each horizontal direction
                // let master run a pre-scan to search the spot of the slave:
                // rectangular size = 10 * 10 mm (which covers up to +/-5mm slave alignment deviation)
                // slave's LAI = 20 (which is related to it's exposure time / sensitivity)
                // all parameters are optional, default values apply to FSS 80.
                cmds.Add(Cmd.Command((CmdID)CHRLibPlugin.CmdID_DFSS_Calib, 30, 5, 5, 20));
                cmds.WaitAndThrowOnError();
            }
            Console.WriteLine("DONE.");

        }

        SmallRectContinuousScan sr = new();
        TriggeredNormalRectScan tn = new();

        public void Run(string demoIdx)
        {
            SynchronousCommandGroup cmds = new(_dfss);

            if (demoIdx == "1")
            {
                Console.WriteLine($"Command line arg == {demoIdx} -> starting demo small rect continuous scan.");

                sr.RunScan(_dfss);

                cmds.Add(Cmd.Command((CmdID)CHRLibPlugin.CmdID_DFSS_Stop));
                cmds.WaitAndThrowOnError();
            }
            else if (demoIdx == "2")
            {
                Console.WriteLine($"Command line arg == {demoIdx} -> starting demo normal rect STR - triggered scan.");

                tn.RunScan(_dfss);

                cmds.Add(Cmd.Command((CmdID)CHRLibPlugin.CmdID_DFSS_Stop));
                cmds.WaitAndThrowOnError();
            }
            else
            {
                Console.WriteLine($"Command line arg == {demoIdx} -> must be 1 or 2.");
            }
        }
        static void Main(string[] args)
        {
            Lib.SetLibLogFileDirectory(".", 1024 * 1024, 50);
            Lib.SetLibLogLevel(4);

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: CSharpDFSSDemo1 <demo index> [calib] [iterations count (default == 10)]");
                Console.WriteLine("demo index: 1 -> small rect continuous scan, 2 -> software (STR) triggered rect scan");
                return;
            }

            bool mustCalibrate = args.Length > 1 && args[1] == "calib";
            int iterations = 50;
            if (args.Length > 2)
            {
                if (int.TryParse(args[2], out int iterationsParam))
                    iterations = iterationsParam;
            }

            try
            {
                Program p = new Program();
                p.Initialize(mustCalibrate); // configure connections and calibrate if requested (only once)

                for (int i = 0; i < iterations; i++)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"--------- iteration #{i} ------------");
                    p.Run("1");
                    p.Run("2");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failure: {ex.ToString()}");
            }
            finally
            {
                Console.WriteLine($"DEMO FINISHED.");
            }
        }
    }
}
