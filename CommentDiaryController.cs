using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Collections.Specialized.BitVector32;

namespace FinalProject.Areas.Teacher.Controllers
{
    public class CommentDiaryController : Controller
    {
        // GET: Teacher/CommentDiary
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

                var currentDate = DateTime.Now;

                var replies = db.CommentDiaries.Where(x => x.Receiver_id == userId).ToList();


                var vm = new CommentDiaryVM
                {

                    CommentDiaries = replies,

                };

                return View(vm);
            }



            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error response or redirect to an error page
                return RedirectToAction("Index");
            }
           
        }



        public ActionResult Reply(string details, int teacherid)
        {
            try
            {
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
                // Return an error response or redirect to an error page
                return Content("An error occurred");
            }
        }

        [HttpPost]
        public ActionResult Reply(CommentDiaryVM obj, string details, string teacherName)
        {
            try
            {
                string username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;

                var teacher = db.CreateAccounts.SingleOrDefault(u => u.Username == teacherName);
                int teacherid = teacher.account_id;

                string originalcomplaint = details;

                // Create a new whole_response object and save it to the database
               CommentDiary sd = new CommentDiary()
                {
                    Sender_id = userId,
                    RollNo= teacherid,
                    subject=originalcomplaint,
                   /* date = DateTime.Now,*/
                    Comment = obj.Comment,
                    Receiver_id = teacherid,
                    OrginalDiary = originalcomplaint,
                };

                db.CommentDiaries.Add(sd);
                db.SaveChanges();

                sd.Receiver_id = 0;
                sd.Sender_id = 0;
                sd.Comment = "";
                ModelState.Clear();

                // Return the view with the new whole_response object as the model
                return View(sd);
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an error response or redirect to an error page
                return View();
            }
        }


    }
}


