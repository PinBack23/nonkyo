using NOnkyo.ISCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NOnkyo.Web.Controller
{
    public class AudioMutingController : ApiController
    {
        [HttpGet]
        public bool GetState()
        {
            return ISCP.Command.CommandBase.GetCommand<ISCP.Command.AudioMuting>().Mute;
        }

        [HttpGet]
        public IHttpActionResult MuteOn()
        {
            var loConnection = ContainerAccessor.Container.Resolve<IConnection>();
            loConnection.SendCommand(ISCP.Command.AudioMuting.Chose(true, loConnection.CurrentDevice));
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult MuteOff()
        {
            var loConnection = ContainerAccessor.Container.Resolve<IConnection>();
            loConnection.SendCommand(ISCP.Command.AudioMuting.Chose(false, loConnection.CurrentDevice));
            return Ok();
        }
    }
}