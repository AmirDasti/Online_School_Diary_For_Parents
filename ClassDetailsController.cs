using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject.Areas.Teacher.Controllers
{
    public class ClassDetailsController : Controller
    {
        // GET: Teacher/ClassDetails
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

                var std = db.AssignSubjectTeachers.Where(x => x.Teacher_id == userId).Distinct().ToList();
                ViewBag.classes = std;

                var section = db.AssignSubjectTeachers.Where(x => x.Teacher_id == userId).Select(x => x.Section).Distinct().Count();
                ViewBag.section = section;

                List<int> tid = db.AssignSubjectTeachers.Where(x => x.Teacher_id == userId).Select(x => x.tid).Distinct().ToList();
                int subjectCount = db.SubjecTeachers.Where(x => tid.Contains(x.tid)).Select(x => x.Subject).Distinct().Count();
                ViewBag.subjectCount = subjectCount;

                var complaintIds = db.complaints.Where(x => x.sender_id == userId).Select(x => x.complaint_id).ToList();

                int newViewComplainCount = GetViewComplaintCount();
                ViewBag.NewViewComplaintCount = newViewComplainCount;

                int newComplaintReplyCount = GetComplaintReplyCount();
                ViewBag.NewComplainReplytCount = newComplaintReplyCount;

                /* int newViewFeeNoticeCount = GetViewFeeNoticetCount();
                 ViewBag.NewViewFeeNoticeCount = newViewFeeNoticeCount;*/

                var teacher = db.AssignSubjectTeachers.Where(x => x.Teacher_id == userId).Select(x => x.tid).ToList();
                var teachsection = db.AssignSubjectTeachers.Where(x => x.Teacher_id == userId).Select(x => x.Section).ToList();
                var subjects = db.SubjecTeachers.Where(x => teacher.Contains(x.tid)).Select(x => x.Subject).Distinct().ToList();

                var lsit = db.SubjecTeachers.Where(x => subjects.Contains(x.Subject)).ToList();
                var sectionlist = db.AssignSubjectTeachers.Where(x => teachsection.Contains(x.Section) &&x.Teacher_id==userId).Distinct().ToList();
                var tables = new AssignTeacherSubjectVM
                {
                    AssignSubjectTeachers = sectionlist,
                    SubjectTeacher = lsit,
                };

                return View(tables);



            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the Index action method: " + ex.Message);
                // Handle the exception or display an error message
                // You can redirect to an error page or return a specific View with an error message
                return View("Error");
            }
        }

        /*[HttpPost]
        public ActionResult MarkNotificationsAsRead()
        {
            // code to mark all the notifications as read goes here
            int newNotificationCount = 0;
            ViewBag.NewNotificationCount = newNotificationCount;
            return Json(new { success = true });
        }*/

        public ActionResult SetDiary(string id)
        {
            try
            {
                string username = Session["User-Name"] as string;

                if (username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }
                /*var section = id;*/
                string Username = Session["User-Name"].ToString();

                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userId = user.account_id;

                var sections = db.AssignSubjectTeachers.Where(x => x.Teacher_id == userId)
                                                       .Select(x => x.Section).Distinct()
                                                       .ToList();

                ViewBag.tutor = sections;

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the SetDiary action method: " + ex.Message);
                // Handle the exception or display an error message
                // You can redirect to an error page or return a specific View with an error message
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult SetDiary(SectionDiaryVM obj)
        {

            string username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;

            try
            {
                SectionDiary student = new SectionDiary();
                student.Date = DateTime.Now;
                student.section = obj.section;
                student.subject = obj.subject;
                student.detail = obj.detail;
                student.teacher_id = userId;

                db.SectionDiaries.Add(student);
                db.SaveChanges();

                DiaryHistory d = new DiaryHistory();
                d.Date = DateTime.Now;
                d.section = obj.section;
                d.subject = obj.subject;
                d.detail = obj.detail;
                d.teacher_id = userId;

                db.DiaryHistories.Add(d);
                db.SaveChanges();

                TempData["Message"] = "Send Successfully Diary ";
            }
            catch (Exception ex)
            {
                // Log the error to a file or the console
                Console.WriteLine(ex.Message);
                ViewBag.Message = string.Format("An error occurred while saving the data.");
            }

            // Check if ModelState is valid before clearing it
            if (ModelState.IsValid)
            {
                ModelState.Clear();
            }
            else
            {
                ViewBag.Message = string.Format("Invalid model state.");
            }

            /* var dataManager = new SectionDiary(); // Replace with your data manager class
             dataManager.DeleteOldSectionDiary();
             var dManager = new SpecificDiary(); // Replace with your data manager class
             dManager.DeleteOldSectionDiary();*/
            /*return RedirectToAction("Index"); // Replace with the name of your action method
*/
            
            
            return RedirectToAction(nameof(SetDiary));
        }

        public ActionResult DeleteOldRecords()
        {
        /*    var dataManager = new SectionDiary(); // Replace with your data manager class
            dataManager.DeleteOldSectionDiary();
            var dManager = new SpecificDiary(); // Replace with your data manager class
            dManager.DeleteOldSectionDiary();*/
            return RedirectToAction("Index"); // Replace with the name of your action method
        }
        public void DeleteOldSectionDiary()
        {
            using (var db = new OSDFinalEntities())
            {
                var oldsectionDiary = db.SectionDiaries.Where(d => d.Date < DateTime.Now.AddMinutes(-1)).ToList();
                var oldSpDiary = db.SpecificDiaries.Where(d => d.Date < DateTime.Now.AddMinutes(-1)).ToList();
                db.SectionDiaries.RemoveRange(oldsectionDiary);
                db.SpecificDiaries.RemoveRange(oldSpDiary);
                db.SaveChanges();
            }
        }


        public List<SelectListItem> getTutotors()
        {
            try
            {
                var tutors = db.AssignSubjectTeachers.Select(t => new { t.Section, t.Teacher_id }).Distinct().ToList();
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (var tutor in tutors)
                {
                    items.Add(new SelectListItem()
                    {
                        Text = tutor.Section,
                        Value = tutor.Teacher_id.ToString()
                    });
                }
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the getTutotors method: " + ex.Message);
                // Handle the exception or display an error message
                // You can throw the exception again or return an empty list or a specific error indicator
                return new List<SelectListItem>(); // Return an empty list in case of an error
            }
        }



        [HttpGet]
        public JsonResult getSUbject(string username)
        {
            try
            {
                string usernam = Session["User-Name"].ToString();
                var users = db.CreateAccounts.SingleOrDefault(u => u.Username == usernam);
                int userId = users.account_id;

                var user = db.AssignSubjectTeachers.Where(x => x.Section == username && x.Teacher_id == userId).Select(x => x.tid).ToList();

                List<String> subjs = db.SubjecTeachers.Where(s => user.Contains(s.tid)).Select(x => x.Subject).Distinct().ToList();

                return Json(subjs, behavior: JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the getSUbject method: " + ex.Message);
                return Json(new List<string>(), behavior: JsonRequestBehavior.AllowGet);
            }
        }




        public ActionResult SendComplaint(string section,string subject)
        {
            try
            {
                string username = Session["User-Name"] as string;

                if (username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }
                /*var section = id;*/
                string Username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userId = user.account_id;


                string teacherName = db.CreateAccounts
                    .Where(u => u.account_id == userId)
                    .Select(u => u.Username)
                    .FirstOrDefault();

                // Store the section and Username parameters in session variables
                Session["Details"] = section;
                Session["subject"] = subject;
                Session["ReplyUsername"] = teacherName;


                var sections = db.AssignSubjectTeachers.Where(x => x.Teacher_id == userId)
                                                       .Select(x => x.Section)
                                                       .ToList();

                ViewBag.tutor = sections;
            }
            catch(Exception ex)
            {

            }
            return View();
        }
        [HttpPost]
        public ActionResult SendComplaint(AllComplaintVM obj,string section,string subject,string UserType)
        {

          
            try
            {
               /* var name = db.CreateAccounts.FirstOrDefault(x => x.UserType == UserType);
                int principalid = name.account_id;*/
                string username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;

                /* string reciver =Session["section"].ToString();*/
                complaint student = new complaint();

                /* student.RollNo = obj.RollNo;*/
                student.complaint_date = DateTime.Now;
               /* student.reaciver_id = principalid;*/
                student.section = section;
                student.subject = subject;
                student.sender_id = userId;
                student.detail = obj.detail;
                

                /*  student.section = section;
                  student.subject = subjects;*/


                db.complaints.Add(student);
              
                db.SaveChanges();
                TempData["Message"] = " Data Inserted Successfully";
                ModelState.Clear();

            }
            catch
            {


                ViewBag.Message = string.Format("  Successfully");
            }
            //return View();
            return RedirectToAction("Index");
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
                Console.WriteLine("An error occurred in the Reply action method: " + ex.Message);
                return RedirectToAction("Index"); // Redirect to a suitable action/view in case of an error
            }
        }

        [HttpPost]
        public ActionResult Reply(Whole_response obj, string details, string teacherName)
        {
            try
            {
                /* Session["Details"] = details;
                 Session["ReplyUsername"] = Username;*/
                string username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;

                var teacher = db.CreateAccounts.SingleOrDefault(u => u.Username == teacherName);
                int teacherid = teacher.account_id;
                //here is Sender Parameter Username but you use to hidden in view then this name save in data base
                /* int Sender = teacherid;*/
                string originalcomplaint = details;
                // Create a new whole_response object and save it to the database
                Whole_response sd = new Whole_response()
                {
                    receiver_id = userId,
                    /* teacher_id = Sender,*/
                    date = DateTime.Now,
                    respdetail = obj.respdetail,
                    sender_id = teacherid,
                    OriginalComplaint = originalcomplaint,
                };
                db.Whole_response.Add(sd);
                db.SaveChanges();
                sd.receiver_id = 0;
                sd.sender_id = 0;
                sd.respdetail = "";
                ModelState.Clear();
                // Return the view with the new whole_response object as the model
                return View(sd);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the Reply action method: " + ex.Message);
                return RedirectToAction("Index"); // Redirect to a suitable action/view in case of an error
            }
        }


        public ActionResult DateComplaintDetails()
        {
            try
            {
                string username = Session["User-Name"] as string;

                if (username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;

                int newViewComplainCount = GetViewComplaintCount();
                ViewBag.NewViewComplaintCount = newViewComplainCount;

                List<string> Complaint = db.Whole_response
                    .Where(x => x.receiver_id == userId)
                    .Select(x => x.date.ToString())
                    .Distinct()
                    .ToList();

                if (userId == 0)
                {
                    // handle situation when user does not have a child associated with their account
                    return View("NoChildFound");
                }
                else
                {
                    var complaints = db.Whole_response
                        .Where(c => Complaint.Contains(c.date.ToString()))
                        .OrderBy(c => c.date)
                        .Distinct()
                        .ToList();

                    var viewModel = new ComplaintViewModel
                    {
                        whole_Responses = complaints,
                        /*  specific_Complaints = specifics,*/
                    };

                    /* ViewBag.NewNotificationCount = GetNewNotificationCount();*/

                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the DateComplaintDetails action method: " + ex.Message);
                return RedirectToAction("Index"); // Redirect to a suitable action/view in case of an error
            }
        }






        public ActionResult ViewComplaintDate(DateTime? date)
        {
            try
            {
                string username = Session["User-Name"] as string;

                if (username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;

                var unseenComplaintsCount = db.Whole_response.Where(c => !c.Checked && c.receiver_id == userId).Count();

                if (unseenComplaintsCount > 0)
                {
                    var unseenComplaints = db.Whole_response.Where(c => !c.Checked && c.receiver_id == userId).ToList();
                    foreach (var complaint in unseenComplaints)
                    {
                        complaint.Checked = true;
                    }
                    db.SaveChanges();
                }

                if (date != null)
                {
                    var parsedDate = date.Value;

                    List<string> ReciverName = db.Whole_response
                        .Where(x => x.receiver_id == userId && DbFunctions.TruncateTime(x.date) == parsedDate.Date)
                        .Select(x => x.date.ToString())
                        .ToList();

                    if (ReciverName != null && ReciverName.Any())
                    {
                        using (var db = new OSDFinalEntities())
                        {
                            var comp = db.Whole_response
                                .Where(x => ReciverName.Contains(x.date.ToString()))
                                .OrderByDescending(x => x.date)
                                .ToList();

                            var viewModel = new ComplaintViewModel
                            {
                                whole_Responses = comp
                            };

                            return View(viewModel);
                        }
                    }
                }

                // handle empty or invalid date parameter
                // For example, you can redirect to an error page or display a message
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewComplaintDate action method: " + ex.Message);
                return RedirectToAction("Index"); // Redirect to a suitable action/view in case of an error
            }
        }


        private int GetViewComplaintCount()
        {
            try
            {
                string Username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userid = user.account_id;

                int newwholeCount = db.complaints.Where(c => c.Checked == false && c.reaciver_id == userid).Count();

                return newwholeCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the GetViewComplaintCount method: " + ex.Message);
                return 0; // Return a default value or handle the error in an appropriate manner
            }
        }


        private int GetComplaintReplyCount()
        {
            try
            {
                string Username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userid = user.account_id;

                int newwholeCount = db.Whole_response.Where(c => c.Checked == false && c.receiver_id == userid).Count();

                return newwholeCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the GetComplaintReplyCount method: " + ex.Message);
                return 0; // Return a default value or handle the error in an appropriate manner
            }
        }


        /*    private int GetPrincipalNoficationCount()
            {

                string Username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userid = user.account_id;

                    // Logic to retrieve the count of new, unchecked notifications from your data source
                    // For example:
                   int newPrincipalNotificationCount = db.Notifications.Where(c => c.Checked == false && c.G == roll).Count();




                return newPrincipalNotificationCount;
            }*/

    }
}
