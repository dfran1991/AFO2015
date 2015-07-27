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
    public class AchievementsController : AsyncController
    {
        private OrlandiaDbContext db = new OrlandiaDbContext();

        // GET: Achievements
        public async Task<ActionResult> IndexAsync()
        {
            return View(await db.Achievements.OrderBy(a=> a.iSortOrder).ToListAsync());
        }

        // GET: Achievements/Create
        public async Task<ActionResult> CreateAsync()
        {
            return View();
        }

        // POST: Achievements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind(Include = "uAchievementID,sName,iSortOrder,bCanBeManuallySet")] Achievement achievement)
        {
            if (ModelState.IsValid)
            {
                achievement.uAchievementID = Guid.NewGuid();
                db.Achievements.Add(achievement);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(achievement);
        }

        // GET: Achievements/Edit/5
        public async Task<ActionResult> EditAsync(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Achievement achievement = await db.Achievements.FindAsync(id);
            if (achievement == null)
            {
                return HttpNotFound();
            }
            return View(achievement);
        }

        // POST: Achievements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind(Include = "uAchievementID,sName,iSortOrder,bCanBeManuallySet")] Achievement achievement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(achievement).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(achievement);
        }

        // GET: Achievements/Delete/5
        public async Task<ActionResult> DeleteAsync(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Achievement achievement = await db.Achievements.FindAsync(id);
            if (achievement == null)
            {
                return HttpNotFound();
            }
            return View(achievement);
        }

        // POST: Achievements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync(Guid id)
        {
            Achievement achievement = await db.Achievements.FindAsync(id);
            db.Achievements.Remove(achievement);
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
