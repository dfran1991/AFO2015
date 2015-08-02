using System;
using Microsoft.AspNet.SignalR;
using Orlandia2015.Controllers;


namespace Orlandia2015.Hubs
{
    public class MapHub : Hub
    {

        private readonly MapController _mapController;

        public MapHub() : this(MapController.Instance) { }

        public MapHub(MapController mapController)
        {
            _mapController = mapController;
        }

        public double GetCurrentSize()
        {
            return _mapController.MapSize;
        }


        
    }

}