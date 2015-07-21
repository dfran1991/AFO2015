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
    public class MissionsController : AsyncController
    {
        private OrlandiaDbContext db = new OrlandiaDbContext();

        // GET: Missions
        public async Task<ActionResult> IndexAsync()
        {
            return View(await db.Missions.ToListAsync());
        }

        // GET: Missions/Details/5
        public async Task<ActionResult> DetailsAsync(Guid? id)
        {
            return HttpNotFound();
        }

        // GET: Missions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Missions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind(Include = "uMissionID,sMissionName,iMissionPoints,bIsMissionQuest")] Mission mission)
        {
            if (ModelState.IsValid)
            {
                mission.uMissionID = Guid.NewGuid();
                db.Missions.Add(mission);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(mission);
        }

        // GET: Missions/Edit/5
        public async Task<ActionResult> EditAsync(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mission mission = await db.Missions.FindAsync(id);
            if (mission == null)
            {
                return HttpNotFound();
            }
            return View(mission);
        }

        // POST: Missions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind(Include = "uMissionID,sMissionName,iMissionPoints,bIsMissionQuest")] Mission mission)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mission).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(mission);
        }

        // GET: Missions/Delete/5
        public async Task<ActionResult> DeleteAsync(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mission mission = await db.Missions.FindAsync(id);
            if (mission == null)
            {
                return HttpNotFound();
            }
            return View(mission);
        }

        // POST: Missions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync(Guid id)
        {
            Mission mission = await db.Missions.FindAsync(id);
            db.Missions.Remove(mission);
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
