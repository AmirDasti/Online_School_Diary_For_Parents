using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject.Areas.Tutor.Controllers
{
    public class AddComplaintController : Controller
    {
        // GET: Tutor/AddComplaint
        OSDFinalEntities db=new OSDFinalEntities();
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
                ViewBag.tutor = db.Assign_Tutor_Stds
                    .Where(x => x.Tutor_ID == userId)
                    .Join(db.CreateAccounts, ats => ats.parent_id, acc => acc.account_id, (ats, acc) => new { acc.Username })
                    .Select(x => x.Username)
                    .Distinct()
                    .ToList();

                ViewBag.section = db.Assign_Tutor_Stds
                    .Where(x => x.Tutor_ID == userId)
                    .Select(x => x.@class)
                    .Distinct()
                    .ToList();

                return View();
            }
            catch (Exception ex)
            {
                // Handle the exception here, you can log the exception or perform any necessary actions
                Console.WriteLine("An error occurred: " + ex.Message);
                // You can also redirect to an error page or display a user-friendly error message
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult Index(complaint obj, string Username)
        {
            try
            {
                string username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;

                obj.complaint_date = DateTime.Now;
                obj.sender_id = userId;

                obj.parent_id = obj.parent_id;
                obj.section = obj.section;
                obj.detail = obj.detail;

                db.complaints.Add(obj);
                db.SaveChanges();

                return View(obj);
            }
            catch (Exception ex)
            {
                // Handle the exception here, you can log the exception or perform any necessary actions
                Console.WriteLine("An error occurred: " + ex.Message);
                // You can also redirect to an error page or display a user-friendly error message
                return RedirectToAction("Error", "Index");
            }
        }


        public ActionResult SendComplaint(string section)
        {
            try
            {
                string Username = Session["User-Name"] as string;

                if (Username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userId = user.account_id;

                var sections = db.Assign_Tutor_Stds
                    .Where(x => x.Tutor_ID == userId)
                    .Select(x => x.parent_id)
                    .ToList();

                ViewBag.tutor = sections;

                return View();
            }
            catch (Exception ex)
            {
                // Handle the exception here, you can log the exception or perform any necessary actions
                Console.WriteLine("An error occurred: " + ex.Message);
                // You can also redirect to an error page or display a user-friendly error message
                return RedirectToAction(nameof(SendComplaint));
            }
        }

        [HttpPost]
        public ActionResult SendComplaint(complaint obj, string id)
        {
            try { 
            string username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;
           


                /* string reciver =Session["section"].ToString();*/
                complaint student = new complaint();

                /* student.RollNo = obj.RollNo;*/
                student.complaint_date = obj.complaint_date;
                student.section = id;
                student.sender_id = userId;
                student.detail = obj.detail;


                /*  student.section = section;
                  student.subject = subjects;*/


                db.complaints.Add(student);
                ViewBag.Message = string.Format(" Data Inserted Successfully");
                db.SaveChanges();
                ModelState.Clear();

            }
            catch
            {


                ViewBag.Message = string.Format("  Successfully");
            }
            //return View();
            return RedirectToAction(nameof(SendComplaint));
        }
    }
}
