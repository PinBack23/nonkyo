using NOnkyo.ISCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NOnkyo.WpfGui.Controller
{
    public class VolumeController : ApiController
    {
        [HttpGet]
        public int GetVolume()
        {
            //var loConnection = App.Container.Resolve<IConnection>();

            return ISCP.Command.CommandBase.GetCommand<ISCP.Command.MasterVolume>().VolumeLevel;
        }

        [HttpGet]
        public IHttpActionResult SetVolumeUp()
        {
            var loConnection = App.Container.Resolve<IConnection>();
            loConnection.SendCommand(ISCP.Command.MasterVolume.UpCommand());
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult SetVolumeDown()
        {
            var loConnection = App.Container.Resolve<IConnection>();
            loConnection.SendCommand(ISCP.Command.MasterVolume.DownCommand());
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult SetVolume([FromUri] int volume)
        {
            var loConnection = App.Container.Resolve<IConnection>();
            loConnection.SendCommand(ISCP.Command.MasterVolume.SetLevel(volume, loConnection.CurrentDevice));
            return Ok();
        }

    }
}
