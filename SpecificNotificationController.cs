using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Areas.Principal.Controllers
{
    public class SpecificNotificationController : Controller
    {
        // GET: Principal/SpecificNotification
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


                /*  int newViewComplainCount = GetViewComplaintCount();
                  ViewBag.NewViewComplaintCount = newViewComplainCount;*/

                var users = db.Users.Select(x => x.UserTypes).ToList(); ;



            List<string> ReciverName = db.CreateAccounts
     .Where(x => x.UserType == "Teacher" || x.UserType == "Parent")
     .Select(x => x.Username.ToString())
     .ToList();

            if (ReciverName != null && ReciverName.Any())
            {
                var pending = db.CreateAccounts
                    .Where(x => ReciverName.Contains(x.Username.ToString()))
                    .ToList();

                var viewModel = new SpecificNotificarionVM();
                viewModel.Users = users;
                viewModel.CreateAccounts = pending;

                return View(viewModel);
            }


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
        public ActionResult Index(SpecificNotificarionVM obj, string[] selectedStudents)
        {
            try
            {
                string Username = Session["User-Name"].ToString();

                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userId = user.account_id;

                if (selectedStudents == null)
                {
                    // Handle the case when no students are selected
                    // For example, show a message indicating no data was inserted
                    ViewBag.Message = "No students were selected.";
                    return RedirectToAction("Index");
                }

                foreach (string username in selectedStudents)
                {
                    var newModel = new SpecificNotificarionVM();
                    if (!string.IsNullOrEmpty(username))
                    {
                        var parentname = db.CreateAccounts.SingleOrDefault(x => x.Username == username);
                        int parentid = parentname.account_id;

                        SpecificNotification d = new SpecificNotification();
                        d.Principal_id = userId;
                        d.ndate = DateTime.Now;
                        d.Receiver_id = parentid;
                        d.Notice = obj.Notice;

                        db.SpecificNotifications.Add(d);
                        db.SaveChanges();
                    }
                }

                var Pending = db.CreateAccounts.ToList();
                var users = db.Users.Select(x => x.UserTypes).ToList();

                var viewModel = new SpecificNotificarionVM();

                viewModel.Users = users;
                viewModel.CreateAccounts = Pending;

                return View(viewModel);
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

    }
}