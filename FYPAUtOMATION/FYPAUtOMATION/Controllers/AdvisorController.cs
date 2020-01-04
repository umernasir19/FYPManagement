using FYPAUtOMATION.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYPAUtOMATION.Controllers
{
    public class AdvisorController : Controller
    {
        // GET: Advisor
        FYPEntities db = new FYPEntities();

        public ActionResult StudentRequests()
        {
            return View();
        }

        public JsonResult GetStudentRequest()
        {
            int advisorid = Convert.ToInt32(Session["AdvisorId"]);
            var advisorreq = (from grp in db.groups
                          join stadvreq in db.Student_Advisor_Request on grp.Id equals stadvreq.Group_Id
                          join sgrp in db.Student_Group on grp.Id equals sgrp.Group_Id
                          join std1 in db.Students on sgrp.Student_1_ID equals std1.ID
                          join std2 in db.Students on sgrp.Student_2_ID equals std2.ID
                          where stadvreq.Advisor_Id == advisorid && stadvreq.Is_Active==true
                          select new StudentGroupViewModel
                          {
                              std_Adv_req_id=stadvreq.Id,
                              group_id = grp.Id,
                              Group_Name = grp.GroupName,
                              Student1_Name = std1.Student_Name,
                              Student2_Name = std2.Student_Name

                          }).ToList();

            return Json(new {data= advisorreq }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult AcceptstudentRequest(int id)
        {
            int advisorid = Convert.ToInt32(Session["AdvisorId"]);
            var adv=db.Advisors.Where(p => p.Id == advisorid).FirstOrDefault();
            int slots =Convert.ToInt32(adv.Advisors_Slot);
            if (slots > 0)
            {
                var stdadvreq = new Student_Advisor_Request()
                {
                    Id = id
                };

                db.Student_Advisor_Request.Attach(stdadvreq);
                stdadvreq.Is_Accepted = true;
                stdadvreq.Date_Accepted = DateTime.Now;
                stdadvreq.Is_Active = false;
                db.SaveChanges();
                db.Dispose();

                db = new FYPEntities();
                var advisorslotupdate = new Advisor()
                {
                    Id = advisorid
                };
                db.Advisors.Attach(advisorslotupdate);
                advisorslotupdate.Advisors_Slot = slots - 1;
                db.SaveChanges();
               
                NotificationAdd(id);
                return Json(new { msg = "Successfull", success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { msg = "Unable To Accept 0 Slots Available", success = true }, JsonRequestBehavior.AllowGet);
            }
           

        }

        public ActionResult Deliverables()
        {
            return View();
        }

        public JsonResult GetGroups()
        {
            int advisorid = Convert.ToInt32(Session["AdvisorId"]);
            var advisorreq = (from grp in db.groups
                              join stadvreq in db.Student_Advisor_Request on grp.Id equals stadvreq.Group_Id
                              join sgrp in db.Student_Group on grp.Id equals sgrp.Group_Id
                              
                              where stadvreq.Advisor_Id == advisorid && stadvreq.Is_Active == false && stadvreq.Is_Accepted==true
                              select new StudentGroupViewModel
                              {
                                  std_Adv_req_id = stadvreq.Id,
                                  group_id = grp.Id,
                                  Group_Name = grp.GroupName,
                                  

                              }).ToList();

            return Json(new { data = advisorreq }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GroupFiles(int id)
        {
            ViewBag.GroupId = id;
            return View();
        }
        public JsonResult GetGroupFiles(int id)
        {

            var GroupFiles = db.tblFiles.Where(p => p.Group_ID == id).ToList();
            return Json(new { data = GroupFiles }, JsonRequestBehavior.AllowGet);

        }

        public FileResult Download(int id)
        {
            string fpath = db.tblFiles.Where(p => p.Id == id).Select(x => x.Document_Path).FirstOrDefault();
            string fullName = Server.MapPath("~" + fpath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullName);
            string fileName = Path.GetFileName(fullName);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }



        public ActionResult GroupsList()
        {
            return View();
        }


        public ActionResult AdvisorSlots()
        {
            int advisorid = Convert.ToInt32(Session["AdvisorId"]);
            ViewBag.Slots = db.Advisors.Where(p => p.Id == advisorid).FirstOrDefault();

            return View();
        }


        [HttpGet]
        public JsonResult GetGroupsList()
        {
            int advid = Convert.ToInt32(Session["AdvisorId"].ToString());
            try
            {

                var groups = (from grp in db.groups
                                                join sgrp in db.Student_Group on grp.Id equals sgrp.Group_Id
                                                join std1 in db.Students on sgrp.Student_1_ID equals std1.ID
                                                join std2 in db.Students on sgrp.Student_2_ID equals std2.ID
                                                join stdadvre in db.Student_Advisor_Request on grp.Id equals stdadvre.Group_Id
                                                where stdadvre.Advisor_Id == advid
                                                select new StudentGroupViewModel
                                                {
                                                    group_id = grp.Id,
                                                    Group_Name = grp.GroupName,
                                                    Student1_Name = std1.Student_Name,
                                                    Student2_Name = std2.Student_Name

                                                }).ToList();



                //var groups = db.Student_Advisor_Request.Where(p=>p.Advisor_Id==advid && p.Is_Accepted==true).ToList();

                return Json(new { success = true, msg = "Success", data = groups }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Internal Server Error" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public static void NotificationAdd(int id)
        {
            FYPEntities db = new FYPEntities();
            var grouplist = db.Student_Group.Where(p => p.Group_Id == id).FirstOrDefault();

            for (int i = 0; i < 2; i++)
            {
                notification notif = new notification();
                if (i == 0)
                {


                    notif.N_To = grouplist.Student_1_ID;
                }
                if (i == 1)
                {
                    notif.N_To = grouplist.Student_2_ID;
                }
                notif.Notification_Msg = "Your Request is Accepted By Advisor";
                notif.Isread = false;

                notif.DateCreated = DateTime.Now;
                notif.is_active = true;
                FYPEntities dbs = new FYPEntities();
                dbs.notifications.Add(notif);
                dbs.SaveChanges();
            }



        }
    }
}