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
    public class MasterVolume : CommandBase
    {
        public static readonly MasterVolume State = new MasterVolume() 
        { 
            CommandMessage = "MVLQSTN"
        };

        public static readonly MasterVolume UP = new MasterVolume()
        {
            CommandMessage = "MVLUP"
        };

        public static readonly MasterVolume DOWN = new MasterVolume()
        {
            CommandMessage = "MVLDOWN"
        };

        public static MasterVolume SetLevel(int pnLevel, Device poDevice)
        {
            if (pnLevel < poDevice.MinVolume || pnLevel > poDevice.MaxVolume)
                throw new ArgumentException("Volume-range is {0}-{1}".FormatWith(poDevice.MinVolume, poDevice.MaxVolume), "pnLevel");

            return new MasterVolume()
            {
                CommandMessage = "MVL{0}".FormatWith(pnLevel.ConverIntValueToHexString())
            };
        }

        #region Constructor / Destructor

        internal MasterVolume()
        { }

        #endregion

        public int VolumeLevel { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1MVL(\w\w)");
            if (loMatch.Success)
            {
                this.VolumeLevel = loMatch.Groups[1].Value.ConvertHexValueToInt();
                return true;
            }
            return false;
        }
    }
}
