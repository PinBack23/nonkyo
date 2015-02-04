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
    public class Tone : CommandBase
    {

        public static Tone StateCommand()
        {
            string lsCommandMessage = "TFRQSTN";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "ZTNQSTN";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "TN3QSTN";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "TN4QSTN";
                    break;
            }
            return new Tone()
            {
                CommandMessage = lsCommandMessage
            };
        }

        public static Tone TrebleUpCommand()
        {
            string lsCommandMessage = "TFRTUP";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "ZTNTUP";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "TN3TUP";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "TN4TUP";
                    break;
            }
            return new Tone()
            {
                CommandMessage = lsCommandMessage
            };
        }

        public static Tone TrebleDownCommand()
        {
            string lsCommandMessage = "TFRTDOWN";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "ZTNTDOWN";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "TN3TDOWN";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "TN4TDOWN";
                    break;
            }
            return new Tone()
            {
                CommandMessage = lsCommandMessage
            };
        }

        public static Tone SetTrebleLevel(int pnLevel)
        {

            string lsCommandMessage = "TFRT{0}";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "ZTNT{0}";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "TN3T{0}";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "TN4T{0}";
                    break;
            }
            return new Tone()
            {
                CommandMessage = lsCommandMessage.FormatWith(pnLevel.ConvertIntToDbValue())
            };
        }

        public static Tone BassUpCommand()
        {
            string lsCommandMessage = "TFRBUP";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "ZTNBUP";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "TN3BUP";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "TN4BUP";
                    break;
            }
            return new Tone()
            {
                CommandMessage = lsCommandMessage
            };
        }

        public static Tone BassDownCommand()
        {
            string lsCommandMessage = "TFRBDOWN";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "ZTNBDOWN";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "TN3BDOWN";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "TN4BDOWN";
                    break;
            }
            return new Tone()
            {
                CommandMessage = lsCommandMessage
            };
        }

        public static Tone SetBassLevel(int pnLevel)
        {

            string lsCommandMessage = "TFRB{0}";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsCommandMessage = "ZTNB{0}";
                    break;
                case EZone.Zone3:
                    lsCommandMessage = "TN3B{0}";
                    break;
                case EZone.Zone4:
                    lsCommandMessage = "TN4B{0}";
                    break;
            }
            return new Tone()
            {
                CommandMessage = lsCommandMessage.FormatWith(pnLevel.ConvertIntToDbValue())
            };
        }

        #region Constructor / Destructor

        internal Tone()
        { }

        #endregion

        public int? TrebleLevel { get; private set; }

        public int? BassLevel { get; private set; }

        public string TrebleDisplay { get; private set; }

        public string BassDisplay { get; private set; }

        public bool CanTrebleDown()
        {
            return this.TrebleLevel.GetValueOrDefault() > -10;
        }

        public bool CanTrebleUp()
        {
            return this.TrebleLevel.GetValueOrDefault() < 10;
        }

        public bool CanBassDown()
        {
            return this.BassLevel.GetValueOrDefault() > -10;
        }

        public bool CanBassUp()
        {
            return this.BassLevel.GetValueOrDefault() < 10;
        }

        public override bool Match(string psStatusMessage)
        {
            string lsMatchToken = "TFR";
            switch (Zone.CurrentZone)
            {
                case EZone.Zone2:
                    lsMatchToken = "ZTN";
                    break;
                case EZone.Zone3:
                    lsMatchToken = "TN3";
                    break;
                case EZone.Zone4:
                    lsMatchToken = "TN4";
                    break;
            }
            var loMatch = Regex.Match(psStatusMessage, @"!1{0}B(.\w)T(.\w)".FormatWith(lsMatchToken));
            if (loMatch.Success)
            {
                this.BassLevel = loMatch.Groups[1].Value.ConvertDbValueToInt();
                this.TrebleLevel = loMatch.Groups[2].Value.ConvertDbValueToInt();
                this.BassDisplay = this.BassLevel.ConvertDbIntValueToDisplay();
                this.TrebleDisplay = this.TrebleLevel.ConvertDbIntValueToDisplay();
                return true;
            }
            return false;
        }

    }
}
