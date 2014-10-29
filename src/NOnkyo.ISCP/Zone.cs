using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP
{
    public static class Zone
    {
        private static EZone meCurrentZone = EZone.Zone1;
        public static EZone CurrentZone
        {
            get { return meCurrentZone; }
            set { meCurrentZone = value; }
        }
    }
}
