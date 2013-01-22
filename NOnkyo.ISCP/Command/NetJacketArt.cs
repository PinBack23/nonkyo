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
    }
   
}
