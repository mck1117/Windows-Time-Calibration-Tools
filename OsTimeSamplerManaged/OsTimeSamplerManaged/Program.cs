using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OsTimeSamplerManaged
{
    class Program
    {
        private static double TSC_Frequency;

        private static double MeasureRdtsc()
        {
            // make two back to back rdtsc calls
            ulong rdtsc1 = NativeMethods.RdTsc();
            ulong rdtsc2 = NativeMethods.RdTsc();

            ulong delta = rdtsc2 - rdtsc1;

            return delta / TSC_Frequency;
        }

        private static double MeasureQueryPerformanceCounter()
        {
            long pc = 0;

            // make two back to back rdtsc calls, with a QPC call in the middle
            ulong rdtsc1 = NativeMethods.RdTsc();

            NativeMethods.QueryPerformanceCounter(out pc);

            ulong rdtsc2 = NativeMethods.RdTsc();


            ulong delta = rdtsc2 - rdtsc1;

            return delta / TSC_Frequency;
        }

        private static double MeasureGetSystemTimePreciseAsFileTime()
        {
            System.Runtime.InteropServices.ComTypes.FILETIME ft;

            // make two back to back rdtsc calls, with a GSTPAFT call in the middle
            ulong rdtsc1 = NativeMethods.RdTsc();

            NativeMethods.GetSystemTimePreciseAsFileTime(out ft);

            ulong rdtsc2 = NativeMethods.RdTsc();


            ulong delta = rdtsc2 - rdtsc1;

            return delta / TSC_Frequency;
        }

        private static double MeasureGetSystemTimeAsFileTime()
        {
            System.Runtime.InteropServices.ComTypes.FILETIME ft;

            // make two back to back rdtsc calls, with a GSTAFT call in the middle
            ulong rdtsc1 = NativeMethods.RdTsc();

            NativeMethods.GetSystemTimeAsFileTime(out ft);

            ulong rdtsc2 = NativeMethods.RdTsc();


            ulong delta = rdtsc2 - rdtsc1;

            return delta / TSC_Frequency;
        }

        private static double MeasureDateTimeNow()
        {
            // make two back to back rdtsc calls, with a DateTime.Now call in the middle
            ulong rdtsc1 = NativeMethods.RdTsc();

            DateTime t = DateTime.Now;

            ulong rdtsc2 = NativeMethods.RdTsc();


            ulong delta = rdtsc2 - rdtsc1;

            return delta / TSC_Frequency;
        }

        private static double MeasureDateTimeUtcNow()
        {
            // make two back to back rdtsc calls, with a DateTime.UtcNow call in the middle
            ulong rdtsc1 = NativeMethods.RdTsc();

            DateTime t = DateTime.UtcNow;

            ulong rdtsc2 = NativeMethods.RdTsc();


            ulong delta = rdtsc2 - rdtsc1;

            return delta / TSC_Frequency;
        }

        private static double GetTscFrequency()
        {
            // To determine the TSC frequency, we take two samples that are
            // about 1 second apart, and measure the actual elapsed time with
            // the Stopwatch.  A little bit of division, and we've calculated
            // the TSC frequency.

            Stopwatch sw = new Stopwatch();
            // Start the sw and take the first TSC sample
            sw.Start();
            ulong rdtsc1 = NativeMethods.RdTsc();

            Thread.Sleep(1000);

            // Stop the sw and take the second TSC sample
            sw.Stop();
            ulong rdtsc2 = NativeMethods.RdTsc();

            // Compute frequency
            double frequency = (rdtsc2 - rdtsc1) / sw.Elapsed.TotalSeconds;

            // Round to nearest 10 mhz
            frequency = Math.Round(frequency / 1e7) * 1e7;

            // Sanity check for dubious CPU frequency
            if (frequency > 6e9 || frequency < 8e7)
            {
                Console.Error.WriteLine("WARNING: Unusual CPU frequency, {0} GHz", frequency / 1e9);
            }

            return frequency;
        }

        // Array of different measurements we can make
        private static readonly Func<double>[] funcs =  {
                                                            MeasureRdtsc,
                                                            MeasureQueryPerformanceCounter,
                                                            MeasureGetSystemTimePreciseAsFileTime,
                                                            MeasureGetSystemTimeAsFileTime,
                                                            MeasureDateTimeNow,
                                                            MeasureDateTimeUtcNow
                                                        };

        private static readonly string[] funcNames =    {
                                                           "rdtsc",
                                                           "QueryPerformanceCounter via P/Invoke",
                                                           "GetSystemTimePreciseAsFileTime via P/Invoke",
                                                           "GetSystemTimeAsFileTime via P/Invoke",
                                                           "DateTime.Now",
                                                           "DateTime.UtcNow"
                                                        };

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: {0} method interval count\n", System.AppDomain.CurrentDomain.FriendlyName);

                Console.WriteLine("Reports measured latency, in seconds, of retrieving the time given the specified method.\nSampled 'count' times, with 'interval' ms delay between samples.\n");

                Console.WriteLine("\nMeasurement methods:");

                for (int i = 0; i < funcNames.Length; i++)
                {
                    Console.WriteLine("\t{0} - {1}", i, funcNames[i]);
                }

                Console.WriteLine();

                return;
            }

            int method, count, delay;

            // Parse command line arguments
            try
            {
                method = int.Parse(args[0]);
                delay = int.Parse(args[1]);
                count = int.Parse(args[2]);
            }
            catch
            {
                Console.WriteLine("Error parsing arguments");
                return;
            }

            // Get the TSC frequency, for interval measurement
            TSC_Frequency = GetTscFrequency();

            for (int i = 0; i < count; i++)
            {
                Thread.Sleep(delay);

                double measurement = funcs[method]();

                Console.WriteLine(measurement);
            }
        }
    }
}
