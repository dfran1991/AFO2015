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
            if (nextRank != null && nextRank.iRankPoints != player.Rank.iRankPoints)
                ViewBag.NextRankPercent = ((player.iPoints - player.Rank.iRankPoints) * 100) / (nextRank.iRankPoints - player.Rank.iRankPoints);
            else
                ViewBag.NextRankPercent = -1;
                

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
                db.Entry(player).Property(x => x.uFactionID).IsModified = true;

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

            await db.SaveChangesAsync();

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
            return HttpNotFound();
        }

        public async Task<ActionResult> AddMissionAsync(Guid? id, Guid uMissionID)
        {
            return HttpNotFound();
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
