using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NOnkyo.ISCP.Command
{
    public class Tuning : CommandBase
    {
        public static readonly Tuning State = new Tuning()
        {
            CommandMessage = "TUNQSTN"
        };

        //public static Tuning Chose(int pnChannel, Device poDevice)
        //{
        //    if (pnChannel < 0 || pnChannel > 9)
        //        throw new ArgumentNullException("pnChannel", "Channel must between {0} and {1}".FormatWith(0, 9));
        //    return new Tuning()
        //    {
        //        CommandMessage = "TUN{0}".FormatWith(pnChannel)
        //    };
        //}

        #region Constructor / Destructor

        internal Tuning()
        { }

        #endregion

        public string  CurrenChannel { get; private set; }

        public override bool Match(string psStatusMessage)
        {
            return false;   
        }
    }
}
