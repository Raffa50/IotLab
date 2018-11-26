using IotLab.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotLab.SimulatedDevice
{
    public class DeviceProperties
    {
        public SoftwareInfo SoftwareInfo { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public CommandDescription[] Commands { get; set; }
    }
}
