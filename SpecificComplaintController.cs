using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject.Areas.Teacher.Controllers
{
    public class SpecificComplaintController : Controller
    {
        // GET: Teacher/SpecificComplaint
        OSDFinalEntities db = new OSDFinalEntities();
        public ActionResult Index()
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

                var vm = new SpecificComplaintVM
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
                return View();
            }
        }


        [HttpPost]
        public ActionResult Index(SpecificComplaintVM obj, string[] selectedName)
        {
            try
            {
                string username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;

                if (selectedName != null)
                {
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
                                specific_complaint d = new specific_complaint();

                                d.complaint_date = DateTime.Now;
                                d.Std_Name = name;
                                d.RollNo = rollno;
                                d.teacher_id = userId;
                                d.subject = obj.Subject;
                                d.section = obj.section;
                                d.detail = obj.detail;

                                db.specific_complaint.Add(d);
                                db.SaveChanges();

                                
                            }
                        }
                    }
                }
                TempData["Message"] = "Data inserted ";
                // Clear the model state and redirect to the Index action
                ModelState.Clear();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the Index action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return RedirectToAction("Index");
            }
        }





        [HttpGet]
        public ActionResult GetTeachersubject(string section)
        {
            try
            {
                var studentlist = db.AddStudentDatas.Where(x => x.Section == section).Select(x => x.Name).ToList();

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

                var classid = db.AssignSubjectTeachers.Where(x => x.Teacher_id == userId && x.Section == section).Select(x => x.tid).ToList();
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









        /*public JsonResult getStudent(string username)
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
        */

    }
}