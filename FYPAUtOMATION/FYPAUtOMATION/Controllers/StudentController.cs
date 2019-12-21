using FYPAUtOMATION.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYPAUtOMATION.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        FYPEntities db = new FYPEntities();

        public ActionResult StudentList()
        {
            int stdid = Convert.ToInt32(Session["StudentId"]);
            if (!CheckStudentGroup(stdid))
            {
                //student in group
                ViewBag.Message = "You Cannot Add Students You Are Already in Group";
            }
            return View();
        }
        //check if user has already in group
        public bool CheckStudentGroup(int stdid)
        {
            var std = db.Student_Request.Where(p => p.Is_Active == false && p.Is_Accepted == true && p.Request_From_Id == stdid).ToList();
            if (std.Count > 0)
            {
                return false;
            }
            std = db.Student_Request.Where(p => p.Is_Active == false && p.Is_Accepted == true && p.Request_To_Id == stdid).ToList();
            if (std.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        [HttpGet]
        public JsonResult GetStudentList()
        {
          int myuserid=  Convert.ToInt32(Session["userid"].ToString());
           var std= db.Users.Where(p => p.Id == myuserid).FirstOrDefault();
            int mystdntid =Convert.ToInt32(std.Std_Adv_Id);
            //var students=  db.Students.Where(p => p.IsActive == true && p.ID !=myid).ToList();
            var myrequstsents =db.Student_Request.Where(p=>p.Request_From_Id== mystdntid).Select(x => x.Request_To_Id).ToArray();
            var students = db.Students.Where(p => !myrequstsents.Contains(p.ID) && p.ID != mystdntid && p.IsActive == true).ToList();
          return Json(new {data=students }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendRequest(int id)
        {
            int myuserid = Convert.ToInt32(Session["userid"].ToString());
            var std = db.Users.Where(p => p.Id == myuserid).FirstOrDefault();
            int mystdntid = Convert.ToInt32(std.Std_Adv_Id);
            Student_Request stdreq = new Student_Request();
            stdreq.Request_To_Id = id;
            stdreq.Request_From_Id = mystdntid;
            stdreq.Is_Active = true;
            stdreq.Date_Created = DateTime.Now;
            stdreq.Is_Accepted = false;
            stdreq.Is_Rejected = false;
            db.Student_Request.Add(stdreq);
            db.SaveChanges();
            return Json(new { success = true,msg="Successfully sent" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MakeIndividualGroup()
        {
            int mystdid = Convert.ToInt32(Session["StudentId"]);
            int id = mystdid;

            SendRequest(id);
            return AcceptRequest(id);
        }

        public ActionResult RequestList()
        {
            int stdid = Convert.ToInt32(Session["StudentId"]);
            if (!CheckStudentGroup(stdid))
            {
                //student in group
                ViewBag.Message = "You Cannot Add Students You Are Already in Group";
            }
            return View();

        }

        [HttpGet]
        public JsonResult GetRRequestList()
        {
            int myuserid = Convert.ToInt32(Session["userid"].ToString());
            var std = db.Users.Where(p => p.Id == myuserid).FirstOrDefault();
            int mystdntid = Convert.ToInt32(std.Std_Adv_Id);
            var requestlist = db.Student_Request.Where(p => p.Request_To_Id == mystdntid && p.Is_Active == true).Select(p=>p.Request_From_Id).ToArray();
            var stdntreq = db.Students.Where(p => requestlist.Contains(p.ID)).ToList();
            return Json(new { data = stdntreq }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AcceptRequest(int id)
        {
            int mystdid = Convert.ToInt32(Session["StudentId"]);
            var Stdreq = db.Student_Request.Where(p => p.Request_From_Id==id&&p.Request_To_Id==mystdid).FirstOrDefault();
            db.Dispose();
            var std = new Student_Request() {ID=Stdreq.ID };
            db = new FYPEntities();
            db.Student_Request.Attach(std);

            std.Is_Active = false;
            std.Is_Accepted = true;
            std.Date_Accepeted = DateTime.Now;
            db.SaveChanges();
            db.Dispose();

            db = new FYPEntities();
            group grp = new group();
            grp.GroupName = "Group "+Stdreq.Request_From_Id +" --"+Stdreq.Request_To_Id;
            grp.Is_Active = true;
            grp.DateCreated = DateTime.Now;
            db.groups.Add(grp);
            db.SaveChanges();

            int grpid = grp.Id;

            var stdgrp = new Student_Group();
            stdgrp.Student_1_ID = Stdreq.Request_From_Id;
            stdgrp.Student_2_ID = Stdreq.Request_To_Id;
            stdgrp.Group_Id = grpid;
            db.Student_Group.Add(stdgrp);
            db.SaveChanges();     
           return Json(new { success = true, msg = "Successfully sent" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SendAdvisorRequest()
        {
            int stdid = Convert.ToInt32(Session["StudentId"]);
            if (!CheckStudentGroup(stdid))
            {
                //student in group
            }
            else
            {
                ViewBag.Message = "You Are Not in Any Group Please Make Group First";
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetAdvisorsListForRequest()
        {
          
            try
            {


                int mystudntid = Convert.ToInt32(Session["StudentID"]);
                var groupid = db.Student_Group.Where(p => p.Student_1_ID == mystudntid || p.Student_2_ID == mystudntid).FirstOrDefault();
                var advid = db.Student_Advisor_Request.Where(p => p.Group_Id == groupid.Group_Id && p.Is_Active == true).Select(x => x.Advisor_Id).ToArray();
                var advisors = db.Advisors.Where(p => !advid.Contains(p.Id) && p.IsActive == true && p.Advisors_Slot > 0).ToList();
                return Json(new { data = advisors }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }
           
        }

        public JsonResult SendRequesttoAdvisor(int id)
        {
            int mystudntid= Convert.ToInt32(Session["StudentID"]);
            var groupid = db.Student_Group.Where(p => p.Student_1_ID == mystudntid || p.Student_2_ID == mystudntid).FirstOrDefault();
            var stdadvreq = new Student_Advisor_Request();
            stdadvreq.Advisor_Id = id;
            stdadvreq.Student_Id = mystudntid;
            stdadvreq.Group_Id = groupid.Group_Id;
            stdadvreq.Is_Active = true;
            stdadvreq.Date_Created = DateTime.Now;
            stdadvreq.Is_Accepted = false;
            db.Student_Advisor_Request.Add(stdadvreq);
            db.SaveChanges();

            return Json(new {msg="Success",success=true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadDeliverables()
        {
            return View();
        }
        [HttpPost]
        public JsonResult UploadDocument(AdminDocument docs)
        {
            var file = docs.Document;
            int mystudntid = Convert.ToInt32(Session["StudentID"]);
            var grpid = db.Student_Group.Where(p => p.Student_1_ID == mystudntid || p.Student_2_ID == mystudntid).FirstOrDefault();
            if (file != null)
            {
                UploadstdntDeliverables(docs);
            }
            tblFile docsbystdnt = new tblFile();
            docsbystdnt.Document_Name = docs.Document_Name;
            docsbystdnt.Document_Path = docs.DocumentPath;
            docsbystdnt.Is_Active = true;
            docsbystdnt.Date_Created = DateTime.Now;
            docsbystdnt.Group_ID = grpid.Group_Id;
            db.tblFiles.Add(docsbystdnt);
            db.SaveChanges();
            return Json(new { success = true, msg = "Saved" }, JsonRequestBehavior.AllowGet);

        }
        public void UploadstdntDeliverables(AdminDocument admndocs)
        {
            var file = admndocs.Document;
            string filename = "";
            string filepath = "";
            try
            {


                filename = System.IO.Path.GetFileName(System.IO.Path.GetRandomFileName() + file.FileName);
                file.SaveAs(Server.MapPath("/Deliverables/" + filename));

                filepath = "/Deliverables/" + filename;
                //file.SaveAs(Server.MapPath(filepath));
                string fullPath = Request.MapPath(admndocs.DocumentPath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                admndocs.DocumentPath = filepath;



            }
            catch (Exception ex)
            {

            }
        }

        public ActionResult DeliverableDetails()
        {
            return View();
        }
        public ActionResult ProjectList()
        {
            return View();
        }


        public ActionResult ProjectMap()
        {
            return View();
        }


        public ActionResult Notifications()
        {
            int stdid = Convert.ToInt32(Session["StudentId"]);
            if (!CheckStudentGroup(stdid))
            {
               
            }
            return View();
        }
    }
}