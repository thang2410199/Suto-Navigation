using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SutoNavigation.Interfaces
{
    public interface IMemoryWatcher
    {
        /// <summary>
        /// Fire when memory state jump up OR when memory in critical state
        /// </summary>
        event EventHandler<MemoryReportArgs> OnMemoryNeeded;

        /// <summary>
        /// Get current status of memory
        /// </summary>
        /// <returns></returns>
        MemoryReportArgs GetMemoryState();

        /// <summary>
        /// Fires a memory report off to all who are listening.
        /// </summary>
        /// <param name="usedMemory"></param>
        /// <param name="memoryLimit"></param>
        /// <param name="usedPercentage"></param>
        void FireMemoryNeeded(ulong usedMemory, ulong memoryLimit, double usedPercentage);
    }
}
