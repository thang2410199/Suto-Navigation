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
        /// Indicates the current memory pressure state of the app.
        /// </summary>
        MemoryReportArgs CurrentMemoryReport { get; set; }

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
        void FireMemoryNeeded(MemoryReportArgs report);

        /// <summary>
        /// Start watching memory to react
        /// </summary>
        void StartWatch();

        /// <summary>
        /// Stop watching memory
        /// </summary>
        void StopWatch();
    }


    /// <summary>
    /// Indicates the current memory pressure state of the app.
    ///
    /// On none we will do nothing, we are good.
    /// On low pressure we fire event when adding new panel
    /// On medium we fire event when adding new panel, remove low importance panels and fire event for each of them
    /// On high we fire event when adding new panel, remove normal or low importance panels and fire event for each of them
    /// </summary>
    public enum MemoryPressureStates
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }
}
