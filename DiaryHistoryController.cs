using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject.Areas.Parent.Controllers
{
    public class DiaryHistoryController : Controller
    {
        // GET: Parent/DiaryHistory
        OSDFinalEntities db = new OSDFinalEntities();
        public ActionResult Index()
        {
            try
            {
                /* string username = Session["User-Name"].ToString();
                 List<string> receiver = db.SectionDiaries
                     .Where(x => x.Username == username)
                     .Select(x => x.section)
                     .ToList();
                 var sectionDiaries = db.SectionDiaries
                     .Where(x => receiver.Contains(x.section))
                     .OrderByDescending(x => x.Date)
                     .ToList();
                 var specificDiaries = db.SpecificDiaries
                     .Where(x => receiver.Contains(x.section))
                     .OrderByDescending(x => x.DateTime)
                     .ToList();

                 var viewModel = new DiaryViewModel
                 {
                     SectionDiaries = sectionDiaries,
                     SpecificDiaries = specificDiaries,
                 };

                 return View(viewModel);*/
                string username = Session["User-Name"] as string;

                if (username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;
                List<string> students = db.Assign_Tutor_Stds.Where(x => x.parent_id == userId).Select(x => x.Std_Name).ToList();
                List<string> sections = db.AddStudentDatas.Where(x => x.parent_id == userId).Select(x => x.Section).ToList();
                /*List<string> rollNo = db.AddStudentDatas.Where(x => x.PUsername == username).Select(x => x.RollNo.ToString()).ToList();*/
                List<string> rollNo = db.AddStudentDatas.Where(x => x.parent_id == userId).Select(x => x.Name.ToString()).ToList();
                if (rollNo == null)
                {
                    // handle situation when user does not have a child associated with their account
                    return View("NoChildFound");
                }
                else
                {
                    var sectionDiaries = db.DiaryHistories.Where(x => sections.Contains(x.section)).OrderByDescending(x => x.Date).Distinct().ToList();
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