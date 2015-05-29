using NOnkyo.ISCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NOnkyo.WpfGui.Controller
{
    public class CommandController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Send([FromUri] string command)
        {
            var loConnection = App.Container.Resolve<IConnection>();
            loConnection.SendPackage(command);
            return Ok();
        }
    }
}
