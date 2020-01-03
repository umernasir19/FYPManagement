using FYPAUtOMATION.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Threading;
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
            try
            {
                //{
                var reques = db.sp_SignUp_Requests().ToList();

                return Json(new { success = false, msg = "Internal Server Error", data = reques }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Internal Server Error" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult AcceptRequests(int id)
        {
            var user = new User { Id = id };
            //db.sp_Accept_SignUp(id);
            db.Users.Attach(user);
            try
            {
                user.Is_Active = true;
                user.Is_Pending = false;
                user.Status = "Active";
                db.SaveChanges();
                db.Dispose();
                notification notifc = new notification();
                notifc.Notification_Msg = "Your Sign Up Request Is Accepted";
                notifc.N_To = id;
                 NotificationAdd(notifc);
               
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            db = new FYPEntities();
            var usr = db.Users.Where(p => p.Id == id).FirstOrDefault();
            if (usr.Is_Student == true)
            {
                //student data update in students table
                int stdid = Convert.ToInt32(usr.Std_Adv_Id);
                var student = new Student() { ID = stdid };
                db.Students.Attach(student);
                student.IsPending = false;
                student.IsActive = true;
                db.SaveChanges();
                //student data update in user in roles table
                var roles = db.Roles.Where(p => p.Role_Name == "Student").FirstOrDefault();
                User_IN_Roles uir = new User_IN_Roles();
                uir.Role_Id = roles.Id;
                uir.User_Id = id;
                uir.Is_Active = true;
                uir.Status = "Active";
                uir.Date_Created = DateTime.Now;
                uir.Created_By = 1;
                db.User_IN_Roles.Add(uir);
                db.SaveChanges();


            }
            else
            {
                //advisor data update in students table
                int advid = Convert.ToInt32(usr.Std_Adv_Id);
                var advisor = new Advisor() { Id = advid };
                db.Advisors.Attach(advisor);
                advisor.IsBlock = false;
                advisor.IsActive = true;
                db.SaveChanges();
                //student data update in user in roles table
                var roles = db.Roles.Where(p => p.Role_Name == "Advisor").FirstOrDefault();
                User_IN_Roles uir = new User_IN_Roles();
                uir.Role_Id = roles.Id;
                uir.User_Id = id;
                uir.Is_Active = true;
                uir.Status = "Active";
                uir.Date_Created = DateTime.Now;
                uir.Created_By = 1;
                db.User_IN_Roles.Add(uir);
                db.SaveChanges();

            }


            return RedirectToAction("AcceptSignUp", "Admin");
        }

        public EmptyResult RejectSignupRequest(int id)
        {
            var user = new User() { Id = id };
            db.Users.Attach(user);
            user.Is_Block = true;
            user.Is_Active = false;
            db.SaveChanges();
            return null;
        }


        public ActionResult UploadDocument()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UploadDocument(AdminDocument docs)
        {
            var file = docs.Document;
            if (file != null)
            {
                UploadProfilePicture(docs);
            }
            Document_By_Admin docsbyadmin = new Document_By_Admin();
            docsbyadmin.Document_Name = docs.Document_Name;
            docsbyadmin.DocumentPath = docs.DocumentPath;
            docsbyadmin.Is_Active = true;
            docsbyadmin.DateCreated = DateTime.Now;

            db.Document_By_Admin.Add(docsbyadmin);
            db.SaveChanges();
            return Json(new { success = true, msg = "Saved" }, JsonRequestBehavior.AllowGet);

        }
        public void UploadProfilePicture(AdminDocument admndocs)
        {
            var file = admndocs.Document;
            string filename = "";
            string filepath = "";
            try
            {


                filename = System.IO.Path.GetFileName(System.IO.Path.GetRandomFileName() + file.FileName);
                file.SaveAs(Server.MapPath("/Docs/" + filename));

                filepath = "/Docs/" + filename;
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

        public ActionResult AdvisorsList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAdvisorsList()
        {
            try
            {
                //{
                var advisors = db.Advisors.Where(p => p.IsActive == true).ToList();

                return Json(new { success = true, msg = "Success", data = advisors }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Internal Server Error" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateAdvisorDetails(int id)
        {
            Advisor adv = db.Advisors.Where(p => p.Id == id).FirstOrDefault();
            return View(adv);
        }
        [HttpPost]
        public JsonResult UpdateAdvisorDetails(Advisor adv)
        {
            Advisor ad = new Advisor() { Id = adv.Id };
            db.Advisors.Attach(ad);
            ad.Advisor_Description = adv.Advisor_Description;
            //user.Is_Pending = false;

            db.SaveChanges();
            return Json(new { success = true, msg = "Updated" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GroupsList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetGroupsList()
        {
            try
            {
                //{
                var groups = db.groups.ToList();

                return Json(new { success = true, msg = "Success", data = groups }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Internal Server Error" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult UpdateGroupDetails(int id)
        {
            StudentGroupViewModel stdgrp = (from grp in db.groups
                                            join sgrp in db.Student_Group on grp.Id equals sgrp.Group_Id
                                            join std1 in db.Students on sgrp.Student_1_ID equals std1.ID
                                            join std2 in db.Students on sgrp.Student_2_ID equals std2.ID
                                            where grp.Id == id
                                            select new StudentGroupViewModel
                                            {
                                                group_id = grp.Id,
                                                Group_Name = grp.GroupName,
                                                Student1_Name = std1.Student_Name,
                                                Student2_Name = std2.Student_Name

                                            }).FirstOrDefault();

            return View(stdgrp);
        }
        [HttpPost]
        public JsonResult UpdateGroupDetails(StudentGroupViewModel grp)
        {
            group gp = new group() { Id = grp.group_id };
            db.groups.Attach(gp);
            gp.GroupName = grp.Group_Name;
            //user.Is_Pending = false;

            db.SaveChanges();
            notification not = new notification();
            not.Notification_Msg = "Your Group name is Changed to " + grp.Group_Name + " By Admin";
            not.Group_Id = grp.group_id;
            NotificationAdd(not);
            return Json(new { success = true, msg = "Updated" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewGroupFiles(int id)
        {
            ViewBag.GroupId = id;
            return View();
        }
        [HttpGet]
        public JsonResult GetGroupsFiles(int id)
        {
            try
            {
                //{
                var groups = db.tblFiles.Where(p => p.Group_ID == id).ToList();

                return Json(new { success = true, msg = "Success", data = groups }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Internal Server Error" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public FileResult Download(int id)
        {
            string fpath = db.tblFiles.Where(p => p.Id == id).Select(x => x.Document_Path).FirstOrDefault();
            string fullName = Server.MapPath("~" + fpath);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullName);
            string fileName = Path.GetFileName(fullName);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        public ActionResult AddProject()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddNewProject(Project pro)
        {
            pro.IsActive = true;
            pro.Date_Created = DateTime.Now;
            db.Projects.Add(pro);
            db.SaveChanges();
            return Json(new { success = true, msg = "Added" }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetAllProjects()
        {
            try
            {
                //{
                var Projects = db.Projects.Where(p => p.IsActive == true).ToList();

                return Json(new { success = true, msg = "Success", data = Projects }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Internal Server Error" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Grades(int id)
        {
            Grade grades = db.Grades.Where(p => p.GroupId == id).FirstOrDefault();
            if (grades == null)
            {
                grades = new Grade();
                grades.GroupId = id;
            }
            else
            {

            }

            return View(grades);
        }

        [HttpPost]
        public JsonResult UploadGrades(Grade gr)
        {
            var checkexisting = db.Grades.Where(p => p.GroupId == gr.GroupId).FirstOrDefault();
            if (checkexisting == null)
            {//add new
                gr.Is_Active = true;
                gr.Date_Created = DateTime.Now;
                db.Grades.Add(gr);
                db.SaveChanges();
                return Json(new { success = true, msg = "Successfull" }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var grade = new Grade() { Id = gr.Id };
                db.Grades.Attach(grade);
                grade.FirstHalf_marks = gr.FirstHalf_marks;
                grade.SecondHalf_marks = gr.SecondHalf_marks;
                grade.ThirdHalf_marks = gr.ThirdHalf_marks;
                db.Grades.Add(grade);
                db.SaveChanges();
                return Json(new { success = true, msg = "Successfull" }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult UploadDeliverables()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UploadNewDeliverables(Deliverable del)
        {
            del.Date_Created = DateTime.Now;
            del.is_Active = true;
            db.Deliverables.Add(del);
            db.SaveChanges();
            return Json(new { success = true, msg = "Successfull" }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetAllDeliverables()
        {
            try
            {
                //{
                var Projects = db.Deliverables.Where(p => p.is_Active == true).ToList();

                return Json(new { success = true, msg = "Success", data = Projects }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Internal Server Error" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult UploadResources()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UploadNewResources(Resource res)
        {
            res.datecreated = DateTime.Now;
            res.Is_Active = true;
           // res.is_Active = true;
            db.Resources.Add(res);
            db.SaveChanges();
            return Json(new { success = true, msg = "Successfull" }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetAllResources()
        {
            try
            {
                //{
                var resources = db.Resources.Where(p=>p.Is_Active==true).ToList();

                return Json(new { success = true, msg = "Success", data = resources }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Internal Server Error" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Project_RoadMap()
        {
            return View();

        }
        [HttpPost]
        public JsonResult UploadProjectRoadmap(AdminDocument docs)
        {
            var file = docs.Document;
            if (file != null)
            {
                UploadRoadMap(docs);
            }
            Project_roadMap docsbyadmin = new Project_roadMap();
            //docsbyadmin.Document_Path = docs.Document_Name;
            docsbyadmin.Document_Path = docs.DocumentPath;
            docsbyadmin.is_Active = true;
            docsbyadmin.DateCreated = DateTime.Now;

            db.Project_roadMap.Add(docsbyadmin);
            db.SaveChanges();
            return Json(new { success = true, msg = "Saved" }, JsonRequestBehavior.AllowGet);

        }
        public void UploadRoadMap(AdminDocument admndocs)
        {
            var file = admndocs.Document;
            string filename = "";
            string filepath = "";
            try
            {


                filename = System.IO.Path.GetFileName(System.IO.Path.GetRandomFileName() + file.FileName);
                file.SaveAs(Server.MapPath("/ProjectRoadMap/" + filename));

                filepath = "/ProjectRoadMap/" + filename;
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


        public void NotificationAdd(notification notif)
        {
            notif.Isread = false;
            
            notif.DateCreated = DateTime.Now;
            notif.is_active = true;
            db = new FYPEntities();
            db.notifications.Add(notif);
            db.SaveChanges();
        }
    }
}