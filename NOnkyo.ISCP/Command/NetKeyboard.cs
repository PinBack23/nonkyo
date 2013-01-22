using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class NetKeyboard : CommandBase
    {

        public static NetKeyboard Send(string psKeyboradInput, Device poDevice)
        {
            if (String.IsNullOrEmpty(psKeyboradInput))
                throw new ArgumentException("psKeyboradInput is null or empty.", "psKeyboradInput");
            if (psKeyboradInput.Length > 128)
                throw new ArgumentException("psKeyboradInput is greater than 128 letters.", "psKeyboradInput");

            return new NetKeyboard()
            {
                CommandMessage = "NKY{0}".FormatWith(psKeyboradInput)
            };
        }

        #region Constructor / Destructor

        internal NetKeyboard()
        { }

        #endregion

        public EKeyboardCategory Category { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1NKY(.*)");
            if (loMatch.Success)
            {
                this.Category = loMatch.Groups[1].Value.FromDescription<EKeyboardCategory>();
                return true;
            }
            return false;
        }
    }

}
