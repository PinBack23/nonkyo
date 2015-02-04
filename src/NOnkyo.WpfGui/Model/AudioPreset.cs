using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NOnkyo.WpfGui.Model
{
    public class AudioPreset
    {
        public int MasterVolume { get; set; }
        public int? BassLevel { get; set; }
        public int? TrebleLevel { get; set; }
        public int? CenterLevel { get; set; }
        public int? SubwooferLevel { get; set; }
    }
}
