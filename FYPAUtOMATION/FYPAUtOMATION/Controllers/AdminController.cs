using FYPAUtOMATION.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
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
            //db.sp_Accept_SignUp(id);
            db.Users.Attach(user);
            try
            {
                user.Is_Active = true;
                user.Is_Pending = false;
                user.Status = "Active";
                db.SaveChanges();
                db.Dispose();
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
            if (usr.Is_Student==true)
            {
                //student data update in students table
                int stdid =Convert.ToInt32(usr.Std_Adv_Id);
                var student = new Student() {ID=stdid };
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


            return RedirectToAction("AcceptSignUp","Admin");
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
            return Json(new {success=true,msg="Saved" }, JsonRequestBehavior.AllowGet);
            
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
                var advisors = db.Advisors.Where(p=>p.IsActive==true).ToList();

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
            return Json(new { success = true,msg="Updated" }, JsonRequestBehavior.AllowGet);
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
                                            group_id=grp.Id,
                                            Group_Name = grp.GroupName,
                                            Student1_Name=std1.Student_Name,
                                            Student2_Name=std2.Student_Name

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
            return Json(new { success = true, msg = "Updated" }, JsonRequestBehavior.AllowGet);
        }
    }
}