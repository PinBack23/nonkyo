using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class NetPlayStatus : CommandBase
    {
        public static readonly NetPlayStatus State = new NetPlayStatus()
        {
            CommandMessage = "NSTQSTN"
        };

        #region Constructor / Destructor

        internal NetPlayStatus()
        { }

        #endregion

        public EPlayStatus PlayStatus { get; private set; }

        public ERepeatStatus RepeatStatus { get; private set; }

        public EShuffleStatus ShuffleStatus { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1NST(.)(.)(.)");
            if (loMatch.Success)
            {
                this.PlayStatus = loMatch.Groups[1].Value.FromDescription<EPlayStatus>();
                this.RepeatStatus = loMatch.Groups[2].Value.FromDescription<ERepeatStatus>();
                this.ShuffleStatus = loMatch.Groups[3].Value.FromDescription<EShuffleStatus>();
                return true;
            }
            return false;
        }
    }
}
