using NOnkyo.ISCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NOnkyo.Web.Controller
{
    public class VolumeController : ApiController
    {

        [HttpGet]
        public int GetMaxVolume()
        {
            return ContainerAccessor.Container.Resolve<IConnection>().CurrentDevice.MaxVolume;
        }

        [HttpGet]
        public int GetVolume()
        {
            return ISCP.Command.CommandBase.GetCommand<ISCP.Command.MasterVolume>().VolumeLevel;
        }

        [HttpGet]
        public IHttpActionResult SetVolumeUp()
        {
            var loConnection = ContainerAccessor.Container.Resolve<IConnection>();
            loConnection.SendCommand(ISCP.Command.MasterVolume.UpCommand());
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult SetVolumeDown()
        {
            var loConnection = ContainerAccessor.Container.Resolve<IConnection>();
            loConnection.SendCommand(ISCP.Command.MasterVolume.DownCommand());
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult SetVolume([FromUri] int volume)
        {
            var loConnection = ContainerAccessor.Container.Resolve<IConnection>();
            loConnection.SendCommand(ISCP.Command.MasterVolume.SetLevel(volume, loConnection.CurrentDevice));
            return Ok();
        }

    }
}
