using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject.Areas.Parent.Controllers
{
    public class ViewComplaintController : Controller
    {
        // GET: Parent/ViewComplaint
        OSDFinalEntities db = new OSDFinalEntities();
        public ActionResult Index(DateTime date)
        {
            /* string username = Session["User-Name"].ToString();

             // Retrieve the receiver value from the database
             string receiver = db.assign_std_to_tutor.Where(x => x.parent_uname == username).Select(x => x.@class).FirstOrDefault();

             // Retrieve the complaints for the receiver and order them by date in descending order
             var complaint = db.complaints.Where(x => x.section == receiver).OrderByDescending(x => x.complaint_date).ToList();

             // Retrieve the specific complaints for the receiver
             var specific = db.specific_complaint.Where(x => x.sender_username == receiver).OrderByDescending(x => x.complaint_date).ToList();

             // Create a view model containing the section and specific diaries
             var viewModel = new ComplaintViewModel
             {
                 complaints = complaint,
                 specific_Complaints = specific,
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
            /*  List<string> rollNo = db.AddStudentDatas.Where(x => x.PUsername == username).Select(x => x.RollNo.ToString()).ToList();*/
            List<string> rollNo = db.AddStudentDatas.Where(x => x.parent_id == userId).Select(x => x.Name.ToString()).ToList();
            /*  List<string> res = db.whole_response.Where(x => x.sender == username).Select(x => x.respdetail).ToList();*/

            if (rollNo == null)
            {
                // handle situation when user does not have a child associated with their account
                return View("NoChildFound");
            }
            else
            {
                var complaints = db.complaints.Where(x => sections.Contains(x.section)).OrderBy(x => x.complaint_date).Distinct().ToList();
                var specific = db.specific_complaint.Where(x => rollNo.Contains(x.Std_Name.ToString())).OrderBy(x => x.complaint_date).Distinct().ToList();
               /* var Response=db.whole_response.Where(x=>res.Contains(x.respdetail)).ToList();*/
                /*var specificDiaries = db.SpecificDiaries.Where(x => x.RollNo.ToString() == rollNo).OrderByDescending(x => x.DateTime).ToList();*/



                var viewModel = new ComplaintViewModel
                {
                   complaints = complaints,
                    specific_Complaints = specific,
                   /* whole_Responses = Response,*/
                };
               /* ViewBag.NewNotificationCount = GetNewNotificationCount();*/
                return View(viewModel);
            }

        
    }
      /*  public int GetNewNotificationCount()
        {
            string username = Session["User-Name"].ToString();

            var complaints = db.complaints.Where(x => x.Recivername == username).ToList();

            int newNotificationCount = complaints.Count(x => !x.IsRead);

            return newNotificationCount;
        }*/

        public ActionResult Send(string details, int teacherid)
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
        [HttpPost]
        public ActionResult Send(Whole_response obj,string details,string teacherName)
        {
           /* Session["Details"] = details;
            Session["ReplyUsername"] = Username;*/
            string username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;


            var teacher = db.CreateAccounts.SingleOrDefault(u => u.Username == teacherName);
            int teacherid = teacher.account_id;
            //here is Sender Parameter Username but you use to hidden in view then this name save in data base
            /* int Sender = teacherid;*/
            string originalcomplaint = details;
            // Create a new whole_response object and save it to the database
            Whole_response sd = new Whole_response()
            {
                receiver_id = userId,
               /* teacher_id = Sender,*/
                date = DateTime.Now,
                respdetail = obj.respdetail,
                sender_id = teacherid, 
                OriginalComplaint = originalcomplaint,
            };
            db.Whole_response.Add(sd);
            db.SaveChanges();
            sd.receiver_id = 0;
            sd.sender_id = 0;
            sd.respdetail = "";
            ModelState.Clear();
            // Return the view with the new whole_response object as the model
            return View(sd);
        }

        public ActionResult ViewResponse()
        {
            string username = Session["User-Name"] as string;

            if (username == null)
            {
                Session.Abandon();
                return RedirectToAction("Login", "Home", new { area = "" });

            }
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;

            if (string.IsNullOrEmpty(userId.ToString()))
            {
                ViewBag.Message = "User name not found in session.";
                return View();
            }

            List<string> ReciverName = db.Whole_response.Where(x => x.receiver_id == userId).Select(x => x.respdetail).ToList();

            if (ReciverName.Any())
            {
                var comp = db.Whole_response.Where(x => ReciverName.Contains(x.respdetail)).OrderBy(x => x.date).ToList();

                var viewModel = new ResponseViewModel
                {
                    whole_Responses =comp,
                };

                return View(viewModel);
            }
            else
            {
                var viewModel = new ResponseViewModel
                {
                    whole_Responses = new List<Whole_response>(),
                };

                ViewBag.Message = "No data found for user " + username;
                return View(viewModel);
            }
        }

    }

    }