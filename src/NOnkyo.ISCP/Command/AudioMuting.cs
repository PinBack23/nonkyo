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
    public class AudioMuting : CommandBase
    {
        public static AudioMuting StateCommand()
        {
            string lsCommandMessage = "AMTQSTN";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "ZMTQSTN";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "MT3QSTN";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "MT4QSTN";
                    break;
            }
            return new AudioMuting()
            {
                CommandMessage = lsCommandMessage
            };
        }

        public static AudioMuting Chose(bool pbMute, Device poDevice)
        {
            string lsCommandMessage = "AMT{0}";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "ZMT{0}";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "MT3{0}";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "MT4{0}";
                    break;
            }
            return new AudioMuting()
            {
                CommandMessage = lsCommandMessage.FormatWith(pbMute ? "01" : "00")
            };
        }

        #region Constructor / Destructor

        internal AudioMuting()
        { }

        #endregion

        public bool Mute { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            string lsMatchToken = "AMT";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsMatchToken = "ZMT";
                    break;
                case EZone.Zone3:
                    lsMatchToken = "MT3";
                    break;
                case EZone.Zone4:
                    lsMatchToken = "MT4";
                    break;
            }
            var loMatch = Regex.Match(psStatusMessage, @"!1{0}(\w\w)".FormatWith(lsMatchToken));
            if (loMatch.Success)
            {
                this.Mute = loMatch.Groups[1].Value == "01";
                return true;
            }
            return false;
        }
    }
}
