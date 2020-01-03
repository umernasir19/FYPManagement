using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYPAUtOMATION.Controllers
{
    public class DashBoardController : Controller
    {
        // GET: DashBoard
        FYPEntities db = new FYPEntities();
        public ActionResult DashBoard()
        {

            ViewBag.announcemnt = db.Announcements.OrderByDescending(x => x.Id).Take(3).ToList();
            return View();
        }
    }
}