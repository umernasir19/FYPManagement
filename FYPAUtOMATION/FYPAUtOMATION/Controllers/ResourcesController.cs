using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYPAUtOMATION.Controllers
{
    public class ResourcesController : Controller
    {
        // GET: Resources
        FYPEntities db = new FYPEntities();
        public ActionResult ViewResources()
        {
            ViewBag.Resources = db.Resources.OrderByDescending(x => x.Id)
                             .Take(2).ToList();

            return View();
        }
    }
}