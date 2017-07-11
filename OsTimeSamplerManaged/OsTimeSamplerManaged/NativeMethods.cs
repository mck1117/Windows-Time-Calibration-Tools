using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace OsTimeSamplerManaged
{
    using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

    public static class NativeMethods
    {

        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpPerformanceCount);

        [DllImport("Intrinsics.dll")]
        public static extern ulong RdTsc();

        [DllImport("kernel32.dll")]
        public static extern void GetSystemTimePreciseAsFileTime(out FILETIME lpSystemTimeAsFileTime);
    }
}
