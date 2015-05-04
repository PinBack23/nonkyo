using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOnkyo.WpfGui.Web
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            this.Get["/"] = v => this.View["Web/index.html"];
        }
    }
}
