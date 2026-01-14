using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Areas.Tutor.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Tutor/Dashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Home", new
            {
                area = ""
            });
        }
    }
}