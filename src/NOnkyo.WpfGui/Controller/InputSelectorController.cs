using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NOnkyo.ISCP;
using NOnkyo.WpfGui.ViewModels.Misc;

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
        public EInputSelector GetEnumState()
        {
            return ISCP.Command.CommandBase.GetCommand<ISCP.Command.InputSelector>().CurrentInputSelector;
        }

        [HttpGet]
        public List<InputSelectorItem> AllSelectors()
        {
            return InputSelectorItem.AllItems;
        }

        [HttpGet]
        public IHttpActionResult Set([FromUri] string selector)
        {
            var loConnection = App.Container.Resolve<IConnection>();
            EInputSelector leInputSelector = selector.ToEnum<EInputSelector>();
            loConnection.SendCommand(ISCP.Command.InputSelector.Chose(leInputSelector, loConnection.CurrentDevice));
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult SetEnum([FromUri] EInputSelector selector)
        {
            var loConnection = App.Container.Resolve<IConnection>();
            loConnection.SendCommand(ISCP.Command.InputSelector.Chose(selector, loConnection.CurrentDevice));
            return Ok();
        }

    }
}
