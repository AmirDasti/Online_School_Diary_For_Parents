using Antlr.Runtime.Misc;
using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Dynamic;
using System.EnterpriseServices;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject.Areas.Parent.Controllers
{
    public class ChildDetailsController : Controller
    {
        // GET: Parent/ChildDetails
        OSDFinalEntities db = new OSDFinalEntities();
      

        [HttpGet]
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
                var std = db.AddStudentDatas.Where(x => x.parent_id == userId).ToList();

                ViewBag.child = std;

                int newViewComplainCount = GetViewComplaintCount();
                ViewBag.NewViewComplaintCount = newViewComplainCount;

                int newViewFeeNoticeCount = GetViewFeeNoticetCount();
                ViewBag.NewViewFeeNoticeCount = newViewFeeNoticeCount;

                int newComplaintReplyCount = GetComplaintReplyCount();
                ViewBag.NewComplaintReplyCount = newComplaintReplyCount;

                int newPrincipalCount = GetViewPrincipalCount();
                ViewBag.NewPrincipalCount = newPrincipalCount;
                

                return View(std);
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error response or redirect to an error page
                return View();
            }
        }




        public ActionResult ViewDiary(string section, string name)
        {
            try
            {
                string username = Session["User-Name"] as string;

                if (username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }
                var std = db.AddStudentDatas.FirstOrDefault(x => x.Name == name);
                var rollno = std.RollNO;
               
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;
                var currentDate = DateTime.Now;
                var sectionDiaries = db.SectionDiaries
     .Where(x => x.section == section && DbFunctions.TruncateTime(x.Date) == currentDate.Date)
     .OrderByDescending(x => x.Date)
     .ToList();

                var specificDiaries = db.SpecificDiaries
     .Where(x => x.section == section &&x.RollNo==rollno&& DbFunctions.TruncateTime(x.Date) == currentDate.Date)
     .OrderByDescending(x => x.Date)
     .ToList();

                var viewModel = new DiaryViewModel
                {
                    SectionDiaries = sectionDiaries,
                    SpecificDiaries = specificDiaries
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error response or redirect to an error page
                return View(); 
            }
        }


        public ActionResult CommentDairy(string details, int teacherid)
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
                // Return an error response or redirect to an error page
                return Content("An error occurred");
            }
        }
        [HttpPost]
        public ActionResult CommentDairy(CommentDiary obj, string details, string teacherName)
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
                CommentDiary sd = new CommentDiary()
                {
                     Sender_id= userId,
                   /* date = DateTime.Now,*/
                   RollNo= userId,
                   subject=originalcomplaint,

                    Comment = obj.Comment,
                    Receiver_id = teacherid,
                    OrginalDiary = originalcomplaint,
                };

                db.CommentDiaries.Add(sd);
                db.SaveChanges();

                sd.Receiver_id = 0;
                sd.Sender_id = 0;
                sd.Comment = "";
                ModelState.Clear();

                // Return the view with the new whole_response object as the model
                return View(sd);
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error response or redirect to an error page
                return View();
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
                // Return an error response or redirect to an error page
                return Content("An error occurred");
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
                    receiver_id = userId,
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
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error response or redirect to an error page
                return View();
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
                    .Select(x => x.date.ToString()).Distinct()
                    .ToList();

                if (userId == 0)
                {
                    // Handle the situation when the user does not have a child associated with their account
                    return View("NoChildFound");
                }
                else
                {
                    var complaints = db.Whole_response.Where(c => Complaint.Contains(c.date.ToString())).OrderBy(c => c.date).Distinct().ToList();

                    var viewModel = new ComplaintViewModel
                    {
                        whole_Responses = complaints,
                    };

                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error response or redirect to an error page
                return View();
            }
        }






        public ActionResult ViewResponse(DateTime? date)
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

                    List<string> Reciver = db.Whole_response
                        .Where(x => x.sender_id == userId && DbFunctions.TruncateTime(x.date) == parsedDate.Date)
                        .Select(x => x.date.ToString())
                        .ToList();

                    if (ReciverName != null && ReciverName.Any())
                    {
                        var comp = db.Whole_response.Where(x => ReciverName.Contains(x.date.ToString())).OrderByDescending(x => x.date).ToList();
                        var comps = db.Whole_response.Where(x => Reciver.Contains(x.date.ToString())).OrderByDescending(x => x.date).ToList();

                        var viewModel = new ResponseViewModel
                        {
                            whole_Responses = comp,
                            whole_Response = comps
                        };

                        return View(viewModel);
                    }
                }

                // Handle empty or invalid date parameter
                // For example, you can redirect to an error page or display a message
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error response or redirect to an error page
                return View();
            }
        }


        private int GetViewComplaintCount()
        {
            try
            {
                string Username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userid = user.account_id;
                var users = db.AddStudentDatas.Where(u => u.parent_id == userid).Select(x => x.Name).ToList();
                int newSpecificCount = 0;
                int zero = 0;
                foreach (var name in users)
                {
                    int count = db.specific_complaint.Where(c => c.Checked == false && c.Std_Name == name).Count();
                    if (count > 0)
                    {
                        newSpecificCount += count;
                    }
                    else
                    {
                        zero = 0;
                        break;
                    }
                }

                int newwholeCount = db.complaints.Where(c => c.Checked == false && c.reaciver_id == userid).Count();

                int totalcount = newSpecificCount + newwholeCount;

                return totalcount;
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return a default count or an error indicator
                return -1; // Return -1 to indicate an error occurred
            }
        }


        private int GetViewFeeNoticetCount()
        {
            
            try
            { 
            string Username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
            int userid = user.account_id;
            var users = db.AddStudentDatas.Where(u => u.parent_id == userid).Select(x => x.RollNO).ToList();
                int newSpecificCount = 0;
                int zero = 0;
                foreach (var roll in users)
                {
                    int count = db.FeeNotices.Where(c => c.Checked == false && c.RollNo == roll).Count();
                    if (count > 0)
                    {
                        newSpecificCount += count;
                    }
                    else
                    {
                        zero = 0;
                        break;
                    }
                }

              
                int totalcount = newSpecificCount ;

                return totalcount;
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return a default count or an error indicator
                return -1; // Return -1 to indicate an error occurred
            }
}
        private int GetComplaintReplyCount()
        {
            try { 
            string Username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
            int userid = user.account_id;
           
               int newFeeNoticeCount = db.Whole_response.Where(c => c.Checked == false && c.receiver_id == userid).Count();

                          


            return newFeeNoticeCount;
                }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return a default count or an error indicator
                return -1; // Return -1 to indicate an error occurred
            }
}
        /* private int GetPrincipalNotiCount()
         {
             string Username = Session["User-Name"].ToString();
             var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
             int userId = user.account_id;

             // Logic to retrieve the count of new, unchecked notifications for the given username from your data source
             // For example:
             int newPrincipalNotiCount = db.Notifications.Where(c => c.Checked == false && c.Re == username).Count();

             int newSpecificPrincipalCount = db.SpecificNotifications.Where(c => c.Checked == false && c.Username == username).Count();

             int totalNotification = newPrincipalNotiCount + newSpecificPrincipalCount;
             return totalNotification;
         }*/

        private int GetViewPrincipalCount()
        {

            try
            {
                string Username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userid = user.account_id;
                var users = db.AddStudentDatas.Where(u => u.parent_id == userid).Select(x => x.RollNO).ToList();
                int newSpecificCount = 0;
                int zero = 0;
                foreach (var roll in users)
                {
                    int count = db.SpecificNotifications.Where(c => c.Checked == false && c.Receiver_id == userid).Count();
                    if (count > 0)
                    {
                        newSpecificCount += count;
                    }
                    else
                    {
                        zero = 0;
                        break;
                    }
                }


                int totalcount = newSpecificCount;

                return totalcount;
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return a default count or an error indicator
                return -1; // Return -1 to indicate an error occurred
            }
        }


        public ActionResult DiaryResponse()
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

                var currentDate = DateTime.Now;

                var replies = db.CommentDiaries.Where(x => x.Receiver_id == userId).ToList();


                var vm = new CommentDiaryVM
                {

                    CommentDiaries = replies,

                };

                return View(vm);
            }



            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error response or redirect to an error page
                return RedirectToAction("Index");
            }

        }


        [HttpGet]
        public ActionResult ReplyDiary(string details, int teacherid)
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
                // Return an error response or redirect to an error page
                return Content("An error occurred");
            }
        }

        [HttpPost]
        public ActionResult ReplyDiary(CommentDiary obj, string details, string teacherName)
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
                CommentDiary sd = new CommentDiary()
                {
                    Sender_id = userId,
                    RollNo = teacherid,
                    subject = originalcomplaint,
                    /* date = DateTime.Now,*/
                    Comment = obj.Comment,
                    Receiver_id = teacherid,
                    OrginalDiary = originalcomplaint,
                };

                db.CommentDiaries.Add(sd);
                db.SaveChanges();

                sd.Receiver_id = 0;
                sd.Sender_id = 0;
                sd.Comment = "";
                ModelState.Clear();

                // Return the view with the new whole_response object as the model
                return View(sd);
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error response or redirect to an error page
                return View();
            }
        }







    }
}