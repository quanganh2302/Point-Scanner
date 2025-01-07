/*
This demo is a simple console demo to demonstrate, how to create a synchronous connection with CHRocodile² device
and then send commands (either using command ID or pure command string) and collecting data.
 */

using System;
using System.Threading;
using CHRocodileLib;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;


namespace TCHRLibBasicConsole
{
    internal class Program
    {
        public static double GetStandardDeviation(List<long> numbers)
        {
            double avg = numbers.Average();
            double sumOfSquaresOfDifferences = numbers.Select(val => (val - avg) * (val - avg)).Sum();
            return Math.Sqrt(sumOfSquaresOfDifferences / numbers.Count);
        }

        static void RetrieveSamples(SynchronousConnection con, bool useGetForAllChannels)
        {
            const int total = 100000;

            var data = con.GetNextSamples(); // get first chunk of samples

            int channelCount = data.Info.SignalGenInfo.ChannelCount; // should be 1200
            int count = 0;
            List<long> timeSpans = new();
            double[] profiles = new double[channelCount * total];
            int profileBase = 0; // start index of next profile
            Stopwatch stopwatch = new Stopwatch();
            int sampleCounter = 0;
            while (true)
            {
                long chunkSampleCount = Math.Min(total - count, data.NumSamples);

                if (chunkSampleCount > 0)
                {
                    stopwatch.Restart();

                    // two alternaties: use GetOfAllChannels or Get
                    if (useGetForAllChannels)
                    {
                        // the "hot" part of the code: copy samples to profiles array
                        foreach (var s in data.Samples())
                        {
                            int sc = (int)s.Get(0);
                            if (sc != sampleCounter)
                            {
                                Console.WriteLine($"Sample counter: {sc} instead of {sampleCounter}");
                                sampleCounter = sc;
                            }
                            sampleCounter++;

                            double[] sample = s.GetOfAllChannels(1);
                            Array.Copy(sample, 0, profiles, profileBase, sample.Length);
                            profileBase += channelCount;
                            if (++count >= total)
                                break;
                        }
                    }
                    else
                    {
                        // the "hot" part of the code: copy samples to profiles array
                        foreach (var s in data.Samples())
                        {
                            for (int i = 0; i < channelCount; i++)
                                profiles[profileBase++] = s.Get(1, i);
                            if (++count >= total)
                                break;
                        }
                    }


                    stopwatch.Stop();
                    TimeSpan timeTaken = stopwatch.Elapsed;
                    long timeTakenUs = timeTaken.Ticks / chunkSampleCount / 10; // microseconds per sample
                    timeSpans.Add(timeTakenUs);

                    // write out a subset of runs:
                    if (count % 16 == 0)
                        Console.WriteLine($"Read {data.NumSamples} samples, time: {timeTakenUs}µs/sample");
                    if (count >= total)
                        break;
                }
                else
                    Thread.Sleep(10); // wait if none have been available yet
                data = con.GetNextSamples(); // get next available samples
            }

            Console.WriteLine($"Finished.");
            double stdDev = GetStandardDeviation(timeSpans);
            Console.WriteLine($"Statistics: avr time per sample: {timeSpans.Average():F1}µs, stdDev={stdDev:F1}µs.");
            Console.WriteLine($"Equivalent sampe rate = about {1.0e6 / (timeSpans.Average() + stdDev):F0} samples/s.");
        }

        static void Main(string[] args)
        {
            //create sychronous connection, for other type of device, please use corresponding device type
            using (var con = new SynchronousConnection("192.168.170.3", DeviceType.MultiChannel))
            {
                try
                {
                    //set scan rate
                    con.Exec(CmdID.ScanRate, 5000.0);
                    con.Exec(CmdID.LampIntensity, 100.0);
                    //query scan rate
                    var response = con.Query(CmdID.ScanRate); // throws on error
                    Console.WriteLine($"The current scan rate is {response.Params[0]}Hz.");

                    con.ExecString("SODX 83 16640");

                    Console.WriteLine("Test Array: ");
                    RetrieveSamples(con, true);
                    Console.WriteLine("Test Single: ");
                    RetrieveSamples(con, false);

                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failure: {ex.ToString()}");
                    try
                    {
                        var response = con.ExecString("GLE");
                        Console.WriteLine($"Error: {response.ToString()}");
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                    Console.ReadLine();
                }
            }
        }
    }
}
