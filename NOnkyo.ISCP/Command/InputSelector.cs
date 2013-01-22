using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class InputSelector : CommandBase
    {
        public static readonly InputSelector State = new InputSelector()
        {
            CommandMessage = "SLIQSTN"
        };

        public static InputSelector Chose(EInputSelector peInputSelector, Device poDevice)
        {
            return new InputSelector()
            {
                CommandMessage = "SLI{0}".FormatWith(((int)peInputSelector).ConverIntValueToHexString())
            };
        }

        #region Constructor / Destructor

        internal InputSelector()
        { }

        #endregion


        public EInputSelector CurrentInputSelector { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1SLI(\w\w)");
            if (loMatch.Success)
            {
                this.CurrentInputSelector = loMatch.Groups[1].Value.ConvertHexValueToInt().ToEnum<EInputSelector>();
                return true;
            }
            return false;   
        }
    }
}
