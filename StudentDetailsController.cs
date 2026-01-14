using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject.Areas.Tutor.Controllers
{
    public class StudentDetailsController : Controller
    {
        // GET: Tutor/StudentDetails
        OSDFinalEntities db =new OSDFinalEntities();
        [HttpGet]
        public ActionResult index()
        {
            try {
                string Username = Session["User-Name"] as string;

                if (Username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }


                int newViewComplainCount = GetViewComplaintCount();
            ViewBag.NewViewComplaintCount = newViewComplainCount;



            int newRequestCount = GetRequestCount();
            ViewBag.NewRequestCount = newRequestCount;


            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
            int userId = user.account_id;
                var stds = db.Assign_Tutor_Stds
         .Where(x => x.Tutor_ID == userId /*&&x.Request==true*/ && x.Auto_diary == true)
         .GroupBy(x => new { x.StartTime, x.ENDTime })
         .Select(g => g.FirstOrDefault())
         .ToList();
                var vm = new HireTutorVM
                {
                    assign_Tutor_Stds = stds,
                };
            return View(vm);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Index");
            }
        }


        public ActionResult ViewDiaryTutor(string subject, string section,string stdname)
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
            List<string> std = db.Assign_Tutor_Stds.Where(x => x.Tutor_ID == userId &&x.Auto_diary==true).Select(x => x.Std_Name).ToList();

            var sectionDiaries = db.SectionDiaries.Where(x => x.section == section).Where(x => x.subject == subject).OrderByDescending(x => x.Date).ToList();

            var specificDiaries = db.SpecificDiaries.Where(x => x.Std_Name == stdname).Where(x => x.subject==subject).OrderByDescending(x => x.Date).ToList();
           /* var specificDiaries = db.SpecificDiaries.Where(x => x.subject == subject).OrderByDescending(x => x.DateTime).ToList();*/

            var viewModel = new DiaryViewModel
            {
             SectionDiaries = sectionDiaries,
        SpecificDiaries = specificDiaries,
            };

            return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Index");
            }
        }

        public ActionResult SendComplaint(string section ,string stdname, string subject)
        {
            try {
                /*var section = id;*/
                string Username = Session["User-Name"] as string;

                if (Username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
            int userId = user.account_id;
                var student = db.AddStudentDatas.FirstOrDefault(x => x.Name == stdname);
                var std = student.RollNO;
           /* string ParentName = db.CreateAccounts
                            .Where(u => u.account_id == parentid)
                            .Select(u => u.Username)
                            .FirstOrDefault();*/
            Session["section"] = section;
            Session["StudentName"] = stdname;
            Session["subject"] = subject;
            
            var sections = db.Assign_Tutor_Stds.Where(x => x.Tutor_ID == userId)
                                                   .Select(x => x.parent_id).Distinct()
                                                   .ToList();

            ViewBag.tutor = sections;
            return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Index");
            }
        }
        [HttpPost]
        public ActionResult SendComplaint(complaint obj, string section,string stdname,string subject)
        {
            try { 
            string username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;

                var std = db.AddStudentDatas.FirstOrDefault(x => x.Name == stdname);

                var rollno = std.RollNO;

            /*var parent = db.CreateAccounts.SingleOrDefault(u => u.Username == ParentName);
            int parentid = parent.account_id;*/
           

                /* string reciver =Session["section"].ToString();*/
               specific_complaint student = new specific_complaint();

                /* student.RollNo = obj.RollNo;*/
                student.complaint_date = DateTime.Now;
                student.section = section;
                student.teacher_id = userId;
                student.RollNo= rollno;
                student.Checked = false;
                student.Std_Name= stdname;  
                student.subject = subject;
                student.detail = obj.detail;


                /*  student.section = section;
                  student.subject = subjects;*/


                db.specific_complaint.Add(student);
                ViewBag.Message = string.Format(" Data Inserted Successfully");
                db.SaveChanges();
                ModelState.Clear();

            }
            catch
            {


                ViewBag.Message = string.Format("  Successfully");
            }
            //return View();
            return RedirectToAction("Index");
        }

        public ActionResult DateComplaintDetails()
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

            int newViewComplainCount = GetViewComplaintCount();
            ViewBag.NewViewComplaintCount = newViewComplainCount;





            List<string> Complaint = db.Whole_response

                .Where(x => x.receiver_id == userId)
                .Select(x => x.date.ToString()).Distinct()
                .ToList();






            if (userId ==0)
            {
                // handle situation when user does not have a child associated with their account
                return View("NoChildFound");
            }
            else
            {

                var complaints = db.Whole_response.Where(c => Complaint.Contains(c.date.ToString())).OrderBy(c => c.date).Distinct().ToList();




                var viewModel = new ComplaintViewModel
                {
                    whole_Responses = complaints,
                    /*  specific_Complaints = specifics,*/

                };
                /* ViewBag.NewNotificationCount = GetNewNotificationCount();*/
                return View(viewModel);
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Index");
            }


        }

        public ActionResult ViewResponse(DateTime? date)
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



            var unseenComplaintsCount = db.Whole_response.Where(c => !c.Checked && c.receiver_id == userId).Count();

            if (unseenComplaintsCount > 0)
            {
                var unseenComplaints = db.Whole_response.Where(c => !c.Checked && c.receiver_id == userId).ToList();
                foreach (var complaint in unseenComplaints)
                {
                    complaint.Checked = true;
                }
                db.SaveChanges();
            }



            if (date != null)
            {
                var parsedDate = date.Value;

                List<string> ReciverName = db.Whole_response
       .Where(x => x.receiver_id == userId && DbFunctions.TruncateTime(x.date) == parsedDate.Date)
       .Select(x => x.date.ToString())
       .ToList();

                List<string> Reciver = db.Whole_response
     .Where(x => x.sender_id == userId && DbFunctions.TruncateTime(x.date) == parsedDate.Date)
     .Select(x => x.date.ToString())
     .ToList();
                if (ReciverName != null && ReciverName.Any())
                {
                    using (var db = new OSDFinalEntities())
                    {
                        var comp = db.Whole_response.Where(x => ReciverName.Contains(x.date.ToString())).OrderByDescending(x => x.date).ToList();
                        var comps = db.Whole_response.Where(x => Reciver.Contains(x.date.ToString())).OrderByDescending(x => x.date).ToList();

                        var viewModel = new ResponseViewModel
                        {
                            whole_Responses = comp,
                            whole_Response = comps
                        };

                        return View(viewModel);
                    }
                }
            }

            // handle empty or invalid date parameter
            // For example, you can redirect to an error page or display a message
            return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return RedirectToAction("Index");
            }
        }

        public ActionResult Reply(string details, int teacherid)
        {
            try {
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
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Index");
            }

        }
        [HttpPost]
        public ActionResult Reply(Whole_response obj, string details, string teacherName)
        {
            try { 
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
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View("Index");
            }
        }

        private int GetViewComplaintCount()
        {
            // Logic to retrieve the count of new, unchecked notifications from your data source
            // For example:
            string Username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
            int userid = user.account_id;





            int newwholeCount = db.complaints.Where(c => c.Checked == false && c.reaciver_id == userid).Count();





            return newwholeCount;
        }



        private int GetRequestCount()
        {
            // Logic to retrieve the count of new, unchecked notifications from your data source
            // For example:
            string Username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
            int userid = user.account_id;





            int newwholeCount = db.Assign_Tutor_Stds.Where(c => c.Request == false && c.Tutor_ID == userid).Count();





            return newwholeCount;
        }


        public ActionResult FeeRequest()
        {
            string username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;

            var request = db.TutorFees
                .Where(c => c.Request == false && c.Receiver_id == userId)
                .GroupBy(c => c.RollNo) // Group by RollNo to get unique students
                .Select(g => g.FirstOrDefault()) // Select the first entry of each group
                .ToList();

            var vm = new TutorFeeVM
            {
                TutorFees = request,
            };

            return View(vm);
        }
        [HttpGet]
        public ActionResult Accepted(TutorFeeVM obj, bool requestid, int rollno)
        {
            string username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;


            var request = db.TutorFees.Where(x => x.Request == requestid && x.RollNo == rollno &&x.Receiver_id==userId).ToList();

            if (request != null)
            {
                TutorFee d = new TutorFee();
                // Update the request status to true (accepted)

                foreach (var assignTutorStd in request)
                {
                    assignTutorStd.Request = true;
                }

                db.SaveChanges();
            
        
                var assignTutorStdList = db.Assign_Tutor_Stds.Where(x => x.RollNo == obj.RollNo).ToList();

                if (assignTutorStdList != null)
                {
                    foreach (var assignTutorStd in assignTutorStdList)
                    {
                        assignTutorStd.Status = "Paid";
                    }

                    db.SaveChanges();
                }

                // Save the changes to the database
                db.SaveChanges();
                TempData["Message"] = "Request Accepted";
                // Perform any additional actions or redirect to a different page
                // For example:
                return RedirectToAction("Index");
            }
            else
            {
                // Request not found, handle the error or return an appropriate response
                // For example:
                return RedirectToAction("Error", "Index");
            }
        }
        /*  public ActionResult ViewRating()
          {
              string username = Session["User-Name"].ToString();
              var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
              int userId = user.account_id;

              var ratings = db.TutorRatings.Where(x => x.TutorId == userId).ToList();

              if (ratings.Count > 0)
              {
                  var tutorRatings = ratings.GroupBy(r => r.TutorId)
      .Select(g => new TutorRating
      {
          TutorId = g.Key,
          Rating = CalculateAverageRating(g.Select(r =>r.Rating))
      })
      .ToList();




                  return View(tutorRatings);
              }

              return View(new List<TutorRating>());
          }

          private double CalculateAverageRating(IEnumerable<double> ratings)
          {
              double sum = ratings.Sum();
              int count = ratings.Count();
              double average = sum / count;

              // Calculate the integer part of the average rating
              int integerPart = (int)average;

              // Calculate the decimal part of the average rating
              double decimalPart = average - integerPart;

              // Round the decimal part to the nearest half value
              double roundedDecimalPart = Math.Round(decimalPart * 2) / 2;

              // Calculate the final average rating
              double finalAverage = integerPart + roundedDecimalPart;

              return finalAverage;
          }*/


        public ActionResult ViewRating()
        {
            string username = Session["User-Name"].ToString();
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;

            var ratings = db.TutorRatings.Where(x => x.TutorId == userId).ToList();

            if (ratings.Count > 0)
            {
                var tutorRatings = ratings.GroupBy(r => r.TutorId)
                                         .Select(g => new TutorRating
                                         {
                                             TutorId = g.Key,
                                             Rating = (int)Math.Round(g.Average(r => r.Rating))
                                         })
                                                 .ToList();
                return View(tutorRatings);
            }

            return View(new List<TutorRating>());
        }



    }
}