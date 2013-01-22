using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NOnkyo.ISCP.Command
{
    public class NetTune : CommandBase
    {

        public static NetTune Chose(ENetTuneOperation peOperation, Device poDevice)
        {
            return new NetTune()
            {
                CommandMessage = "NTC{0}".FormatWith(peOperation.ToDescription())
            };
        }

        #region Constructor / Destructor

        internal NetTune()
        { }

        #endregion

        public override bool Match(string psStatusMessage)
        {
            return false;
        }
    }
}
