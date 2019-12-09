using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYPAUtOMATION.Controllers
{
    public class AnnouncementController : Controller
    {
        // GET: Announcement
        FYPEntities db = new FYPEntities();
        public ActionResult AddAnnouncement()
        {
            return View();
        }

        public ActionResult ViewAnnouncement()
        {
            return View();
        }

        public JsonResult GetAnnouncement()
        {
            var announcment = db.Announcements.ToList();
            return Json(new {data=announcment }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddAnnouncement(Announcement announcement)
        {
            try
            {
                announcement.DateCreated = DateTime.Now;
                announcement.IsActive = true;
                db.Announcements.Add(announcement);
                db.SaveChanges();
                return Json(new { success = true, msg = "successful"  }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new {success=false,msg="Failed"+ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}