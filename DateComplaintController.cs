using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Areas.Tutor.Controllers
{
    public class DateComplaintController : Controller
    {
        // GET: Tutor/DateComplaint
        OSDFinalEntities db = new OSDFinalEntities();
        public ActionResult Index()
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

            /* var names = db.AddStudentDatas.Where(x => x.parent_id == userId).Select(x => x.Name).ToList();

             foreach( var name in names)
             {
                 var allcom = db.specific_complaint.Where(x => !x.Checked && x.Std_Name == name);

                 foreach( var updated in allcom)
                 {
                     updated.Checked = true;
                 }
                 db.SaveChanges();

             }
             var unseenComplaints = db.complaints.Where(c => !c.Checked && c.reaciver_id==userId);

             foreach( var unseen in unseenComplaints)
             {

                 unseen.Checked = true;
             }
             db.SaveChanges();*/

            

            int newViewComplainCount = GetViewComplaintCount();
            ViewBag.NewViewComplaintCount = newViewComplainCount;


            // Update the Checked field for the unseen complaints in both tables


            // Save the changes to the database
            db.SaveChanges();


            List<string> Complaint = db.complaints

                .Where(x => x.reaciver_id == userId)
                .Select(x => x.complaint_date.ToString()).Distinct()
                .ToList();

            if (userId == 0)
            {
                // handle situation when user does not have a child associated with their account
                return View("NoChildFound");
            }
            else
            {

                var complaints = db.complaints.Where(c => Complaint.Contains(c.complaint_date.ToString())).OrderBy(c => c.complaint_date).Distinct().ToList();
               



                var viewModel = new ComplaintViewModel
                {
                    complaints = complaints,
               

                };
                ViewBag.NewNotificationCount = GetViewComplaintCount();
                return View(viewModel);
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View();
            }

        }

        public ActionResult ViewComplaintDate(DateTime? date)
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

            var unseenComplaintsCount = db.complaints.Where(c => !c.Checked && c.reaciver_id == userId).Count();

            if (unseenComplaintsCount > 0)
            {
                var unseenComplaints = db.complaints.Where(c => !c.Checked && c.reaciver_id == userId).ToList();
                foreach (var complaint in unseenComplaints)
                {
                    complaint.Checked = true;
                }
                db.SaveChanges();
            }

            if (date != null)
            {
                var parsedDate = date.Value;

                List<string> ReciverName = db.complaints
       .Where(x => x.reaciver_id == userId && DbFunctions.TruncateTime(x.complaint_date) == parsedDate.Date)
       .Select(x => x.complaint_date.ToString())
       .ToList();



                if (ReciverName != null && ReciverName.Any())
                {
                    using (var db = new OSDFinalEntities())
                    {
                        var comp = db.complaints.Where(x => ReciverName.Contains(x.complaint_date.ToString())).OrderByDescending(x => x.complaint_date).ToList();

                        var viewModel = new ComplaintViewModel
                        {
                            complaints = comp
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
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
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
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View();
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
                sender_id = userId,
                /* teacher_id = Sender,*/
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
            return View(sd);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Error");
            }
        }


        public ActionResult ViewResponse()
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

            if (string.IsNullOrEmpty(username))
            {
                ViewBag.Message = "User name not found in session.";
                return View();
            }

            List<string> ReciverName = db.Whole_response.Where(x => x.sender_id == userId).Select(x => x.respdetail).ToList();

            if (ReciverName.Any())
            {
                var comp = db.Whole_response.Where(x => ReciverName.Contains(x.respdetail)).OrderBy(x => x.date).ToList();

                var viewModel = new ResponseViewModel
                {
                    whole_Responses = comp,
                };

                return View(viewModel);
            }
            else
            {
                var viewModel = new ResponseViewModel
                {
                    whole_Responses = new List<Whole_response>(),
                };

                ViewBag.Message = "No data found for user " + username;
                return View(viewModel);
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Error");
            }
        }

        private int GetViewComplaintCount()
        {
            // Logic to retrieve the count of new, unchecked notifications from your data source
            // For example:
            string Username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
            int userid = user.account_id;
            var users = db.AddStudentDatas.Where(u => u.parent_id == userid).Select(x => x.Name).ToList();




            int newwholeCount = db.complaints.Where(c => c.Checked == false && c.reaciver_id == userid).Count();





            return newwholeCount;
        }
    }
}