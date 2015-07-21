using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Orlandia2015.Models;

namespace Orlandia2015.Controllers
{
    public class RanksController : AsyncController
    {
        private OrlandiaDbContext db = new OrlandiaDbContext();

        // GET: Ranks
        public async Task<ActionResult> IndexAsync()
        {
            return View((await db.Ranks.OrderBy(r => r.iRankNumber).ToListAsync()).GroupBy(r => r.Faction.sName));
        }

        // GET: Ranks/Details/5
        public async Task<ActionResult> DetailsAsync(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rank rank = await db.Ranks.FindAsync(id);
            if (rank == null)
            {
                return HttpNotFound();
            }
            return View(rank);
        }

        // GET: Ranks/Create
        public async Task<ActionResult> CreateAsync()
        {
            ViewBag.uFactionID = new SelectList(await db.Factions.ToListAsync(), "uFactionID", "sName");
            return View();
        }

        // POST: Ranks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind(Include = "uRankID,uFactionID,iRankNumber,iRankPoints,sRankName")] Rank rank)
        {
            if (ModelState.IsValid)
            {
                rank.uRankID = Guid.NewGuid();
                db.Ranks.Add(rank);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(rank);
        }

        // GET: Ranks/Edit/5
        public async Task<ActionResult> EditAsync(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rank rank = await db.Ranks.FindAsync(id);
            if (rank == null)
            {
                return HttpNotFound();
            }

            ViewBag.uFactionID = new SelectList(await db.Factions.ToListAsync(), "uFactionID", "sName", rank.uFactionID);

            return View(rank);
        }

        // POST: Ranks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind(Include = "uRankID,uFactionID,iRankNumber,iRankPoints,sRankName")] Rank rank)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rank).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(rank);
        }

        // GET: Ranks/Delete/5
        public async Task<ActionResult> DeleteAsync(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rank rank = await db.Ranks.FindAsync(id);
            if (rank == null)
            {
                return HttpNotFound();
            }
            return View(rank);
        }

        // POST: Ranks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync(Guid id)
        {
            Rank rank = await db.Ranks.FindAsync(id);
            db.Ranks.Remove(rank);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
