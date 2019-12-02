using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYPAUtOMATION.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public JsonResult Login(User users)
        {
            return Json(new { users = users }, JsonRequestBehavior.AllowGet);
        }
    }
}