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
using System.IO;
using System.Drawing;

namespace NOnkyo.ISCP.Command
{
    public class NetJacketArt : CommandBase
    {

        #region Attributes

        private List<byte> moImageBytes;

        #endregion

        #region Constructor / Destructor

        internal NetJacketArt()
        { }

        #endregion

        public Byte[] Album { get; private set; }

        public bool IsBMP { get; private set; }

        public bool IsReady { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            var loMatch = Regex.Match(psStatusMessage, @"!1NJA(\d)(\d)(.*)");
            if (loMatch.Success)
            {
                bool lbImageComplete = false;
                switch (loMatch.Groups[2].Value)
                {
                    case "0": //Start
                        this.IsBMP = loMatch.Groups[1].Value == "0";
                        this.moImageBytes = new List<byte>();
                        this.IsReady = false;
                        break;
                    case "1": //Next
                        this.IsReady = false;
                        break;
                    case "2": //End
                        lbImageComplete = true;
                        break;
                    default:
                        break;
                }
                this.moImageBytes.AddRange(loMatch.Groups[3].Value.ConvertHexValueToByteArray());

                if (lbImageComplete)
                {
                    this.Album = this.moImageBytes.ToArray();
                    this.IsReady = true;
                }

                return lbImageComplete;
            }
            return false;
        }

        public void Clear()
        {
            this.Album = null;
            this.IsBMP = this.IsReady = false;
        }
    }
   
}
