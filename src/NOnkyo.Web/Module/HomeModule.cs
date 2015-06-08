using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOnkyo.Web.Module
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            this.Get["/"] = v => this.View["index.html"];
        }
    }
}
