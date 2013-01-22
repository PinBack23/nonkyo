using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class Power : CommandBase
    {

        public static readonly Power State = new Power()
        {
            CommandMessage = "PWRQSTN"
        };

        public static Power On(Device poDevice)
        {
            return new Power()
            {
                CommandMessage = "PWR01"
            };
        }

        public static Power Off(Device poDevice)
        {
            return new Power()
            {
                CommandMessage = "PWR00"
            };
        }

        #region Constructor / Destructor

        internal Power()
        { }

        #endregion

        public bool IsOn { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1PWR(.*)");
            if (loMatch.Success)
            {
                this.IsOn = loMatch.Groups[1].Value == "01";
                return true;
            }
            return false;
        }
    }

}
