using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotLab.SimulatedDevice
{
    public class CommandDescription
    {
        public string Name { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
        public string Description { get; set; }
    }
}
