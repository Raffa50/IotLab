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
        public string ShipName { get; set; }
        public IDictionary<string, string> Errors { get; set; }
        public IDictionary<string, CommandDescription> Commands { get; set; }
        public SoftwareInfo SoftwareInfo { get; set; }
    }
}
