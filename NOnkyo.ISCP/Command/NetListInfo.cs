using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class NetListInfo : CommandBase
    {
        public static NetListInfo ChoseLine(int pnLine, Device poDevice)
        {
            return new NetListInfo()
            {
                CommandMessage = "NLSL{0}".FormatWith(pnLine)
            };
        }

        public static NetListInfo ChoseIndex(int pnIndex, Device poDevice)
        {
            return new NetListInfo()
            {
                CommandMessage = "NLSI{0}".FormatWith(pnIndex)
            };
        }

        #region Constructor / Destructor

        internal NetListInfo()
        {
            this.InfoList = new List<NetListItem>();
        }

        public int CursorPosition { get; private set; }

        public List<NetListItem> InfoList { get; private set; }

        #endregion

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1NLSC(\d)([CP])");
            if (loMatch.Success)
            {
                this.CursorPosition = Convert.ToInt32(loMatch.Groups[1].Value);
                if (loMatch.Groups[2].Value == "P") //Page Update
                    this.InfoList.Clear();
                return true;
            }
            loMatch = Regex.Match(psStatusMessage, @"!1NLSC([-])P");
            if (loMatch.Success)
            {
                this.CursorPosition = 0;
                this.InfoList.Clear();
                return true;
            }
            loMatch = Regex.Match(psStatusMessage, @"!1NLS([UA])(\d)-(.*)");
            if (loMatch.Success)
            {
                this.AddInfoItem(loMatch);
                return true;
            }
            return false;
        }

        private void AddInfoItem(Match poMatch)
        {
            bool lbIsUnicode = poMatch.Groups[1].Value == "U";
            int lnLine = Convert.ToInt32(poMatch.Groups[2].Value);
            string lsItemName = poMatch.Groups[3].Value;

            //Message is now unicode-utf8
            this.InfoList.RemoveAll(item => item.Line == lnLine);
            this.InfoList.Add(new NetListItem(lnLine, lsItemName));
        }
    }
}
