using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject.Areas.Parent.Controllers
{
    public class ViewFeeNoticeController : Controller
    {
        // GET: Parent/ViewFeeNotice

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
                var users = db.AddStudentDatas.Where(x => x.parent_id == userId).Select(x => x.RollNO).ToList();

                foreach (var roll in users)
                {
                    var unsen = db.FeeNotices.Where(x => !x.Checked && x.RollNo == roll);
                    foreach (var Notice in unsen)
                    {
                        Notice.Checked = true;
                    }
                }
                db.SaveChanges();

                List<string> rollNo = db.AddStudentDatas.Where(x => x.parent_id == userId).Select(x => x.RollNO.ToString()).ToList();

                if (rollNo == null)
                {
                    // handle situation when user does not have a child associated with their account
                    return View("NoChildFound");
                }
                else
                {
                    var notice = db.FeeNotices.Where(x => rollNo.Contains(x.RollNo.ToString())).OrderBy(x => x.ndate).Distinct().ToList();

                    var viewModel = new FeeNoticeVM
                    {
                        FeeNotices = notice,
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


        public ActionResult Response(string details, int financeid)
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
                var users = db.CreateAccounts.SingleOrDefault(u => u.account_id == financeid);
                string Username = users.Username;
                // Store the section and Username parameters in session variables
                Session["Details"] = details;
                Session["ReplyUsername"] = Username;
                return View();
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

        [HttpPost]
        public ActionResult Response(NoticeReply obj, string details, string Username)
        {
            try
            {
                /* Session["Details"] = details;
                 Session["ReplyUsername"] = Username;*/
                string username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                var users = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userId = user.account_id;
                int finaceid = users.account_id;

                /* int rollNoInt = int.Parse(details);*/
                string roll = db.FeeNotices.Select(x => x.RollNo.ToString()).FirstOrDefault();

                //here is Sender Parameter Username but you use to hidden in view then this name save in data base
                /* string Sender = roll;*/
                string originalcomplaint = details;
                // Create a new whole_response object and save it to the database
                NoticeReply sd = new NoticeReply()
                {
                    sender_id = userId,
                    receiver_id = finaceid,
                    ndate = DateTime.Now,
                    Details = obj.Details,
                    Response = obj.Response,
                };
                db.NoticeReplies.Add(sd);
                db.SaveChanges();
                sd.receiver_id = 0;
                sd.sender_id = 0;
                sd.Details = "";
                ModelState.Clear();
                // Return the view with the new whole_response object as the model
                return View(sd);
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






        public ActionResult DateComplaintDetails()
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

                /* int newViewComplainCount = GetViewComplaintCount();*//*
                 ViewBag.NewViewComplaintCount = newViewComplainCount;*/

                List<string> Complaint = db.NoticeReplies
                    .Where(x => x.receiver_id == userId)
                    .Select(x => x.ndate.ToString()).Distinct()
                    .ToList();

                if (userId == 0)
                {
                    // handle situation when user does not have a child associated with their account
                    return View("NoChildFound");
                }
                else
                {
                    var complaints = db.NoticeReplies.Where(c => Complaint.Contains(c.ndate.ToString())).OrderBy(c => c.ndate).Distinct().ToList();

                    var viewModel = new ComplaintViewModel
                    {
                        NoticeReplies = complaints,
                        /*  specific_Complaints = specifics,*/
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
                // Return an appropriate error view or redirect to an error page
                return View("Error");
            }
        }




        public ActionResult ViewResponse(DateTime? date)
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



                var unseenComplaintsCount = db.NoticeReplies.Where(c => !c.Checked && c.receiver_id == userId).Count();

                if (unseenComplaintsCount > 0)
                {
                    var unseenComplaints = db.NoticeReplies.Where(c => !c.Checked && c.receiver_id == userId).ToList();
                    foreach (var complaint in unseenComplaints)
                    {
                        complaint.Checked = true;
                    }
                    db.SaveChanges();
                }



                if (date != null)
                {
                    var parsedDate = date.Value;

                    List<string> ReciverName = db.NoticeReplies
           .Where(x => x.sender_id == userId && DbFunctions.TruncateTime(x.ndate) == parsedDate.Date)
           .Select(x => x.ndate.ToString())
           .ToList();

                    List<string> Reciver = db.NoticeReplies
         .Where(x => x.receiver_id == userId && DbFunctions.TruncateTime(x.ndate) == parsedDate.Date)
         .Select(x => x.ndate.ToString())
         .ToList();
                    if (ReciverName != null && ReciverName.Any())
                    {
                        using (var db = new OSDFinalEntities())
                        {
                            var comp = db.NoticeReplies.Where(x => ReciverName.Contains(x.ndate.ToString())).OrderByDescending(x => x.ndate).ToList();
                            var comps = db.NoticeReplies.Where(x => Reciver.Contains(x.ndate.ToString())).OrderByDescending(x => x.ndate).ToList();

                            var viewModel = new ResponseViewModel
                            {
                                NoticeReplies = comp,
                                NoticeReplie = comps
                            };

                            return View(viewModel);
                        }
                    }
                }

                // handle empty or invalid date parameter
                // For example, you can redirect to an error page or display a message
                return View();

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