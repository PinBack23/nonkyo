using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class NetTrackInfo : CommandBase
    {
        public static readonly NetTrackInfo State = new NetTrackInfo()
        {
            CommandMessage = "NTRQSTN"
        };

        #region Constructor / Destructor

        internal NetTrackInfo()
        { }

        #endregion

        public int CurrentTrack { get; private set; }

        public int TotalTrack { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1NTR(\d{4})/(\d{4})");
            if (loMatch.Success)
            {
                this.CurrentTrack = Convert.ToInt32(loMatch.Groups[1].Value);
                this.TotalTrack = Convert.ToInt32(loMatch.Groups[2].Value);
                return true;
            }
            return false;
        }
    }
}
