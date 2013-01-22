using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace NOnkyo.ISCP
{
    public class Device
    {
        public string Category { get; set; }
        public string Model { get; set; }
        public int Port { get; set; }
        public string Area { get; set; }
        public string MacAddress { get; set; }
        public IPAddress IP { get; set; }

        public int MaxVolume { get; set; }
        public int MinVolume { get; set; }

        public override string ToString()
        {
            return "{0}: {1}".FormatWith(this.Model, this.IP);
        }
    }
}
