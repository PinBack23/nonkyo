using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class MasterVolume : CommandBase
    {
        public static readonly MasterVolume State = new MasterVolume() 
        { 
            CommandMessage = "MVLQSTN"
        };

        public static readonly MasterVolume UP = new MasterVolume()
        {
            CommandMessage = "MVLUP"
        };

        public static readonly MasterVolume DOWN = new MasterVolume()
        {
            CommandMessage = "MVLDOWN"
        };

        public static MasterVolume SetLevel(int pnLevel, Device poDevice)
        {
            if (pnLevel < poDevice.MinVolume || pnLevel > poDevice.MaxVolume)
                throw new ArgumentException("Volume-range is {0}-{1}".FormatWith(poDevice.MinVolume, poDevice.MaxVolume), "pnLevel");

            return new MasterVolume()
            {
                CommandMessage = "MVL{0}".FormatWith(pnLevel.ConverIntValueToHexString())
            };
        }

        #region Constructor / Destructor

        internal MasterVolume()
        { }

        #endregion

        public int VolumeLevel { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1MVL(\w\w)");
            if (loMatch.Success)
            {
                this.VolumeLevel = loMatch.Groups[1].Value.ConvertHexValueToInt();
                return true;
            }
            return false;
        }
    }
}
