using FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject.Areas.Teacher.Controllers
{
    public class WholeComplaintController : Controller
    {
        // GET: Teacher/WholeComplaint
        OSDFinalEntities db = new OSDFinalEntities();
        public ActionResult Index()
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
                var std = db.AssignSubjectTeachers.Where(x => x.Teacher_id == userId).ToList();

                ViewBag.classes = std;

                return View(std);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the Index action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Error");
            }
        }

        public ActionResult SendComplaint(string id)
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

                var sections = db.AssignSubjectTeachers.Where(x => x.Teacher_id == userId)
                                                       .Select(x => x.Section)
                                                       .ToList();

                ViewBag.tutor = sections;
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the SendComplaint action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult SendComplaint(complaint obj)
        {

            string username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;
            try
            {
                /* SpecificDiary student = db.SpecificDiaries.Where(x => x.RollNo == sub[0].ToString()).FirstOrDefault();
 */
                /* string reciver = obj.section;*/
                complaint student = new complaint();
                string Reciver = Session["section"].ToString();
                /* student.RollNo = obj.RollNo;*/
                student.complaint_date = obj.complaint_date;
                student.section = Reciver;
                student.sender_id = userId;
                student.detail = obj.detail;
                student.parent_id = obj.parent_id;
               

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

        public List<SelectListItem> getTutotors()
        {
            try
            {
                var tutors = db.AssignSubjectTeachers.Select(t => new { t.Section, t.tid }).Distinct().ToList();
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (var tutor in tutors)
                {
                    items.Add(new SelectListItem()
                    {
                        Text = tutor.Section,
                        Value = tutor.tid.ToString()
                    });
                }
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the getTutotors method: " + ex.Message);
                // Handle the error in an appropriate manner, such as logging the error or throwing an exception
                return new List<SelectListItem>();
            }
        }


        /*
                [HttpGet]
                public JsonResult getSUbject(string username)
                {
                    var user = db.AssignSubjectTeachers.Where(x => x.Section == username).FirstOrDefault();
                    if (user == null)
                    {
                        return Json(new List<string>() { "Bal", "Contruction" }, behavior: JsonRequestBehavior.AllowGet);

                    }
                    List<String> subjs = db.SubjectTeachers.Where(s => s.tid == user.tid).Select(x => x.subject).Distinct().ToList();
                    if (user == null)
                    {
                        return Json(new List<string>() { "No", "Subjects", "enrooled", "by", "Contruction" }, behavior: JsonRequestBehavior.AllowGet);

                    }
                    return Json(subjs, behavior: JsonRequestBehavior.AllowGet);
                }*/

    }
}