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
    public class ViewDiaryHistoryController : Controller
    {
        // GET: Tutor/ViewDiaryHistory
        OSDFinalEntities db = new OSDFinalEntities();
        public ActionResult Index()
        {
            string username = Session["User-Name"] as string;

            if (username == null)
            {
                Session.Abandon();
                return RedirectToAction("Login", "Home", new { area = "" });

            }
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;
            List<string> subjects = db.Assign_Tutor_Stds.Where(x => x.Tutor_ID == userId).Select(x => x.subject).ToList();
           /* List<string> sections = db.AddStudentDatas.Where(x => x.PUsername == username).Select(x => x.Section).ToList();*/
            List<string> rollNo = db.Assign_Tutor_Stds.Where(x => x.Tutor_ID == userId).Select(x => x.Std_Name.ToString()).ToList();
            if (rollNo == null)
            {
                // handle situation when user does not have a child associated with their account
                return View("NoChildFound");
            }
            else
            {
                var sectionDiaries = db.DiaryHistories.Where(x => subjects.Contains(x.subject)).OrderByDescending(x => x.Date).Distinct().ToList();
                var specificDiaries = db.SpDiaryHistories.Where(x => rollNo.Contains(x.Std_Name.ToString())).OrderByDescending(x => x.Date).Distinct().ToList();
                /*var specificDiaries = db.SpecificDiaries.Where(x => x.RollNo.ToString() == rollNo).OrderByDescending(x => x.DateTime).ToList();*/



                var viewModel = new DiaryHistoryVM
                {
                    diaryHistories = sectionDiaries,
                    SpDiaryHistories = specificDiaries,
                };

                return View(viewModel);
            }
        }
    }
}