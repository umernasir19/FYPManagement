using FYPAUtOMATION;
using FYPAUtOMATION.Models;
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
        FYPEntities db = new FYPEntities();
        

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User users)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(p => p.Email == users.Email && p.User_Password == users.User_Password && p.Is_Active == true).FirstOrDefault();
                if (user != null)
                {
                    var userrole = db.User_IN_Roles.Where(p => p.User_Id == user.Id).FirstOrDefault();
                    if (userrole != null)
                    {
                        var rolename = db.Roles.Where(p => p.Id == userrole.Role_Id).FirstOrDefault();

                        Session["userid"] = user.Id;
                        Session["UserName"] = user.User_Name;
                        Session["RoleID"] = userrole.Role_Id;
                        Session["RoleName"] = rolename.Role_Name;
                    }
                    //return RedirectToAction("Dashboard", "Dashboard");
                   return Json(new { url = "/Dashboard/Dashboard", msg="Logged In",success=true }, JsonRequestBehavior.AllowGet);

                }

                else
                {
                    return Json(new { url = "/Account/Login", msg = "No User Found Or Account In Pending", success = false }, JsonRequestBehavior.AllowGet);

                }


            }
            else
            {
                return View(users);

            }
        }


        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Register(User_Registration registermodel)
        {

            if (registermodel.Registration_Type == 1)//student
            {

                Student std = new Student();
                std.Student_Name = registermodel.Name;
                std.Student_Registration_No = registermodel.university_Id;
                std.Student_Email = registermodel.Student_Email;
                std.Student_Registration_No = registermodel.university_Id;
                std.IsActive = false;
                std.IsPending = true;
                std.DateCreated = DateTime.Now;

                db.Students.Add(std);
                db.SaveChanges();
                var id = std.ID;




                User regusr = new User();
                regusr.User_Name = registermodel.Name;
                regusr.User_Password = registermodel.User_Password;
                regusr.Date_Created = DateTime.Now;
                regusr.Status = "Pending";
                regusr.Is_Active =false;
                regusr.Is_Pending =true ;
                regusr.Is_Block = false;
                regusr.Email = registermodel.Student_Email;
                regusr.Is_Student = true;
                regusr.Is_Advisor = false;
                regusr.Std_Adv_Id = Convert.ToInt32(id);

                db.Users.Add(regusr);
                db.SaveChanges();

                return Json(new { url = "/Account/Login", msg = "Successfully Created Waiting For Approval From Admin", success = true }, JsonRequestBehavior.AllowGet);




            }
            else//advisor
            {
                Advisor adv = new Advisor();
                adv.AdvisorsName = registermodel.Name;
                adv.Advisor_Description = "";
                adv.Advisor_Email = registermodel.Student_Email;
                adv.Advisor_Uni_ID = registermodel.university_Id;
                adv.Advisor_Requested_Date = DateTime.Now;
                adv.IsBlock = false;
                adv.IsActive = false;
                adv.Advisors_Slot = 5;

                db.Advisors.Add(adv);
                db.SaveChanges();
                var id = adv.Id;




                User regusr = new User();
                regusr.User_Name = registermodel.Name;
                regusr.User_Password = registermodel.User_Password;
                regusr.Date_Created = DateTime.Now;
                regusr.Status = "Pending";
                regusr.Is_Active = false;
                regusr.Is_Pending = true;
                regusr.Is_Block = false;
                regusr.Email = registermodel.Student_Email;
                regusr.Is_Student = false;
                regusr.Is_Advisor = true;
                regusr.Std_Adv_Id = Convert.ToInt32(id);

                db.Users.Add(regusr);
                db.SaveChanges();
                return Json(new { url = "/Account/Login", msg = "Successfully Created Waiting For Approval From Admin", success = true }, JsonRequestBehavior.AllowGet);

            }


        }
    }
}