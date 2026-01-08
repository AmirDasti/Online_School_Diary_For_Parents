using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Areas.Parent.Controllers
{
    public class CatagoryComplaintController : Controller
    {
        // GET: Parent/CatagoryComplaint
        OSDFinalEntities db = new OSDFinalEntities();
        public ActionResult Index()
        {
            try
            {
                string username = Session["User-Name"] as string;

                if (username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }
                var tutor = db.Users.Select(x => x.UserTypes).ToList();

                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;

                var std = db.AddStudentDatas.FirstOrDefault(x => x.parent_id == userId);
                var name = std.Name;

                var Student = db.AddStudentDatas.Where(x => x.parent_id == userId).Select(x => x.Name).ToList();
                var TutorUsernames = (from ast in db.AssignSubjectTutors
                                      join ca in db.CreateAccounts on ast.Tutor_id equals ca.account_id
                                      select ca.Username).Distinct().ToList();


                var complaint = db.complaints.Where(x => x.reaciver_id == userId).ToList();
                var spec = db.specific_complaint.Where(x => x.Std_Name == name).ToList();


                var viewModel = new ComplaintsVM
                {
                    Complaints=complaint,
                    Specific_Complaints=spec,
                    Tutors = TutorUsernames,
                    Students = Student,
                    Types = tutor,
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                return View("Error"); // Display an error view
            }
        }

        [HttpGet]
        public ActionResult GetStudentSubjects(string stdName)
        {
            try
            {
                // Retrieve the selected student from the database
                var student = db.AddStudentDatas.FirstOrDefault(x => x.Name == stdName);

                if (student != null)
                {
                    var className = student.Class; // Assuming the class name is stored in the 'Class' property of the 'AddStudentData' model

                    var classId = db.Classes.Where(x => x.Class1 == className).Select(x => x.Class_id).FirstOrDefault();

                    List<string> subjects = db.ClassSubjects.Where(x => x.Class_ID == classId).Select(x => x.Subjects).ToList();

                    // Return the list of subjects
                    return Json(subjects, JsonRequestBehavior.AllowGet);
                }

                return Content(""); // Return an empty response if the student is not found
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                return Content(""); // Return an empty response or an error message
            }
        }

        [HttpGet]


        public ActionResult GetTeachername(string subject)
        {
            try
            {

                // Retrieve the teacher's name based on the selected subject from the database
                var teacherid = db.SubjecTeachers.Where(t => t.Subject == subject).Select(t => t.tid).FirstOrDefault();

                if (teacherid != 0)
                {
                    var teachename = db.CreateAccounts.Where(x => x.account_id == teacherid).Select(x => x.Username).FirstOrDefault();
                    return Json(teachename, JsonRequestBehavior.AllowGet);
                }

                return Content(""); // Return an empty response if the teacher is not found
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                return Content(""); // Return an empty response or an error message
            }
        }


        /*   [HttpGet]
           public ActionResult GetTeacherBySubject(string subject)
           {
               // Retrieve the teacher's name based on the selected subject from the database
               var teacherid = db.SubjecTeachers.Where(t => t.Subject == subject).Select(t => t.Teacher_id).FirstOrDefault();

               if (teacherid != null)
               {

                   var teachename = db.CreateAccounts.Where(x => x.account_id == teacherid).Select(x => x.Username).FirstOrDefault();
                   return Json(teachename, JsonRequestBehavior.AllowGet);
               }

               return Content(""); // Return an empty response if the teacher is not found
           }
   */

        [HttpPost]
        public ActionResult Index(ComplaintsVM obj, string Recivername)
        {
            try
            {
                string username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.FirstOrDefault(u => u.Username == username);
                int userId = user.account_id;

                var teacher = db.CreateAccounts.SingleOrDefault(u => u.Username == Recivername);
                int receiverId = teacher.account_id;

                string studentName = obj.Std_Name;

                int rollNo = db.AddStudentDatas.Where(x => x.Name == studentName && x.parent_id == userId).Select(x => x.RollNO).FirstOrDefault();

                var com = db.complaints.Where(x => x.sender_id == receiverId).ToList();

                var tutor = db.Users.Select(x => x.UserTypes).ToList();

                var naem = obj.Std_Name;

                var std = db.AddStudentDatas.FirstOrDefault(x => x.parent_id == userId);
                var name = std.Name;

                var Student = db.AddStudentDatas.Where(x => x.parent_id == userId).Select(x => x.Name).ToList();
                var TutorUsernames = (from ast in db.AssignSubjectTutors
                                      join ca in db.CreateAccounts on ast.Tutor_id equals ca.account_id
                                      select ca.Username).Distinct().ToList();


                var complaint = db.complaints.Where(x => x.sender_id == receiverId).ToList();
                var spec = db.specific_complaint.Where(x => x.Std_Name == naem &&x.teacher_id==receiverId).ToList();


                var viewModel = new ComplaintsVM
                {
                    Complaints = complaint,
                    Specific_Complaints = spec,
                    Tutors = TutorUsernames,
                    Students = Student,
                    Types = tutor,
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                return View(); // Return an error view or redirect to an error page
            }
        }


        [HttpGet]
        public JsonResult getSubject(string username, string subject, string studentname)
        {
            try
            {
                var name = db.AddStudentDatas.FirstOrDefault(x => x.Name == studentname);
                int roll = name.RollNO;
                if (username == "Teacher")
                {
                    // Get the list of usernames for the teacher's assigned sections
                    string Username = Session["User-Name"].ToString();
                    var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                    int userId = user.account_id;
                    var assignedSections = db.AddStudentDatas.Where(x => x.parent_id == userId)

                                                .Select(x => x.Section)
                                                .Distinct()
                                                .ToList();

                    // Get the list of usernames for the teachers in the same sections as the current teacher
                    var tutor = db.AddStudentDatas.Where(x => x.parent_id == userId)

                                                .Select(x => x.Section)
                                                .Distinct()
                                                .ToList();

                    if (tutor == null)
                    {
                        return Json(new List<string>() { "No tutor found" }, behavior: JsonRequestBehavior.AllowGet);
                    }

                    // Get the list of subjects assigned to the teacher's sections
                    var subjs = db.AssignSubjectTeachers
         .Where(x => assignedSections.Contains(x.Section)).Select(x => x.tid).ToList();


                    var tids = db.SubjecTeachers.Where(x => subjs.Contains(x.tid) && x.Subject == subject).Select(x => x.tid).ToList();

                    var teacherid = db.AssignSubjectTeachers.Where(x => tids.Contains(x.tid)).Select(x => x.Teacher_id).ToList();

                    var Users = db.CreateAccounts.Where(x => teacherid.Contains(x.account_id)).Select(x => x.Username).ToList();





                    if (Users.Count == 0)
                    {
                        return Json(new List<string>() { "No subjects enrolled" }, behavior: JsonRequestBehavior.AllowGet);
                    }

                    return Json(Users, behavior: JsonRequestBehavior.AllowGet);
                }
                else if (username == "Tutor")
                {
                    // Get the list of usernames for the teacher's assigned sections
                    string Username = Session["User-Name"].ToString();
                    var user = db.CreateAccounts.FirstOrDefault(u => u.Username == Username);
                    int userId = user.account_id;

                    var assignedSections = db.Assign_Tutor_Stds.Where(x => x.parent_id == userId && x.subject == subject && x.RollNo == roll)

                                                .Select(x => x.Tutor_ID)
                                                .Distinct()
                                                .ToList();

                    // Get the list of usernames for the teachers in the same sections as the current teacher
                    var tutor = db.AddStudentDatas.Where(x => x.parent_id == userId)

                                                .Select(x => x.Section)
                                                .Distinct()
                                                .ToList();

                    if (tutor == null)
                    {
                        return Json(new List<string>() { "No tutor found" }, behavior: JsonRequestBehavior.AllowGet);
                    }

                    // Get the list of subjects assigned to the teacher's sections
                    var subjs = db.CreateAccounts
        .Where(x => assignedSections.Contains(x.account_id)).Select(x => x.Username).ToList();


                    /*   var tids = db.SubjecTutors.Where(x => subjs.Contains(x.s_id) && x.Subject == subject).Select(x => x.s_id).ToList();

                       var teacherid = db.AssignSubjectTutors.Where(x => tids.Contains(x.s_id)).Select(x => x.Tutor_id).ToList();

                       var Users = db.CreateAccounts.Where(x => teacherid.Contains(x.account_id)).Select(x => x.Username).ToList();
       */


                    if (subjs.Count == 0)
                    {
                        return Json(new List<string>() { "No Assign Tutor" }, behavior: JsonRequestBehavior.AllowGet);
                    }

                    return Json(subjs, behavior: JsonRequestBehavior.AllowGet);


                }
                else if (username == "Principal")
                {
                    // Get the list of usernames for the teacher's assigned sections
                    string Username = Session["User-Name"].ToString();
                    var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                    int userId = user.account_id;



                    // Get the list of subjects assigned to the teacher's sections
                    List<string> subjs = db.CreateAccounts.Where(u => u.UserType == username).Select(u => u.Username).ToList();





                    if (subjs.Count == 0)
                    {
                        return Json(new List<string>() { "No Assign Tutor" }, behavior: JsonRequestBehavior.AllowGet);
                    }

                    return Json(subjs, behavior: JsonRequestBehavior.AllowGet);


                }
                else
                {
                    return Json(new List<string>() { "Invalid userType" }, behavior: JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                return Json(new List<string>() { "Error occurred" }, behavior: JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Reply(string details, int teacherid)
        {
            try
            {
                string username = Session["User-Name"] as string;

                if (username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }
                string teacherName = db.CreateAccounts
                                    .Where(u => u.account_id == teacherid)
                                    .Select(u => u.Username)
                                    .FirstOrDefault();

                // Store the section and Username parameters in session variables
                Session["Details"] = details;
                Session["ReplyUsername"] = teacherName;
                return View();
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Redirect to an error page or display an error message
                return View();
            }
        }

        [HttpPost]
        public ActionResult Reply(Whole_response obj, string details, string teacherName)
        {
            try
            {
                string username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;

                var teacher = db.CreateAccounts.SingleOrDefault(u => u.Username == teacherName);
                int teacherid = teacher.account_id;

                string originalcomplaint = details;

                // Create a new whole_response object and save it to the database
                Whole_response sd = new Whole_response()
                {
                    sender_id = userId,
                    date = DateTime.Now,
                    respdetail = obj.respdetail,
                    receiver_id = teacherid,
                    OriginalComplaint = originalcomplaint,
                };

                db.Whole_response.Add(sd);
                db.SaveChanges();

                sd.receiver_id = 0;
                sd.sender_id = 0;
                sd.respdetail = "";
                ModelState.Clear();

                // Return the view with the new whole_response object as the model
                return View();
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Redirect to an error page or display an error message
                return View();
            }
        }
    }


    
}