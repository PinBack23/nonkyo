using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class NLT : CommandBase
    {
        #region Constructor / Destructor

        internal NLT()
        { }

        #endregion

        public int NetworkSource { get; private set; }
        public int MenuDepth { get; private set; }
        public int SelectedItem { get; private set; }
        public int TotalItems { get; private set; }
        public long NetworkIcon { get; private set; }
        public string CurrentTitle { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1NLT(\w{2})(\w{2})(\w{4})(\w{4})(\w{8})\w{2}(.*)");
            if (loMatch.Success)
            {
                this.NetworkSource = loMatch.Groups[1].Value.ConvertHexValueToInt();
                this.MenuDepth = loMatch.Groups[2].Value.ConvertHexValueToInt();
                this.SelectedItem = loMatch.Groups[3].Value.ConvertHexValueToInt();
                this.TotalItems = loMatch.Groups[4].Value.ConvertHexValueToInt();
                this.NetworkIcon = loMatch.Groups[5].Value.ConvertHexValueToLong();
                this.CurrentTitle = loMatch.Groups[6].Value;
                return true;
            }
            return false;
        }
    }
}
