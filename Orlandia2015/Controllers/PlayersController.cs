using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Orlandia2015.Models;
using Orlandia2015.Services;
using PagedList;

// TODO: (Next year) Move all business logic into service classes. Code is becoming too difficult to maintain as is.

namespace Orlandia2015.Controllers
{
    public class PlayersController : AsyncController
    {
        private OrlandiaDbContext db = new OrlandiaDbContext();

        // GET: Players
        //public async Task<ActionResult> IndexAsync()
        //{
        //    var players = db.Players.Include(p => p.Faction);
        //    return View("Index", await players.ToListAsync());
        //}

        
        public async Task<ActionResult> IndexAsync(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.FactionSortParm = sortOrder == "faction" ? "faction_desc" : "faction";
            ViewBag.RankSortParm = sortOrder == "rank" ? "rank_desc" : "rank";
            ViewBag.PointsSortParm = sortOrder == "points" ? "points_desc" : "points";

            if(searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var players = db.Players.Include(p => p.Faction);

            if(!string.IsNullOrEmpty(searchString))
            {
                players = players.Where(p => p.sName.Contains(searchString));
            }

            switch(sortOrder)
            {
                case "name_desc":
                    players = players.OrderByDescending(p => p.sName);
                    break;
                case "faction":
                    players = players.OrderBy(p => p.Faction.sName);
                    break;
                case "faction_desc":
                    players = players.OrderByDescending(p => p.Faction.sName);
                    break;
                case "rank":
                    players = players.OrderBy(p => p.Rank.sRankName);
                    break;
                case "rank_desc":
                    players = players.OrderByDescending(p => p.Rank.sRankName);
                    break;
                case "points":
                    players = players.OrderBy(p => p.iPoints);
                    break;
                case "points_desc":
                    players = players.OrderByDescending(p => p.iPoints);
                    break;
                    
                default:
                    players = players.OrderBy(p => p.sName);
                    break;
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);

            return View("Index", (await players.ToListAsync()).ToPagedList(pageNumber, pageSize));

        }

        // GET: Players/Details/5
        public async Task<ActionResult> DetailsAsync(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var player = await db.Players.FindAsync(id);
            if (player == null)
            {
                return HttpNotFound();
            }

            var nextRank = await db.Ranks.OrderBy(r => r.iRankNumber).FirstOrDefaultAsync(r => r.uFactionID == player.uFactionID && r.iRankPoints > player.iPoints);
            var maxRank = await db.Ranks.OrderByDescending(r => r.iRankNumber).FirstOrDefaultAsync(r => r.uFactionID == player.uFactionID);

            if (nextRank != null && nextRank.iRankPoints != player.Rank.iRankPoints)
            {
                ViewBag.NextRankPercent = ((player.iPoints - player.Rank.iRankPoints) * 100) / (nextRank.iRankPoints - player.Rank.iRankPoints);
                ViewBag.NextRankPoints = nextRank.iRankPoints;
                ViewBag.TotalPercent = (player.iPoints * 100) / maxRank.iRankPoints;

                if (ViewBag.TotalPercent > 100)
                    ViewBag.TotalPercent = 100;

            }
            else
            {
                ViewBag.NextRankPercent = -1;
            }

            var playerRanks = await db.Ranks.OrderBy(r => r.iRankNumber).Where(r => r.uFactionID == player.uFactionID && r.iRankPoints <= player.iPoints).Select(r => r.sRankName).ToListAsync();
            ViewBag.PlayerRanks = playerRanks;
                

            return View(player);
        }

        // GET: Players/Create
        public async Task<ActionResult> CreateAsync()
        {
            ViewBag.uFactionID = new SelectList(await db.Factions.ToListAsync(), "uFactionID", "sName");
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind(Include = "uPlayerID,uFactionID,sName")] Player player)
        {
            if (ModelState.IsValid)
            {
                player.uPlayerID = Guid.NewGuid();
                Rank rank = await db.Ranks.FirstAsync(r => r.uFactionID == player.uFactionID && r.iRankNumber == 0);
                player.uRankID = rank.uRankID;
                db.Players.Add(player);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.uFactionID = new SelectList(await db.Factions.ToListAsync(), "uFactionID", "sName", player.uFactionID);
            return View(player);
        }

        // GET: Players/Edit/5
        public async Task<ActionResult> EditAsync(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var player = await db.Players.FindAsync(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            ViewBag.uFactionID = new SelectList(await db.Factions.ToListAsync(), "uFactionID", "sName", player.uFactionID);
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind(Include = "uPlayerID,uFactionID,sName")] Player player)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(player).State = EntityState.Modified;
                db.Players.Attach(player);
                db.Entry(player).Property(x => x.sName).IsModified = true;

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.uFactionID = new SelectList(await db.Factions.ToListAsync(), "uFactionID", "sName", player.uFactionID);
            return View(player);
        }

