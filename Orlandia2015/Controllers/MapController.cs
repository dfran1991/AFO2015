using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Orlandia2015.Hubs;
using Orlandia2015.Models;

namespace Orlandia2015.Controllers
{

    public class MapController
    {
        private readonly static Lazy<MapController> _instance = new Lazy<MapController>(() => new MapController(GlobalHost.ConnectionManager.GetHubContext<MapHub>().Clients));

        private OrlandiaDbContext db = new OrlandiaDbContext();

        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(10);
        private readonly Timer _timer;


        private MapController(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _timer = new Timer(UpdateFactionPoints, null, _updateInterval, _updateInterval);

            MapSize = 800.0;
        }

        public static MapController Instance
        {
            get { return _instance.Value; }
        }

        public double MapSize { get; private set; }


        private IHubConnectionContext<dynamic> Clients { get; set; }

        private async void UpdateFactionPoints(object state)
        {
            var factions = await db.Factions.ToListAsync();

            // Using Equation S = log(Mwh*T^2) - [((Sb + Sr) * Mp) + Sdem]
            var updateDb = false;
            var Mwh = await db.GameProperties.FirstOrDefaultAsync(gp => gp.sName == "Withered Hand Multiplier");
            if (Mwh == null)
            {
                Mwh = new GameProperty {sName = "Withered Hand Multiplier", sValue = "1"};
                db.GameProperties.Add(Mwh);
                updateDb = true;
            }

            var Mp = await db.GameProperties.FirstOrDefaultAsync(gp => gp.sName == "Player Score Multiplier");
            if (Mp == null)
            {
                Mp = new GameProperty { sName = "Player Score Multiplier", sValue = "1" };
                db.GameProperties.Add(Mp);
                updateDb = true;
            }

            var Sdem = await db.GameProperties.FirstOrDefaultAsync(gp => gp.sName == "Deus Ex Machina Points");
            if (Sdem == null)
            {
                Sdem = new GameProperty { sName = "Deus Ex Machina Points", sValue = "0" };
                db.GameProperties.Add(Sdem);
                updateDb = true;
            }

            if (updateDb)
            {
                await db.SaveChangesAsync();
            }

            var whScore = CalculateWitheredHandScore(Convert.ToDouble(Mwh.sValue)); // log(Mwh*T^2)
            
            double factionPoints = 0;
            factions.ForEach(f => factionPoints += f.iPoints); // Sb + Sr

            factionPoints *= Convert.ToDouble(Mp.sValue); //prev * Mp

            factionPoints += Convert.ToDouble(Sdem.sValue); //prev + Sdem

            var radius = whScore - factionPoints; // log(Mwh*T^2) - [((Sb + Sr) * Mp) + Sdem]

            if (radius < 0)
                radius = 0;
            else if (radius > 820)
                radius = 820;

            UpdateSize(radius);
            MapSize = radius;
        }

        private static double CalculateWitheredHandScore(double Mwh)
        {
            double elapsedTime;
            switch (DateTime.Now.DayOfYear)
            {
                case 226:
                    elapsedTime = (DateTime.Now - new DateTime(2015, 08, 14, 20, 15, 00)).TotalSeconds; // 2700 max
                    break;
                case 227:
                    elapsedTime = 2700 + (DateTime.Now - new DateTime(2015, 08, 15, 10, 00, 00)).TotalSeconds; // 35100 max (2700 + 32400)
                    if (DateTime.Now > new DateTime(2015, 08, 15, 18, 15, 00))
                        elapsedTime -= 9000;
                    break;
                case 228:
                    elapsedTime = 35100 + (DateTime.Now - new DateTime(2015, 08, 16, 10, 00, 00)).TotalSeconds;
                    break;
                default:
                    return 0;
            }

            return Math.Log10(Math.Pow(elapsedTime, 2)*Mwh);
        }

        // Client Broadcast functions
        private void UpdateSize(double newSize)
        {
            Clients.All.updateSize(newSize);
        }

    }
}