using System;
using System.Collections.Generic;
using System.IO;
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
            var announcment = db.Announcements.OrderByDescending(p=>p.Id).ToList();
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

        public ActionResult ViewDocuments()
        {
            return View();
        }


        public JsonResult GetDocuments()
        {
            var announcment = db.Document_By_Admin.Where(p=>p.Is_Active==true).ToList();
            return Json(new { data = announcment }, JsonRequestBehavior.AllowGet);
        }

        public FileResult Download(int id)
        {
          string fpath=  db.Document_By_Admin.Where(p => p.ID == id).Select(x => x.DocumentPath).FirstOrDefault();
            string fullName = Server.MapPath("~" + fpath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullName);
            string fileName = Path.GetFileName(fullName);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        
    }
}