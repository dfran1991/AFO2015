using Microsoft.AspNet.SignalR;

namespace Orlandia2015.Hubs
{
    public class MapHub : Hub
    {
        public void UpdateSize(double newSize)
        {
            Clients.All.updateSize(newSize);
        }

        public void NewClientLoaded()
        {
            // TODO: Get current size. Then update it.    
        }
    }
}