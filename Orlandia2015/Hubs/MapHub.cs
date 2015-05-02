using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Orlandia2015.Hubs
{
    public class MapHub : Hub
    {
        public void UpdateSize(double size)
        {
            Clients.All.updateSize(size);
        }
    }
}