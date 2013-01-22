using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NOnkyo.ISCP;

namespace NOnkyo.WpfGui.ViewModels
{
    public class SelectDeviceEventArgs : EventArgs
    {
        public Device Device { get; set; }
    }
}
