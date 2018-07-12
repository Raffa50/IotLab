using System;
using System.Collections.Generic;

namespace IotLab.Common
{
    [Serializable]
    public class C2DMessage
    {
        public string From { get; set; }
        public string Content { get; set; }
        public DateTime SentOn { get; private set; }

        public C2DMessage() {
            SentOn = DateTime.Now;
        }
    }
}
