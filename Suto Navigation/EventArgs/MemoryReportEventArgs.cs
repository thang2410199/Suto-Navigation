using SutoNavigation.NavigationService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SutoNavigation
{
    /// <summary>
    /// Use to send memory report
    /// </summary>
    public class MemoryReportArgs : EventArgs
    {
        public MemoryPressureStates CurrentPressure;
        public MemoryPressureStates LastPressure;
        public ulong CurrentMemoryUsage;
        public ulong AvailableMemory;
        public double UsagePercentage;
    }
}
