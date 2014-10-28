using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NOnkyo.ISCP.Command
{
    public class ZoneCommand : CommandBase
    {
        internal static ZoneCommand CurrentZoneCommand { get; set; }

        internal static EZone ZoneFromCurrentZoneCommand 
        { 
            get 
            {
                if (CurrentZoneCommand != null)
                    return CurrentZoneCommand.CurrentZone;
                return EZone.Zone1;
            } 
        }

        public static List<ZoneCommand> CeckSelectedZone = new List<ZoneCommand>()
        {
            new ZoneCommand() { CommandMessage = "ZPWQSTN" },
            new ZoneCommand() { CommandMessage = "PW3QSTN" },
            new ZoneCommand() { CommandMessage = "PW4QSTN" }
        };

        #region Attributes

        private bool mbZone1Enabeld = true;
        private bool mbZone2Enabeld = false;
        private bool mbZone3Enabeld = false;
        private bool mbZone4Enabeld = false;

        #endregion

        #region Constructor / Destructor

        internal ZoneCommand()
        {
            this.CurrentZone = EZone.Zone1;
        }

        #endregion

        public EZone CurrentZone { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            bool lbMatch = false;
            var loMatch = Regex.Match(psStatusMessage, @"!ZPW(\w\w)");
            if (loMatch.Success)
            {
                this.mbZone2Enabeld = loMatch.Groups[1].Value == "01";
                if (this.mbZone2Enabeld)
                    this.mbZone1Enabeld = this.mbZone3Enabeld = this.mbZone4Enabeld = false;
                lbMatch = true;
            }
            loMatch = Regex.Match(psStatusMessage, @"!PW3(\w\w)");
            if (loMatch.Success)
            {
                this.mbZone3Enabeld = loMatch.Groups[1].Value == "01";
                if (this.mbZone3Enabeld)
                    this.mbZone1Enabeld = this.mbZone2Enabeld = this.mbZone4Enabeld = false;
                lbMatch = true;
            }
            loMatch = Regex.Match(psStatusMessage, @"!PW4(\w\w)");
            if (loMatch.Success)
            {
                this.mbZone4Enabeld = loMatch.Groups[1].Value == "01";
                if (this.mbZone4Enabeld)
                    this.mbZone1Enabeld = this.mbZone2Enabeld = this.mbZone3Enabeld = false;
                lbMatch = true;
            }


            if (lbMatch && !this.mbZone1Enabeld && !this.mbZone2Enabeld && !this.mbZone3Enabeld && !this.mbZone4Enabeld)
            {
                this.mbZone1Enabeld = true;
            }



            return lbMatch;
        }
    }
}
