using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class Preset : CommandBase
    {
        public static readonly Preset State = new Preset()
        {
            CommandMessage = "PRSQSTN"
        };

        public static readonly Preset Up = new Preset()
        {
            CommandMessage = "PRSUP"
        };

        public static readonly Preset Down = new Preset()
        {
            CommandMessage = "PRSDOWN"
        };

        public static Preset Chose(int pnPresetNumber, Device poDevice)
        {
            if (pnPresetNumber < 1 || pnPresetNumber > 40)
                throw new ArgumentNullException("pnPresetNumber", "PresetNumber must between {0} and {1}".FormatWith(1, 40));
            return new Preset()
            {
                CommandMessage = "PRS{0}".FormatWith(pnPresetNumber.ConverIntValueToHexString())
            };
        }

        #region Constructor / Destructor

        internal Preset()
        { }

        #endregion

        public int CurrentPresetNumber { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1PRS(\w\w)");
            if (loMatch.Success)
            {
                this.CurrentPresetNumber = loMatch.Groups[1].Value.ConvertHexValueToInt();
                return true;
            }
            return false;
        }
    }
}
