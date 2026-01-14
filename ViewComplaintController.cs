using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject.Areas.Tutor.Controllers
{
    public class ViewComplaintController : Controller
    {
        // GET: Tutor/ViewComplaint
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
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username ==username);
            int userId = user.account_id;

            List<string> ReciverName = db.complaints.Where(x => x.reaciver_id == userId)
                .Select(x => x.detail)
    .ToList();

            if (ReciverName != null && ReciverName.Any())
            {
                var comp = db.complaints.Where(x => ReciverName.Contains(x.detail)).OrderByDescending(x => x.complaint_date).ToList();
                var com = db.specific_complaint.Where(x => ReciverName.Contains(x.detail)).OrderByDescending(x => x.complaint_date).ToList();
                var viewModel = new ComplaintViewModel
                {
                    complaints = comp,
                    specific_Complaints = com,
                };

                return View(viewModel);
            }
            /* Task.Delay(TimeSpan.FromMinutes(2)).ContinueWith((task) => ClearHistory());*/
            return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Index");
            }
        }

        public ActionResult Send(string details, int parentid)
        {
            try {
                string username = Session["User-Name"] as string;

                if (username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }
                var user = db.CreateAccounts.SingleOrDefault(x => x.account_id == parentid);
            int userid=user.account_id;
            string Username = user.Username;
            // Store the section and Username parameters in session variables
            Session["Details"] = details;
            Session["ReplyUsername"] = Username;
            return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Index");
            }

        }
        [HttpPost]
        public ActionResult Send(Whole_response obj, string details, string Username)
        {
            try { 
            /* Session["Details"] = details;
             Session["ReplyUsername"] = Username;*/
            string username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);

            var users = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
            int userId = user.account_id;

            int parentid = users.account_id;
            //here is Sender Parameter Username but you use to hidden in view then this name save in data base
            /* int Sender = parentid;*/
            string originalcomplaint = details;
            // Create a new whole_response object and save it to the database
            Whole_response sd = new Whole_response()
            {
                sender_id = userId,
                receiver_id = parentid,
                date = DateTime.Now,
                respdetail = obj.respdetail,
                OriginalComplaint = originalcomplaint,
            };
            db.Whole_response.Add(sd);
            db.SaveChanges();
            sd.sender_id = 0;
            sd.receiver_id= 0;
            sd.respdetail = "";
            ModelState.Clear();
            // Return the view with the new whole_response object as the model
            return View(sd);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Index");
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

            List<string> ReciverName = db.Whole_response
    .Where(x => x.sender_id == userId)
    .Select(x => new { x.date, x.respdetail })
    .OrderByDescending(x => x.date)
    .Select(x => x.respdetail)
    .ToList();


            if (ReciverName != null && ReciverName.Any())
            {
                var comp = db.Whole_response.Where(x => ReciverName.Contains(x.respdetail)).OrderByDescending(x => x.date).ToList();

                var viewModel = new ResponseViewModel
                {
                    whole_Responses = comp,

                    /* specific_Complaints = specific,*/
                };



                return View(viewModel);
            }
            return View();


            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Index");
            }
        }
    }
}