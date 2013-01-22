using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class NetAlbumName : CommandBase
    {
        public static readonly NetAlbumName State = new NetAlbumName()
        {
            CommandMessage = "NALQSTN"
        };

        #region Constructor / Destructor

        internal NetAlbumName()
        { }

        #endregion

        public string Info { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1NAL(.*)");
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
