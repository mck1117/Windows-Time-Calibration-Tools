using System;
using System.Collections.Generic;
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
            long qpcFrequency;

            if (NativeMethods.QueryPerformanceFrequency(out qpcFrequency) == false)
            {
                return 0;
            }

            return qpcFrequency << 10;
        }

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: {0} method interval count\n", System.AppDomain.CurrentDomain.FriendlyName);

                Console.WriteLine("Reports measured latency, in seconds, of retrieving the time given the specified method.\nSampled 'count' times, with 'interval' ms delay between samples.\n");
                
                Console.WriteLine("\nMeasurement methods:");
                Console.WriteLine("\t1 - rdtsc");
                Console.WriteLine("\t2 - QueryPerformanceCounter via P/Invoke");
                Console.WriteLine("\t3 - GetSystemTimePreciseAsFileTime via P/Invoke");
                Console.WriteLine("\t4 - DateTime.Now");
                Console.WriteLine("\t5 - DateTime.UtcNow");

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

            // Array of different measurements we can make
            Func<double>[] funcs =  {
                                        MeasureRdtsc,
                                        MeasureQueryPerformanceCounter,
                                        MeasureGetSystemTimePreciseAsFileTime,
                                        MeasureDateTimeNow,
                                        MeasureDateTimeUtcNow
                                    };

            TSC_Frequency = GetTscFrequency();

            for (int i = 0; i < count; i++)
            {
                Thread.Sleep(delay);

                double measurement = funcs[method]();

                Console.WriteLine(measurement);
            }

            Console.WriteLine(MeasureQueryPerformanceCounter());
        }
    }
}
