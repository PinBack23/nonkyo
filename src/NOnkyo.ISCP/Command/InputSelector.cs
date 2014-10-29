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
    public class InputSelector : CommandBase
    {
        public static InputSelector StateCommand()
        {
            string lsCommandMessage = "SLIQSTN";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "SLZQSTN";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "SL3QSTN";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "SL4QSTN";
                    break;
            }
            return new InputSelector()
            {
                CommandMessage = lsCommandMessage
            };
        }

        public static InputSelector Chose(EInputSelector peInputSelector, Device poDevice)
        {
            string lsCommandMessage = "SLI{0}";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "SLZ{0}";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "SL3{0}";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "SL4{0}";
                    break;
            }
            return new InputSelector()
            {
                CommandMessage = lsCommandMessage.FormatWith(((int)peInputSelector).ConverIntValueToHexString())
            };
        }

        #region Constructor / Destructor

        internal InputSelector()
        { }

        #endregion

        public EInputSelector CurrentInputSelector { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            string lsMatchToken = "SLI";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsMatchToken = "SLZ";
                    break;
                case EZone.Zone3:
                    lsMatchToken = "SL3";
                    break;
                case EZone.Zone4:
                    lsMatchToken = "SL4";
                    break;
            }
            var loMatch = Regex.Match(psStatusMessage, @"!1{0}(\w\w)".FormatWith(lsMatchToken));
            if (loMatch.Success)
            {
                this.CurrentInputSelector = loMatch.Groups[1].Value.ConvertHexValueToInt().ToEnum<EInputSelector>();
                return true;
            }
            return false;   
        }
    }
}
