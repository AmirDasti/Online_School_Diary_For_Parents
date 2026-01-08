using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace FinalProject.Areas.Parent.Controllers
{
    public class DateComplaintController : Controller
    {
        // GET: Parent/DateComplaint

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

            /* var names = db.AddStudentDatas.Where(x => x.parent_id == userId).Select(x => x.Name).ToList();

             foreach( var name in names)
             {
                 var allcom = db.specific_complaint.Where(x => !x.Checked && x.Std_Name == name);

                 foreach( var updated in allcom)
                 {
                     updated.Checked = true;
                 }
                 db.SaveChanges();

             }
             var unseenComplaints = db.complaints.Where(c => !c.Checked && c.reaciver_id==userId);

             foreach( var unseen in unseenComplaints)
             {

                 unseen.Checked = true;
             }
             db.SaveChanges();*/

            var std = db.AddStudentDatas.Where(x => x.parent_id == userId).FirstOrDefault();


            string stdname = std.Name;
            string section = std.Section;


            int newViewComplainCount = GetViewComplaintCount();
            ViewBag.NewViewComplaintCount = newViewComplainCount;


            // Update the Checked field for the unseen complaints in both tables


            // Save the changes to the database
            db.SaveChanges();



            List<string> specific = db.specific_complaint
         .Where(x => x.Std_Name == stdname)

         .Select(x => x.complaint_date.ToString()).Distinct()
         .ToList();

            List<string> Complaint = db.complaints

                .Where(x => x.section == section)
                .Select(x => x.complaint_date.ToString()).Distinct()
                .ToList();


            if (stdname == null)
            {
                // handle situation when user does not have a child associated with their account
                return View("NoChildFound");
            }
            else
            {

                var complaints = db.complaints.Where(c => Complaint.Contains(c.complaint_date.ToString())).OrderBy(c => c.complaint_date).Distinct().ToList();
                var specifics = db.specific_complaint.Where(c => specific.Contains(c.complaint_date.ToString())).OrderBy(c => c.complaint_date).Distinct().ToList();



                var viewModel = new ComplaintViewModel
                {
                    complaints = complaints,
                   /* specific_Complaints = specifics,*/

                };
                /* ViewBag.NewNotificationCount = GetNewNotificationCount();*/
                return View(viewModel);
            }

        }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                return View();
            }
        }



        public ActionResult ViewComplaintDate(string date)
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
                var students = db.AddStudentDatas.Where(x => x.parent_id == userId).ToList();

                var names = db.AddStudentDatas.Where(x => x.parent_id == userId).Select(x => x.Name).ToList();

                foreach (var name in names)
                {
                    var allcom = db.specific_complaint.Where(x => !x.Checked && x.Std_Name == name);

                    foreach (var updated in allcom)
                    {
                        updated.Checked = true;
                    }
                    db.SaveChanges();
                }

                var unseenComplaints = db.complaints.Where(c => !c.Checked && c.reaciver_id == userId);

                foreach (var unseen in unseenComplaints)
                {
                    unseen.Checked = true;
                }
                db.SaveChanges();

                if (string.IsNullOrEmpty(date))
                {
                    // handle empty date parameter
                }
                else
                {
                    try
                    {
                        foreach (var student in students)
                        {
                            string stdname = student.Name;
                            string section = student.Section;

                            var parsedDate = DateTime.ParseExact(date, "yyyy-dd-MM", CultureInfo.InvariantCulture);
                            if (DateTime.TryParseExact(date, "yyyy-dd-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                            {
                                List<string> ReciverName = db.complaints
                                    .Where(x => x.section == section || x.reaciver_id == userId)
                                    .Select(x => x.complaint_date.ToString())
                                    .ToList();

                                List<string> specific = db.specific_complaint
                                    .Where(x => x.Std_Name == stdname)
                                    .Select(x => x.complaint_date.ToString())
                                    .ToList();

                                if (ReciverName != null && ReciverName.Any())
                                {
                                    using (var db = new OSDFinalEntities())
                                    {
                                        var complaints = db.complaints
                                            .Where(c => ReciverName.Contains(c.complaint_date.ToString()) && DbFunctions.TruncateTime(c.complaint_date) == parsedDate.Date)
                                            .ToList();

                                        var specifics = db.specific_complaint
                                            .Where(c => specific.Contains(c.complaint_date.ToString()) && DbFunctions.TruncateTime(c.complaint_date) == parsedDate.Date)
                                            .ToList();

                                        var viewModel = new ComplaintViewModel
                                        {
                                            complaints = complaints,
                                            specific_Complaints = specifics
                                        };

                                        return View(viewModel);
                                    }
                                }
                            }
                        }
                    }
                    catch (FormatException)
                    {
                        // handle invalid date format
                    }
                }

                return RedirectToAction("ViewComplaintDate");
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Redirect to an error page or display an error message
                return View();
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
                // Redirect to an error page or display an error message
                return View();
            }
        }

        [HttpPost]
        public ActionResult Reply(Whole_response obj, string details, string teacherName)
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
               Whole_response   sd = new Whole_response()
                {
                    sender_id = userId,
                    date = DateTime.Now,
                    respdetail = obj.respdetail,
                    receiver_id = teacherid,
                    OriginalComplaint = originalcomplaint,
                };

                db.Whole_response.Add(sd);
                db.SaveChanges();

                sd.receiver_id = 0;
                sd.sender_id = 0;
                sd.respdetail = "";
                ModelState.Clear();

                // Return the view with the new whole_response object as the model
                return View();
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Redirect to an error page or display an error message
                return View();
            }
        }



        public ActionResult ViewResponse()
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

                if (string.IsNullOrEmpty(username))
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
                        whole_Responses = comp,
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
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Redirect to an error page or display an error message
                return View();
            }
        }




        private int GetViewComplaintCount()
        {
            try
            {
                string Username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userid = user.account_id;
                var users = db.AddStudentDatas.Where(u => u.parent_id == userid).Select(x => x.Name).ToList();
                int newSpecificCount = 0;
                int zero = 0;
                foreach (var name in users)
                {
                    int count = db.specific_complaint.Where(c => c.Checked == false && c.Std_Name == name).Count();
                    if (count > 0)
                    {
                        newSpecificCount += count;
                    }
                    else
                    {
                        zero = 0;
                        break;
                    }
                }

                int newwholeCount = db.complaints.Where(c => c.Checked == false && c.reaciver_id == userid).Count();

                int totalCount = newSpecificCount + newwholeCount;

                return totalCount;
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return a default count or an error indicator
                return -1;
            }
        }


    }
}