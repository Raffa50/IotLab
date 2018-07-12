using System;
using System.Collections.Generic;
using System.Text;

namespace IotLab.Common
{
    [Serializable]
    public class SoftwareInfo
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public string Manufacturer { get; set; }

        public override string ToString()
        {
            return Name + " v" + Version;
        }
    }
}
