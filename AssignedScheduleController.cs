using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Areas.Parent.Controllers
{
    public class AssignedScheduleController : Controller
    {
        // GET: Parent/AssignedSchedule
        OSDFinalEntities db = new OSDFinalEntities();
        public ActionResult Index()
        {
            string username = Session["User-Name"] as string;
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;


            var getschedule = db.Assign_Tutor_Stds.Where(x => x.parent_id == userId).ToList();

            var vm = new ScheduleVM
            {

                Assign_Tutor_Stds = getschedule,
            };


            return View(vm);
        }
    }
}