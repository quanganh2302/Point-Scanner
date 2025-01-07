using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CHRocodileLib;

namespace TCHRLibAsyncScanConsole
{
    internal class Axis : IDisposable
    {
        public CHRocodileLib.AsynchronousConnection Con { get; set; }
        public void Dispose() { }

        public void MoveToStart() { Console.WriteLine("Asis moving to start..."); }
        public void MoveToEnd()
        {
            Thread.Sleep(100); // simulate axis acceleration
            Console.WriteLine("Asis moving to end...");
            // simulate encoder / index move -> triggers sensor
            // NOTE: This does not work with FSS connected!
            Con.Exec("STR", null);
        }
    }

    internal struct ChrDataSample
    {
        public int Counter;
        public List<double> DistanceOne;
        public List<double> DistanceTwo;
        public List<double> DistanceThree;
        public List<double> DistanceFour;
        public List<double> IntensityOne;
        public List<double> IntensityTwo;
        public List<double> IntensityThree;
        public List<double> IntensityFour;
    };

    internal class DataAcquisition
    {
        private AsynchronousConnection _con;
        private int _dataAveraging = 1;

        public ConcurrentQueue<ChrDataSample> DataSamples { get; set; } = new ConcurrentQueue<ChrDataSample>();


        public DataAcquisition(AsynchronousConnection con)
        {
            _con = con;
        }

        private void CollectData(AsyncDataStatus status, Data data)
        {
            if (status <= AsyncDataStatus.Error)
            {
                Console.WriteLine($"*** data status {Lib.ErrorString((Int32)status)}");
                return;
            }
            ProcessDataSamples(data);
        }


        /// <summary>
        /// Sets the variables to prepare for a scan.
        /// </summary>
        public void PrepareStartScan()
        {
            DataSamples = new ConcurrentQueue<ChrDataSample>();
            _con.AutomaticMode = true;
            _con.SetDataCallback(CollectData);

            //store the data averaging, so when stopping the scan the Precitec device does not need to be accessed, and thus put no unnecessary load on it.
            {
                var exec = new CHRocodileLib.SynchronousCommandGroup(_con);
                int tokenAVR = exec.Add(Cmd.Query(CmdID.DataAverage)); // data averaging not supported by CLS2! will always return 1
                // exec.Add(Cmd.Command(CmdID.OutputSignals, 83, 16648, 16656, 16664, 16641, 16649, 16657, 16665));
                exec.Add(Cmd.Command(CmdID.OutputSignals, 83, 16640, 16641));
                exec.Add(Cmd.Command(CmdID.CCDRange, 0, 90)); // reduce range, enable higher frequency
                exec.Add(Cmd.Command(CmdID.ScanRate, 16000));
                //exec.Add(Cmd.Command(CmdID.DeviceTriggerMode, TriggerMode.TriggerWindow));
                var res = exec.WaitAndThrowOnError(); // wait for all commands to complete
                _dataAveraging = res[tokenAVR].GetParam<int>(0); // get average value from response to CmdID.DataAverage
                exec.Add(Cmd.FromStr("STR 1")); // software trigger to simulate machine
            }
            _con.StartDataStream();
        }

