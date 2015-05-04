using NOnkyo.ISCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NOnkyo.WpfGui.Controller
{
    public class PowerController : ApiController
    {
        [HttpGet]
        public string GetState()
        {
            if (ISCP.Command.CommandBase.GetCommand<ISCP.Command.Power>().IsOn)
                return "ON";
            return "OFF";
        }

        [HttpGet]
        public IHttpActionResult PowerOn()
        {
            var loConnection = App.Container.Resolve<IConnection>();
            loConnection.SendCommand(ISCP.Command.Power.On(loConnection.CurrentDevice));
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult PowerOff()
        {
            var loConnection = App.Container.Resolve<IConnection>();
            loConnection.SendCommand(ISCP.Command.Power.Off(loConnection.CurrentDevice));
            return Ok();
        }
    }
}
