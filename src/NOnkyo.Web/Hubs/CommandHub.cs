using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NOnkyo.Web.Hubs
{
    public class CommandHub : Hub
    {
        public string GetMessage()
        {
            return "Hallo Welt";
        }
    }
}