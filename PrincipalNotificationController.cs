using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Areas.Teacher.Controllers
{
    public class PrincipalNotificationController : Controller
    {
        // GET: Teacher/PrincipalNotification
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
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;

                var users = db.CreateAccounts.Where(x => x.account_id == userId).Select(x => x.UserType).ToList();

                List<string> usertype = db.CreateAccounts.Where(x => x.account_id == userId).Select(x => x.UserType.ToString()).ToList();
                List<string> res = db.SpecificNotifications.Where(x => x.Receiver_id == userId).Select(x => x.Notice).ToList();

                if (usertype == null)
                {
                    // handle situation when user does not have a child associated with their account
                    return View("NoChildFound");
                }
                else
                {
                    var notice = db.Notifications.Where(x => usertype.Contains(x.Usertype.ToString())).OrderBy(x => x.ndate).Distinct().ToList();

                    var spNotice = db.SpecificNotifications.Where(x => res.Contains(x.Notice) && x.Receiver_id == userId).ToList();

                    var viewModel = new SpecificNotificarionVM
                    {
                        Notifications = notice,
                        SpecificNotifications = spNotice,
                    };

                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the Index action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View();
            }

        }
    }
}
