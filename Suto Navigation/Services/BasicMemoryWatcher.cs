using SutoNavigation.Helpers;
using SutoNavigation.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SutoNavigation
{
    public class BasicMemoryWatcher : IMemoryWatcher
    {

        // These are the limits we use to define the memory pressure.
        // Expressed as % of memory available.
        const double memoryPressueLimit_None = 0.5;
        const double memoryPressueLimit_Low = 0.7;
        const double memoryPressueLimit_Medium = 0.85;

        // For low just do it every now and then to free up pages we don't need.
        // For medium and high to it more frequently.
        const int tickRollover_low = 200;
        const int tickRollover_medium_or_high = 10;

        public event EventHandler<MemoryReportArgs> OnMemoryNeeded
        {
            add
            {
                onMemoryNeeded.Add(value);
            }
            remove
            {
                onMemoryNeeded.Remove(value);
            }
        }
        SmartWeakEvent<EventHandler<MemoryReportArgs>> onMemoryNeeded = new SmartWeakEvent<EventHandler<MemoryReportArgs>>();

        public MemoryReportArgs CurrentMemoryReport { get; set; } = new MemoryReportArgs();

        bool shouldRun = false;
        object lockObj = new object();
        double m_cleanupTick = 0;
        public void StopWatch()
        {
            shouldRun = false;
        }

        public void StartWatch()
        {
            lock (lockObj)
            {
                // If already running, return.
                if (shouldRun)
                {
                    return;
                }
                shouldRun = true;
            }

            Task.Run(async () =>
            {
                // loop forever to get the correct 
                while (true)
                {
                    if (!shouldRun)
                        return;

                    // Sleep for some time, sleep longer if the memory pressure is lower.
                    int sleepTime = 100;
                    switch (CurrentMemoryReport.CurrentPressure)
                    {
                        case MemoryPressureStates.None:
                            sleepTime = 500;
                            break;
                        case MemoryPressureStates.Low:
                            sleepTime = 300;
                            break;
                        case MemoryPressureStates.Medium:
                        case MemoryPressureStates.High:
                            sleepTime = 100;
                            break;
                    }
                    await Task.Delay(sleepTime);

                    // Calculate the current memory pressure.
                    var memoryReport = CaculateCurrentMemory();

                    // If our new state is higher than our old state
                    if (memoryReport.CurrentPressure > memoryReport.LastPressure)
                    {
                        // We went up a state, notice others about that so they can react
                        FireMemoryNeeded(memoryReport);

                        // Set the count
                        m_cleanupTick = 0;
                    }
                    // If our new state is lower than our old state.
                    else if (memoryReport.CurrentPressure < memoryReport.LastPressure)
                    {
                        // We did well, but we can still be at medium or low, reset the counter.
                        m_cleanupTick = 0;
                    }
                    else
                    {
                        // Things are the same, if we are low or above take action.
                        if (memoryReport.CurrentPressure > MemoryPressureStates.Low)
                        {
                            // Count
                            m_cleanupTick++;

                            // Get the rollover count.
                            int tickRollover = memoryReport.CurrentPressure == MemoryPressureStates.Low ? tickRollover_low : tickRollover_medium_or_high;

                            // Check for roll over
                            if (m_cleanupTick > tickRollover)
                            {
                                FireMemoryNeeded(memoryReport);
                                m_cleanupTick = 0;
                            }
                        }
                    }
                }
            });
        }

        private MemoryReportArgs CaculateCurrentMemory()
        {
            var memoryReport = new MemoryReportArgs();
            memoryReport.AvailableMemory = Windows.System.MemoryManager.AppMemoryUsageLimit;
            memoryReport.CurrentMemoryUsage = Windows.System.MemoryManager.AppMemoryUsage;
            memoryReport.UsagePercentage = (double)memoryReport.CurrentMemoryUsage / (double)memoryReport.AvailableMemory;

            // Set the pressure state.
            memoryReport.LastPressure = CurrentMemoryReport.CurrentPressure;
            if (memoryReport.UsagePercentage < memoryPressueLimit_None)
            {
                memoryReport.CurrentPressure = MemoryPressureStates.None;
            }
            else if (memoryReport.UsagePercentage < memoryPressueLimit_Low)
            {
                memoryReport.CurrentPressure = MemoryPressureStates.Low;
            }
            else if (memoryReport.UsagePercentage < memoryPressueLimit_Medium)
            {
                memoryReport.CurrentPressure = MemoryPressureStates.Medium;
            }
            else
            {
                memoryReport.CurrentPressure = MemoryPressureStates.High;
            }

            return memoryReport;
        }

        public void FireMemoryNeeded(MemoryReportArgs report)
        {
            try
            {
                onMemoryNeeded.Raise(this, report);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to clean memory");
            }
        }

        public MemoryReportArgs GetMemoryState()
        {
            return CaculateCurrentMemory();
        }
    }
}
