using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class NetTimeInfo : CommandBase
    {
        public static readonly NetTimeInfo State = new NetTimeInfo()
        {
            CommandMessage = "NTMQSTN"
        };

        private const string IsCompleteReg = @"\d\d:\d\d";

        #region Constructor / Destructor

        internal NetTimeInfo()
        { }

        #endregion

        public string Elapsed { get; private set; }

        public string Total { get; private set; }

        public bool IsComplete { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1NTM(.*)/(.*)");
            if (loMatch.Success)
            {
                this.Elapsed = loMatch.Groups[1].Value;
                this.Total = loMatch.Groups[2].Value;
                this.IsComplete = Regex.Match(this.Elapsed, IsCompleteReg).Success &&
                    Regex.Match(this.Total, IsCompleteReg).Success;
                return true;
            }
            return false;
        }
    }
}
