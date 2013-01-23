using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class NetArtistName : CommandBase
    {
        public static readonly NetArtistName State = new NetArtistName()
        {
            CommandMessage = "NATQSTN"
        };

        #region Constructor / Destructor

        internal NetArtistName()
        { }

        #endregion

        public string Info { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1NAT(.*)");
            if (loMatch.Success)
            {
                string lsInfo = loMatch.Groups[1].Value;
                this.Info = (lsInfo == ISCPDefinitions.EmptyNetInfo) ? string.Empty : lsInfo;
                return true;
            }
            return false;
        }
    }
}
