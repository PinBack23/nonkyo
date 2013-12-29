#region License
/*Copyright (c) 2013, Karl Sparwald
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that 
the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following 
disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the 
following disclaimer in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
OF THE POSSIBILITY OF SUCH DAMAGE.*/
#endregion

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
                CommandMessage = "NLSI{0:00000}".FormatWith(pnIndex)
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
