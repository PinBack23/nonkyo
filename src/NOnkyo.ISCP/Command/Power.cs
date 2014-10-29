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
    public class Power : CommandBase
    {

        public static Power StateCommand()
        {
            string lsCommandMessage = "PWRQSTN";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "ZPWQSTN";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "PW3QSTN";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "PW4QSTN";
                    break;
            }
            return new Power()
            {
                CommandMessage = lsCommandMessage
            };
        }

        public static Power On(Device poDevice)
        {
            string lsCommandMessage = "PWR01";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "ZPW01";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "PW301";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "PW401";
                    break;
            }
            return new Power()
            {
                CommandMessage = lsCommandMessage
            };
        }

        public static Power Off(Device poDevice)
        {
            string lsCommandMessage = "PWR00";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "ZPW00";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "PW300";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "PW400";
                    break;
            }
            return new Power()
            {
                CommandMessage = lsCommandMessage
            };
        }

        #region Constructor / Destructor

        internal Power()
        { }

        #endregion

        public bool IsOn { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            string lsMatchToken = "PWR";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsMatchToken = "ZPW";
                    break;
                case EZone.Zone3:
                    lsMatchToken = "PW3";
                    break;
                case EZone.Zone4:
                    lsMatchToken = "PW4";
                    break;
            }
            var loMatch = Regex.Match(psStatusMessage, @"!1{0}(.*)".FormatWith(lsMatchToken));
            if (loMatch.Success)
            {
                this.IsOn = loMatch.Groups[1].Value == "01";
                return true;
            }
            return false;
        }
    }

}
