/*
 * This console demo is to demonstrate how to open an asynchronous connection to a CHRocodile 2 device,
 * send commands (either using command ID or pure command string), and collect data.
 */

using System;
using System.Threading;
using CHRocodileLib;

namespace TCHRLibAsyncConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //create asychronous connection, for other type of device, please use corresponding device type
                using (var con = new AsynchronousConnection("192.168.170.3", DeviceType.Chr2))
                {
                    //set connection to automatically process device output, 
                    //i.e. let CHRocodileLib to create an internal thread for output processing
                    //all the reponses and data are delivered through callback function withing CHRocodileLib internal thread 
                    con.AutomaticMode = true;

                    // Demonstration on how to send commands and receive responses asynchronously:
                    //set scan rate asynchronously, i.e. without waiting for the response. For this command, no callback function is set
                    con.Exec(CmdID.ScanRate, null, 4000.0);
                    // query scan rate, lambda routine is called on command completion:
                    con.Query(CmdID.ScanRate, rsp => Console.WriteLine($"Response: SHZ={rsp.GetParam<float>(0)}"));

                    Console.WriteLine("...now setting up measurement...");

                    // Demonstration on how to send commands and receive responses blocking - even in async mode:
                    // Set up device for measurement:
                    // request some signals: sample counter (ID 83) and distance 1 (ID 256),
                    // set the light intenisty to 20% and the scan rate to 6000 Hz:
                    string[] cmds = { "SODX 83 16640", "LAI 20", "SHZ 6000" };
                    // Send the commands above, wait for all responses, and check for errors:
                    var grp = new SynchronousCommandGroup(con);
                    foreach (string cmdString in cmds)
                        grp.Add(Cmd.FromStr(cmdString));
                    var responses = grp.WaitAll();
                    foreach (var res in responses)
                    {
                        Console.WriteLine($"{res.Value.ToString()} done.");
                        if (res.Value.IsError())
                            throw new Exception($"command {res.ToString()} failed.");
                    }

                    // now read in data:
                    const int total = 12000;
                    int count = total;
                    bool firstSample = true;
                    UInt16 shadowCounter = 0;

                    Console.WriteLine($"Setup done. Now measuring ({total} samples)...");

                    con.SetGeneralResponseCallback(rsp =>
                    {
                        // whenever some response or update occurs, this callback is called:
                        Console.WriteLine($"Message from sensor: {rsp.ToString()}");
                    });

                    // then register data callback function:
                    AsynchronousConnection.DataCallback dcb = (status, data) =>
                    {
                        var myData = data.DetachRenew(); // detach data from callback thread
                        foreach (var s in myData.Samples())
                        {
                            if (count <= 0)
                                break;

                            UInt16 sampleCounter = (UInt16)s.Get(0);
                            if (firstSample)
                            {
                                shadowCounter = sampleCounter;
                                firstSample = false;
                            }
                            else
                            {
                                shadowCounter++;
                                // check for missing samples
                                if (sampleCounter != shadowCounter)
                                {
                                    Console.WriteLine($"******** Missing samples: {shadowCounter} -> {sampleCounter}");
                                    shadowCounter = sampleCounter;
                                }
                            }
                            // gets 2nd signal of SODX request above (distance):
                            if (sampleCounter % 1000 == 0)
                            {
                                Console.WriteLine($"Sample Counter: {sampleCounter}");
                                string str = "";
                                for (int i = 0; i < 1200; i++)
                                {
                                    double d = s.Get(1, i);
                                    str += d.ToString("F0") + " ";
                                }
                                Console.WriteLine(str);
                            }
                            --count;
                        }
                    };
                    con.SetDataCallback(dcb, 100, 300);

                    while (count > 0) // wait for data collection to finish
                        Thread.Sleep(10);

                    con.Exec(CmdID.StopDataStream, null);

                    Console.WriteLine("Demo completed, press return.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failure: {ex.ToString()}");
            }
        }
    }
}