        // GET: Players/Delete/5
        public async Task<ActionResult> DeleteAsync(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var player = await db.Players.FindAsync(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync(Guid id)
        {
            var player = await db.Players.FindAsync(id);
            db.Players.Remove(player);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> AddPointsAsync(Guid id)
        {
            var player = await db.Players.FindAsync(id);

            if (player == null)
            {
                return HttpNotFound();
            }

            return View(player);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPointsAsync(Guid? id, int iPoints)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var player = await db.Players.FindAsync(id);

            if (player == null)
            {
                return HttpNotFound();
            }

            if (iPoints < 0)
            {
                return new HttpStatusCodeResult(400, "Invalid number of points to add");
            }

            await AddPointsToPlayerAsync(player, iPoints);

            return new RedirectResult(Url.Action("Details", "Players", new { @id = id }));

        }


        // TODO: Implement These
        public async Task<ActionResult> AddAchievementAsync(Guid? id)
        {
            return HttpNotFound();
        }

        public async Task<ActionResult> AddAchievementAsync(Guid? id, Guid uAchievementID)
        {
            return HttpNotFound();
        }

        public async Task<ActionResult> AddMissionAsync(Guid? id)
        {
            if(!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var player = await db.Players.FirstOrDefaultAsync(p => p.uPlayerID == id);

            if(player == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var rank = player.Rank;
            var playerMissions = player.Missions.Select(pm => pm.uMissionID);

            var missions = db.Missions.Where(m => m.iMissionLevel <= rank.iRankNumber);

            var incompleteMissions = await missions.Where(m => !playerMissions.Contains(m.uMissionID) || m.bIsMissionQuest).ToListAsync();

            ViewBag.id = id;
            return View(incompleteMissions);
        }

        [ActionName("AddMissionToPlayer")]
        public async Task<ActionResult> AddMissionAsync(Guid? id, Guid uMissionID)
        {
            if(!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid User ID");

            var player = await db.Players.FirstOrDefaultAsync(p => p.uPlayerID == id);

            if (player == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "User Not Found: " + id.ToString());

            var mission = await db.Missions.FirstOrDefaultAsync(m => m.uMissionID == uMissionID);

            // Handles adding points to player and faction. Also maintains ranks.
            await AddPointsToPlayerAsync(player, mission.iMissionPoints);

            // Add record of completing mission
            PlayerMission playerMission = new PlayerMission();
            playerMission.uMissionID = uMissionID;
            playerMission.uPlayerID = id.Value;
            playerMission.uPlayerMissionID = Guid.NewGuid();


            // Add completion achievement, if earned.
            var completedMissions = player.Missions.Count() + 1;

            var missionAchievement = await db.MissionAchievements.FirstOrDefaultAsync(ma => ma.iMissionCount == completedMissions);

            if (missionAchievement != null)
            {
                if (!player.Achievements.Any(pa => pa.uAchievementID == missionAchievement.uAchievementID))
                {
                    PlayerAchievements playerAchievement = new PlayerAchievements();
                    playerAchievement.uAchievementID = missionAchievement.uAchievementID;
                    playerAchievement.uPlayerID = id.Value;
                    playerAchievement.uPlayerAchievementID = Guid.NewGuid();

                    db.PlayerAchievements.Add(playerAchievement);
                }
            }

            db.PlayerMissions.Add(playerMission);

            await db.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id });
        }



        private async Task AddPointsToPlayerAsync(Player player, int iPoints)
        {
            player.iPoints += iPoints;
            db.Players.Attach(player);
            db.Entry(player).Property(p => p.iPoints).IsModified = true;

            var currRank = player.Rank;


            // Check Rank
            var nextRank =
                await
                    db.Ranks.OrderBy(r => r.iRankNumber)
                        .FirstOrDefaultAsync(r => r.uFactionID == player.uFactionID && r.iRankNumber > currRank.iRankNumber);


            if (nextRank != null && nextRank.iRankPoints <= player.iPoints)
            {
                player.uRankID = nextRank.uRankID;
                db.Entry(player).Property(p => p.uRankID).IsModified = true;
            }

            // Add Faction points
            var faction = await db.Factions.FirstOrDefaultAsync(f => f.uFactionID == player.uFactionID);
            faction.iPoints += iPoints;
            db.Factions.Attach(faction);
            db.Entry(faction).Property(f => f.iPoints).IsModified = true;

            await db.SaveChangesAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
