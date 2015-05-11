using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NOnkyo.ISCP;

namespace NOnkyo.WpfGui.Controller
{
    public class InputSelectorController : ApiController
    {
        [HttpGet]
        public string GetState()
        {
            return ISCP.Command.CommandBase.GetCommand<ISCP.Command.InputSelector>().CurrentInputSelector.ToString();
        }

        [HttpGet]
        public IHttpActionResult Set([FromUri] string selector)
        {
            var loConnection = App.Container.Resolve<IConnection>();
            EInputSelector leInputSelector = selector.ToEnum<EInputSelector>();
            loConnection.SendCommand(ISCP.Command.InputSelector.Chose(leInputSelector, loConnection.CurrentDevice));
            return Ok();
        }

    }
}
