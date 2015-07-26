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
    public class MissionAchievementsController : Controller
    {
        private OrlandiaDbContext db = new OrlandiaDbContext();

        // GET: MissionAchievements
        public async Task<ActionResult> Index()
        {
            var missionAchievements = db.MissionAchievements.Include(m => m.Achievement);
            return View(await missionAchievements.ToListAsync());
        }

        // GET: MissionAchievements/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MissionAchievement missionAchievement = await db.MissionAchievements.FindAsync(id);
            if (missionAchievement == null)
            {
                return HttpNotFound();
            }
            return View(missionAchievement);
        }

        // GET: MissionAchievements/Create
        public ActionResult Create()
        {
            ViewBag.uAchievementID = new SelectList(db.Achievements, "uAchievementID", "sName");
            return View();
        }

        // POST: MissionAchievements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "uMissionAchievementID,uAchievementID,iMissionCount")] MissionAchievement missionAchievement)
        {
            if (ModelState.IsValid)
            {
                missionAchievement.uMissionAchievementID = Guid.NewGuid();
                db.MissionAchievements.Add(missionAchievement);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.uAchievementID = new SelectList(db.Achievements, "uAchievementID", "sName", missionAchievement.uAchievementID);
            return View(missionAchievement);
        }

        // GET: MissionAchievements/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MissionAchievement missionAchievement = await db.MissionAchievements.FindAsync(id);
            if (missionAchievement == null)
            {
                return HttpNotFound();
            }
            ViewBag.uAchievementID = new SelectList(db.Achievements, "uAchievementID", "sName", missionAchievement.uAchievementID);
            return View(missionAchievement);
        }

        // POST: MissionAchievements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "uMissionAchievementID,uAchievementID,iMissionCount")] MissionAchievement missionAchievement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(missionAchievement).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.uAchievementID = new SelectList(db.Achievements, "uAchievementID", "sName", missionAchievement.uAchievementID);
            return View(missionAchievement);
        }

        // GET: MissionAchievements/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MissionAchievement missionAchievement = await db.MissionAchievements.FindAsync(id);
            if (missionAchievement == null)
            {
                return HttpNotFound();
            }
            return View(missionAchievement);
        }

        // POST: MissionAchievements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            MissionAchievement missionAchievement = await db.MissionAchievements.FindAsync(id);
            db.MissionAchievements.Remove(missionAchievement);
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
