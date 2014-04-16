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
    public class NLT : CommandBase
    {
        #region Constructor / Destructor

        internal NLT()
        { }

        #endregion

        public ENetworkServiceType NetworkSource { get; private set; }
        public string UiType { get; private set; }
        public string LayerInfo { get; private set; }
        public int CurrentCursorPosition { get; private set; }
        public int NumberOfListItems { get; private set; }
        public int NumberOfLayer { get; private set; }
        public string Reserved { get; set; }
        public string IconLeft { get; private set; }
        public string IconRight { get; private set; }
        public string StatusInfo { get; private set; }
        public string CurrentTitle { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1NLT(\w{2})(\w{1})(\w{1})(\w{4})(\w{4})(\w{2})(\w{2})(\w{2})(\w{2})(\w{2})(.*)");
            if (loMatch.Success)
            {
                this.NetworkSource = loMatch.Groups[1].Value.ConvertHexValueToInt().ToEnum<ENetworkServiceType>(ENetworkServiceType.None);
                this.UiType = loMatch.Groups[2].Value;
                this.LayerInfo = loMatch.Groups[3].Value;
                this.CurrentCursorPosition = loMatch.Groups[4].Value.ConvertHexValueToInt();
                this.NumberOfListItems = loMatch.Groups[5].Value.ConvertHexValueToInt();
                this.NumberOfLayer = loMatch.Groups[6].Value.ConvertHexValueToInt();
                this.Reserved = loMatch.Groups[7].Value;
                this.IconLeft = loMatch.Groups[8].Value;
                this.IconRight = loMatch.Groups[9].Value;
                this.StatusInfo = loMatch.Groups[10].Value;
                this.CurrentTitle = loMatch.Groups[11].Value;
                return true;
            }
            return false;
        }
    }
}
