using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Areas.Principal.Controllers
{
    public class WholeNotificationController : Controller
    {
        // GET: Principal/WholeNotification
        OSDFinalEntities db = new OSDFinalEntities();
        public ActionResult Index()
        {
            string username = Session["User-Name"] as string;

            if (username == null)
            {
                Session.Abandon();
                return RedirectToAction("Login", "Home", new { area = "" });

            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(NotificationVM obj, string[] UserType)
        {
            try
            {
                if (UserType == null || UserType.Length == 0)
                {
                    ViewBag.ErrorMessage = "Please select the data";
                    return RedirectToAction("Index");
                }

                string username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(x => x.Username == username);
                int userid = user.account_id;

                foreach (var usertype in UserType)
                {
                    Notification d = new Notification();
                    d.ndate = DateTime.Now;
                    d.Principal_id = userid;
                    d.Notice = obj.Notice;
                    d.Usertype = usertype;

                    db.Notifications.Add(d);
                    db.SaveChanges();
                }

                return View();
            }
            catch (Exception ex)
            {
                
                return RedirectToAction("Index");
            }
        }



        public ActionResult Reply(string details, int teacherid)
        {
            try {
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
                // Return an appropriate error view or redirect to an error page
                return View("Error");
            }

        }
        [HttpPost]
        public ActionResult Reply(Whole_response obj, string details, string teacherName)
        {
            try { 
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
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an appropriate error view or redirect to an error page
                return View("Error");
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
                // handle situation when user does not have a child associated with their account
                return View("NoChildFound");
            }
            else
            {

                var complaints = db.Whole_response.Where(c => Complaint.Contains(c.date.ToString())).OrderBy(c => c.date).Distinct().ToList();




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
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an appropriate error view or redirect to an error page
                return View("Error");
            }


        }






        public ActionResult ViewResponse(DateTime? date)
        {
            try {
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
                    using (var db = new OSDFinalEntities())
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
            }

            // handle empty or invalid date parameter
            // For example, you can redirect to an error page or display a message
            return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an appropriate error view or redirect to an error page
                return View("Error");
            }
        }


        private int GetViewComplaintCount()
        {
            try
            {
                // Logic to retrieve the count of new, unchecked notifications from your data source
                // For example:
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
                // Handle the exception here, you can log the exception or perform any necessary actions
                Console.WriteLine("An error occurred: " + ex.Message);
                // You can also throw the exception to propagate it further if required
                throw;
            }
        }

        private int GetViewFeeNoticetCount()
        {
            try { 

            string Username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
            int userid = user.account_id;
            var users = db.AddStudentDatas.Where(u => u.parent_id == userid).Select(x => x.RollNO).ToList();
            int newFeeNoticeCount = 0;
            foreach (var roll in users)
            {
                // Logic to retrieve the count of new, unchecked notifications from your data source
                // For example:
                int count = db.FeeNotices.Where(c => c.Checked == false && c.RollNo == roll).Count();
                if (count > 0)
                {
                    newFeeNoticeCount++;

                }
                else
                {
                    newFeeNoticeCount = 0;
                }

            }


            return newFeeNoticeCount;
            }
            catch (Exception ex)
            {
                // Handle the exception here, you can log the exception or perform any necessary actions
                Console.WriteLine("An error occurred: " + ex.Message);
                // You can also throw the exception to propagate it further if required
                throw;
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
                // Handle the exception here, you can log the exception or perform any necessary actions
                Console.WriteLine("An error occurred: " + ex.Message);
                // You can also throw the exception to propagate it further if required
                throw;
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




    }
}