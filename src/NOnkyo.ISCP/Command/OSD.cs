using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NOnkyo.ISCP.Command
{
    public class OSD : CommandBase
    {
        public static readonly OSD Menu = new OSD()
        {
            CommandMessage = "OSDMENU"
        };

        public static OSD Chose(EOSDOperation peOSDOperation, Device poDevice)
        {
            return new OSD()
            {
                CommandMessage = "OSD{0}".FormatWith(peOSDOperation.ToString())
            };
        }

        #region Constructor / Destructor

        internal OSD()
        { }

        #endregion

        public override bool Match(string psStatusMessage)
        {
            return false;   
        }
    }
}
