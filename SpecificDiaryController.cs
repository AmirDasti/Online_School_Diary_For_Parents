using FinalProject.Models;
using FinalProject.ViewModel;
using PagedList;
using System;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject.Areas.Teacher.Controllers
{
    public class SpecificDiaryController : Controller
    {
        // GET: Teacher/SpecificDiary
        OSDFinalEntities db = new OSDFinalEntities();
        /*private string[] selectedNames;*/

        /*  public ActionResult Index()
          {
              var username = Session["User-Name"].ToString();
              var Vm = new SpecificDiary()
              {
                  Dropdown_Section = DropdownSection(username),


              };

              return View(Vm);
          }*/


        public ActionResult Index(string id)
        {
            try
            {
                string Username = Session["User-Name"] as string;

                if (Username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userId = user.account_id;

                var sections = db.AssignSubjectTeachers.Where(x => x.Teacher_id == userId)
                                                       .Select(x => x.Section).Distinct()
                                                       .ToList();

                var students = db.AddStudentDatas.ToList();

                var vm = new SpecificDiaryVM
                {
                    Sections = sections,
                    AddStudentDatas = students,
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the Index action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Error");
            }
        }



        [HttpGet]
        public ActionResult GetTeachersubject(string section)
        {
            try
            {
                var studentlist = db.AddStudentDatas
        .Where(x => x.Section == section)
        .Select(x => new { Name = x.Name/*, ImageUrl = x.Image*/ })
        .ToList();



                if (studentlist != null)
                {
                    
                   
                    return Json(studentlist, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    

                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately (e.g., logging, returning error response)
                return Content("Error");
            }
        }

        [HttpGet]
        public ActionResult GetTeacher(string section)
        {
            try
            {
                string Username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userId = user.account_id;

                var classid = db.AssignSubjectTeachers.Where(x =>x.Teacher_id==userId && x.Section == section).Select(x => x.tid).ToList();
                var subject = db.SubjecTeachers.Where(x => classid.Contains(x.tid)).Select(x => x.Subject).ToList();

                if (subject != null)
                {


                    return Json(subject, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately (e.g., logging, returning error response)
                return Content("Error");
            }
        }





        [HttpPost]
        public ActionResult Index(SpecificDiaryVM obj, string[] selectedName)
        {
            try
            {
                if (selectedName == null || selectedName.Length == 0)
                {
                    ModelState.AddModelError(string.Empty, "No students selected.");
                    // Optionally, you can handle this case differently, such as displaying an error message to the user
                    return RedirectToAction("Index", obj);
                }

                string username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;

                foreach (string nameStr in selectedName)
                {
                    string[] names = nameStr.Split(':'); // assuming the delimiter is a colon
                    foreach (string name in names)
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            var Name = db.AddStudentDatas.FirstOrDefault(x => x.Name == name);
                            var rollno = Name.RollNO;

                            // Save the name and other details to the database
                            SpecificDiary d = new SpecificDiary();

                            d.Date = DateTime.Now;
                            d.RollNo = rollno;
                            d.Std_Name = name;
                            d.subject = obj.subject;
                            d.section = obj.section;
                            d.teacher_id = userId;
                            d.detail = obj.detail;

                            db.SpecificDiaries.Add(d);
                            SpDiaryHistory ob = new SpDiaryHistory();
                            ob.Date = DateTime.Now;
                            ob.Std_Name = name;
                            ob.subject = obj.subject;
                            ob.section = obj.section;
                            ob.teacher_id = userId;
                            d.RollNo = rollno;
                            ob.detail = obj.detail;
                            db.SpDiaryHistories.Add(ob);

                            db.SaveChanges();

                            TempData["Message"] = "Send Successfully Diary ";
                        }
                    }
                }

                // Clear the model state and return the view
                ModelState.Clear();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred");
                    Console.WriteLine("An error occurred in the Index action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return RedirectToAction("Index");
            }
        }











        /* public List<SelectListItem> getTutotors()
         {
             var tutors = db.AssignSubjectTeachers.Select(t => new { t.Section, t.tid }).Distinct().ToList();
             List<SelectListItem> items = new List<SelectListItem>();
             foreach (var tutor in tutors)
             {
                 items.Add(new SelectListItem()
                 {
                     Text = tutor.Section,
                     Value = tutor.tid.ToString()
                 });
             }
             return items;
         }
         public List<SelectListItem> DropdownSection(string username)
         {
             var tutors = db.AssignSubjectTeachers.Select(t => new { t.Section, t.tid }).Distinct().ToList();
             List<SelectListItem> items = new List<SelectListItem>();
             foreach (var tutor in tutors)
             {
                 items.Add(new SelectListItem()
                 {
                     Value = tutor.tid.ToString(),
                     Text = tutor.Section,

                 });
             }
             return items;
         }

         [HttpGet]
         public JsonResult getSUbject(string username)
         {
             var user = db.AssignSubjectTeachers.Where(x => x.Section == username).FirstOrDefault();
             if (user == null)
             {
                 return Json(new List<string>() { "Bal", "Contruction" }, behavior: JsonRequestBehavior.AllowGet);

             }
             List<String> subjs = db.SubjecTeachers.Where(s => s.sbt_id == user.tid).Select(x => x.Subject).Distinct().ToList();
             if (user == null)
             {
                 return Json(new List<string>() { "No", "Subjects", "enrooled", "by", "Contruction" }, behavior: JsonRequestBehavior.AllowGet);

             }
             return Json(subjs, behavior: JsonRequestBehavior.AllowGet);
         }


         [HttpGet]
         public JsonResult getStudent(string username)
         {

             var user = db.AssignSubjectTeachers.Where(x => x.Section == username).FirstOrDefault();
             if (user == null)
             {
                 return Json(new List<string>() { "No Data" }, behavior: JsonRequestBehavior.AllowGet);

             }
             List<string> subjs = db.AddStudentDatas
      .Where(s => s.Section == user.Section)
      .Select(x => x.Name + " : " + x.RollNO)
      .ToList();
             if (user == null)
             {
                 return Json(new List<string>() { "No", "Subjects", "enrooled", "by", "Contruction" }, behavior: JsonRequestBehavior.AllowGet);

             }
             return Json(subjs, behavior: JsonRequestBehavior.AllowGet); ;
         }

        */

        /*public List<SelectListItem> getallStudent()
        {
            var tutors = db.AddStudentDatas.Select(x => new { x.Name, x.Stdid }).ToList();
            List<SelectListItem> item = new List<SelectListItem>();
            foreach (var tutor in tutors)
            {

                item.Add(new SelectListItem()
                {
                    Text = tutor.Name,
                    Value = tutor.Stdid.ToString()
                });

            }
            return item;

        }*/
        /* public List<SelectListItem> getsubjets()
         {
             var tutors = db.AssignSubjectTeachers.Select(t => new { t.Username, t.tid }).ToList();
             List<SelectListItem> items = new List<SelectListItem>();
             foreach (var tutor in tutors)
             {
                 items.Add(new SelectListItem()
                 {
                     Text = tutor.Username,
                     Value = tutor.tid.ToString()
                 });
             }
             return items;
         }*/

    }
}