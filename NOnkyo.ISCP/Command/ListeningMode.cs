using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class ListeningMode : CommandBase
    {
        public static readonly ListeningMode State = new ListeningMode()
        {
            CommandMessage = "LMDQSTN"
        };

        public static readonly ListeningMode MovieUP = new ListeningMode()
        {
            CommandMessage = "LMDMOVIE"
        };

        public static readonly ListeningMode MusicUP = new ListeningMode()
        {
            CommandMessage = "LMDMUSIC"
        };

        public static readonly ListeningMode GameUP = new ListeningMode()
        {
            CommandMessage = "LMDGAME"
        };

        public static ListeningMode Chose(EListeningMode peListeningMode, Device poDevice)
        {
            return new ListeningMode()
            {
                CommandMessage = "LMD{0}".FormatWith(((int)peListeningMode).ConverIntValueToHexString())
            };
        }

        #region Constructor / Destructor

        internal ListeningMode()
        { }

        #endregion

        public EListeningMode CurrentListeningMode { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1LMD(\w\w)");
            if (loMatch.Success)
            {
                this.CurrentListeningMode = loMatch.Groups[1].Value.ConvertHexValueToInt().ToEnum<EListeningMode>();
                return true;
            }
            return false;   
        }
    }
}
