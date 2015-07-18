using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orlandia2015.Models;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Orlandia2015.Controllers
{
    public class HomeController : AsyncController
    {
        private OrlandiaDbContext db = new OrlandiaDbContext();

        public async Task<ActionResult> IndexAsync()
        {
            var factions = await db.Factions.OrderByDescending(f => f.iPoints).ToListAsync();
            return View("Index", factions);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Map()
        {
            
            return View();
        }
    }
}