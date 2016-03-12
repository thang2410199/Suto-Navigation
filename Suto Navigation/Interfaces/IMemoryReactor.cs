using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SutoNavigation.Interfaces
{
    public interface IMemoryReactor
    {
        void OnMemoryNeeded(object sender, MemoryReportArgs e);
    }
}