        /// <summary>
        /// Gets the relevant data from the data sample and adds them to the DataSamples property.
        /// </summary>
        /// <param name="data">The current data to convert.</param>
        private void ProcessDataSamples(Data data)
        {
            try
            {
                DataInfo info = data.Info;
                int channelCount = info.SignalGenInfo.ChannelCount;
                foreach (Data.Sample dataSample in data.Samples())
                {
                    ChrDataSample currentSample = new ChrDataSample();
                    int signalIndex = 0;
                    foreach ((SignalInfo, int) signalInfo in info.SignalInfos)
                    {
                        //Sample Counter
                        if (signalInfo.Item1.SignalID == 83)
                        {
                            currentSample.Counter = (int)dataSample.Get(signalIndex);
                        }
                        else if (signalInfo.Item1.SignalID == 256)
                        {
                            currentSample.DistanceOne = new List<double>();
                            currentSample.DistanceOne.Add(dataSample.Get(signalIndex));
                        }
                        else if (signalInfo.Item1.SignalID == 16640)
                        {
                            currentSample.DistanceOne = new List<double>();
                            for (int i = 0; i < channelCount; i++)
                            {
                                currentSample.DistanceOne.Add(dataSample.Get(signalIndex, i));
                            }

                            currentSample.DistanceOne.Reverse();
                        }
                        else if (signalInfo.Item1.SignalID == 16648)
                        {
                            currentSample.DistanceTwo = new List<double>();
                            for (int i = 0; i < channelCount; i++)
                            {
                                currentSample.DistanceTwo.Add(dataSample.Get(signalIndex, i));
                            }

                            currentSample.DistanceTwo.Reverse();
                        }
                        else if (signalInfo.Item1.SignalID == 16656)
                        {
                            currentSample.DistanceThree = new List<double>();
                            for (int i = 0; i < channelCount; i++)
                            {
                                currentSample.DistanceThree.Add(dataSample.Get(signalIndex, i));
                            }

                            currentSample.DistanceThree.Reverse();
                        }
                        else if (signalInfo.Item1.SignalID == 16664)
                        {
                            currentSample.DistanceFour = new List<double>();
                            for (int i = 0; i < channelCount; i++)
                            {
                                currentSample.DistanceFour.Add(dataSample.Get(signalIndex, i));
                            }

                            currentSample.DistanceFour.Reverse();
                        }
                        else if (signalInfo.Item1.SignalID == 257)
                        {
                            currentSample.IntensityOne = new List<double>();
                            currentSample.IntensityOne.Add(dataSample.Get(signalIndex));
                        }
                        else if (signalInfo.Item1.SignalID == 16641)
                        {
                            currentSample.IntensityOne = new List<double>();
                            for (int i = 0; i < channelCount; i++)
                            {
                                currentSample.IntensityOne.Add(dataSample.Get(signalIndex, i));
                            }

                            currentSample.IntensityOne.Reverse();
                        }
                        else if (signalInfo.Item1.SignalID == 16649)
                        {
                            currentSample.IntensityTwo = new List<double>();
                            for (int i = 0; i < channelCount; i++)
                            {
                                currentSample.IntensityTwo.Add(dataSample.Get(signalIndex, i));
                            }

                            currentSample.IntensityTwo.Reverse();
                        }
                        else if (signalInfo.Item1.SignalID == 16657)
                        {
                            currentSample.IntensityThree = new List<double>();
                            for (int i = 0; i < channelCount; i++)
                            {
                                currentSample.IntensityThree.Add(dataSample.Get(signalIndex, i));
                            }

                            currentSample.IntensityThree.Reverse();
                        }
                        else if (signalInfo.Item1.SignalID == 16665)
                        {
                            currentSample.IntensityFour = new List<double>();
                            for (int i = 0; i < channelCount; i++)
                            {
                                currentSample.IntensityFour.Add(dataSample.Get(signalIndex, i));
                            }

                            currentSample.IntensityFour.Reverse();
                        }

                        signalIndex++;
                    }

                    DataSamples.Enqueue(currentSample);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"ProcessDataSample:\t{e}");
            }
        }
    };

    internal class Program
    {
        private static int _counter = -1;

        static bool writeToFile(StreamWriter sw, ConcurrentQueue<ChrDataSample> ds)
        {
            bool receivedSamples = false;
            while (ds.TryDequeue(out ChrDataSample outData))
            {
                receivedSamples = true;
                if (_counter == -1)
                {
                    sw.WriteLine("Counter,DistanceOne,DistanceTwo,DistanceThree,DistanceFour,IntensityOne,IntensityTwo,IntensityThree,IntensityFour");
                    _counter = outData.Counter;
                }
                else
                {
                    if (outData.Counter != (_counter & 0xFFFF))
                    {
                        Console.WriteLine($"Missed sample: {_counter + 1}");
                        _counter = outData.Counter;
                    }
                }
                //sw.Write($"{outData.Counter},");
                _counter++;
            }
            return receivedSamples;
        }

        static void Main(string[] args)
        {
            using (var con = new AsynchronousConnection("192.168.170.3", DeviceType.MultiChannel, 128 * 1024 * 1024)) // 1 * 1024 * 1024 * 1024))
            using (StreamWriter sw = new StreamWriter("data.csv"))
            {
                var dataAcquition = new DataAcquisition(con);
                dataAcquition.PrepareStartScan();
                while (true)
                {
                    bool receivedSamples = writeToFile(sw, dataAcquition.DataSamples);
                    if (!receivedSamples)
                        Thread.Sleep(100);
                }
            }
        }
    }
}
