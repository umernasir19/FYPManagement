using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYPAUtOMATION.Controllers
{
    public class AdminController : Controller
    {
        FYPEntities db = new FYPEntities();
        // GET: Admin
        public ActionResult AcceptSignUp()
        {
            return View();
        }

        [HttpGet]
        public JsonResult SignUpRequest()
        {
            try {
                //{
                var reques = db.sp_SignUp_Requests().ToList();

                return Json(new { success = false, msg = "Internal Server Error",data=reques }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = false, msg = "Internal Server Error"+ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult AcceptRequests(int id)
        {
            var user = new User { Id=id };
            db.sp_Accept_SignUp(id);
            //db.Users.Attach(user);
            //user.Is_Active = true;
            //user.Is_Pending = false;

            //db.SaveChanges();

            return RedirectToAction("AcceptSignUp","Admin");
        }

    }
}