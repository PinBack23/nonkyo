using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class AudioMuting : CommandBase
    {
        public static readonly AudioMuting State = new AudioMuting()
        {
            CommandMessage = "AMTQSTN"
        };

        public static AudioMuting Chose(bool pbMute, Device poDevice)
        {
            return new AudioMuting()
            {
                CommandMessage = "AMT{0}".FormatWith(pbMute ? "01" : "00")
            };
        }

        #region Constructor / Destructor

        internal AudioMuting()
        { }

        #endregion

        public bool Mute { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1AMT(\w\w)");
            if (loMatch.Success)
            {
                this.Mute = loMatch.Groups[1].Value == "01";
                return true;
            }
            return false;
        }
    }
}
