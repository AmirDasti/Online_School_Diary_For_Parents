using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject.Areas.Teacher.Controllers
{
    public class DiaryHistoryTeacherController : Controller
    {
        // GET: Teacher/DiaryHistoryTeacher
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
                List<string> receiver = db.SectionDiaries
                    .Where(x => x.teacher_id == userId)
                    .Select(x => x.section)
                    .ToList();
                var diaryHistory = db.DiaryHistories
                    .Where(x => receiver.Contains(x.section))
                    .OrderByDescending(x => x.Date)
                    .ToList();
                var specificDiaries = db.SpDiaryHistories
                    .Where(x => receiver.Contains(x.section))
                    .OrderByDescending(x => x.Date)
                    .ToList();

                var viewModel = new DiaryHistoryVM
                {
                    diaryHistories = diaryHistory,
                    SpDiaryHistories = specificDiaries,
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the Index action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Error");
            }
        }
    }
}