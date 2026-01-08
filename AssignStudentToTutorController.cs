using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace FinalProject.Areas.Parent.Controllers
{
    public class AssignStudentToTutorController : Controller
    {
        // GET: Parent/AssignStudentToTutor

        OSDFinalEntities db = new OSDFinalEntities();



        public ActionResult Index()
        {
            try
            {
                string username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;
                var Student = db.AddStudentDatas.Where(x => x.parent_id == userId).Select(x => x.Name).ToList();
                var TutorUsernames = (from ast in db.AssignSubjectTutors
                                      join ca in db.CreateAccounts on ast.Tutor_id equals ca.account_id
                                      select ca.Username).Distinct().ToList();

                var viewModel = new AssignTutorStudentVM
                {
                    Tutors = TutorUsernames,
                    Students = Student,
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error view or redirect to an error page
                return View("Error");
            }
        }


        [HttpPost]
        public ActionResult Index(AssignTutorStudentVM obj)
        {
            try
            {
                string username = Session["User-Name"].ToString();
                string name = obj.ParentName;
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == name);
                int userId = user.account_id;
                string tutorname = obj.tutor_name;

              
                var tutor = db.CreateAccounts.SingleOrDefault(u => u.Username == tutorname);
                int tutorid = tutor.account_id;
                Assign_Tutor_Std d = new Assign_Tutor_Std();
                d.Std_Name = obj.Std_Name;
                d.parent_id = userId;
                d.Tutor_ID = tutorid;
                d.Request = obj.Request.ToString() == "0";

                d.subject = obj.subject;
                d.Section = obj.Section;
                d.@class = obj.Section;
                d.Day = obj.Day;
               /* d.Timeing = obj.Timeing;*/
                d.Auto_diary = obj.Auto_diary;
               
                db.Assign_Tutor_Stds.Add(d);
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }



            return RedirectToAction("Index");
            }
        public ActionResult GetStudentDetails(string studentId)
        {
            try
            {
                // Query the database for the student details using the studentId
                var student = db.AddStudentDatas.FirstOrDefault(s => s.Name == studentId);

                if (student == null)
                {
                    // Return an error response if the student does not exist
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Student not found.");
                }

                // Query the database for the parent details using the parentId
                var parent = db.CreateAccounts.SingleOrDefault(p => p.account_id == student.parent_id);

                if (parent == null)
                {
                    // Return an error response if the parent does not exist
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Parent not found.");
                }

                // Build a JSON response object with the parent name and section details
                var response = new
                {
                    ParentName = parent.Username,
                    Section = student.Section
                };

                // Return the JSON response
                return Json(response);
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error response or redirect to an error page
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred.");
            }
        }


        [HttpGet]
        public JsonResult getSubject(string username)
        {
            try
            {
                var user = db.CreateAccounts.FirstOrDefault(x => x.Username == username);
                if (user == null)
                {
                    return Json(new List<string>() { "No data found" }, behavior: JsonRequestBehavior.AllowGet);
                }

                var tutor = db.AssignSubjectTutors.FirstOrDefault(x => x.Tutor_id == user.account_id);
                if (tutor == null)
                {
                    return Json(new List<string>() { "No Subjects Enrolled" }, behavior: JsonRequestBehavior.AllowGet);
                }

                List<string> subjects = db.SubjecTutors.Where(st => st.s_id == tutor.Tutor_id)
                                                       .Select(st => st.Subject)
                                                       .ToList();
                /*   List<string> day = db.SubjecTutors.Where(st => st.Tutor_id == tutor.Tutor_id)
                                                          .Select(st => st.Day)
                                                          .ToList();
                   List<string> time = db.SubjecTutors.Where(st => st.Tutor_id == tutor.Tutor_id)
                                                          .Select(st => st.Timing)
                                                          .ToList();*/
                var result = new
                {
                    subjects,
                    /* day,
                     time*/
                };

                return Json(result, behavior: JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error response or redirect to an error page
                return Json(new { error = "An error occurred" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetSchedule(string subject)
        {
            try
            {
                var scheduleList = db.InsertSchedules.Where(s => s.Subjects == subject).ToList();
                var scheduleData = scheduleList.Select(s => new { Day = s.Day, /*Timeing = s.Timing*/ }).ToList();
                return Json(scheduleData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error response or redirect to an error page
                return Json(new { error = "An error occurred" }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}