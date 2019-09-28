using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorHeatmap
{
    class CpuUsage : IDisposable
    {
        private readonly PerformanceCounter[] _idleTimeCounters;
        private bool _isDisposing = false;

        public CpuUsage()
        {
            var processorCategory = new PerformanceCounterCategory(@"Processor");
            var instanceNames = processorCategory.GetInstanceNames();
            var counters = new List<PerformanceCounter>();
            foreach (var instanceName in instanceNames)
            {
                if (int.TryParse(instanceName, out _))
                {
                    counters.Add(new PerformanceCounter(processorCategory.CategoryName, @"% Idle Time", instanceName));
                }
            }
            counters.Sort((pc1, pc2) => int.Parse(pc1.InstanceName).CompareTo(int.Parse(pc2.InstanceName)));
            _idleTimeCounters = counters.ToArray();
        }

        public int CoreCount
        {
            get => _idleTimeCounters.Length;
        }

        public float GetCoreUsage(int core)
        {
            if (_isDisposing)
                return 0;
            return 100.0f - _idleTimeCounters[core].NextValue();
        }

        public void Dispose()
        {
            if (_isDisposing)
                return;

            _isDisposing = true;
            foreach (var pc in _idleTimeCounters)
            {
                pc.Dispose();
            }
        }
    }
}
