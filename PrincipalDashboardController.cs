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

namespace FinalProject.Areas.Principal.Controllers
{
    public class PrincipalDashboardController : Controller
    {
        // GET: Principal/PrincipalDashboard

        OSDFinalEntities db = new OSDFinalEntities();
        public ActionResult Index()
        {
            string Username = Session["User-Name"] as string;

            if (Username == null)
            {
                Session.Abandon();
                return RedirectToAction("Login", "Home", new { area = "" });

            }
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
            int userId = user.account_id;
            var std = db.AddStudentDatas.Where(x => x.parent_id == userId).ToList();

            ViewBag.child = std;

            int newViewComplainCount = GetViewTeachersCount();
            ViewBag.NewViewTeachersCount = newViewComplainCount;

            int newViewFeeNoticeCount = GetViewAccountCount();
            ViewBag.NewAccountCount = newViewFeeNoticeCount;

            int newComplaintReplyCount = GetViewTutorsCount();
            ViewBag.NewTutorsCount = newComplaintReplyCount;

            return View();





            
        }

        public ActionResult TeachersComplaint(string type)
        {
            string username = Session["User-Name"] as string;

            if (username == null)
            {
                Session.Abandon();
                return RedirectToAction("Login", "Home", new { area = "" });

            }
            int newViewComplainCount = GetViewTeachersCount();
            ViewBag.NewViewTeachersCount = newViewComplainCount;
            Session["Type"] = type;
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;

            var userlist=db.CreateAccounts.Where(x=>x.UserType==type).Select(x=>x.account_id).ToList();


            var complaints = db.complaints.Where(c => userlist.Contains(c.sender_id)).Select(x=>x.complaint_date).ToList();

            var complaintss = db.complaints.Where(c => complaints.Contains(c.complaint_date)).ToList();

            var Specfic = db.specific_complaint.Where(c => userlist.Contains(c.teacher_id)).ToList();

            /*var Specifics = db.specific_complaint.Where(x => Specfic.Contains(x.complaint_date)).ToList();*/


            if (userId == 0)
            {
                // handle situation when user does not have a child associated with their account
                return View("No Found");
            }
            else
            {


                var viewModel = new PrincipalCompalintPenalVM
                {
                    complaints = complaintss,
                    specific_comps= Specfic,


                };
                /* ViewBag.NewNotificationCount = GetNewNotificationCount();*/
                return View(viewModel);



            }
        }

        public ActionResult ParentsComplaint(string type)
        {
            string username = Session["User-Name"] as string;

            if (username == null)
            {
                Session.Abandon();
                return RedirectToAction("Login", "Home", new { area = "" });

            }
            int newComplaintReplyCount = GetViewTutorsCount();
            ViewBag.NewTutorsCount = newComplaintReplyCount;
            Session["Type"] = type;
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;

            var userlist = db.CreateAccounts.Where(x => x.UserType == type).Select(x => x.account_id).ToList();


            

            /*var complaintss = db.complaints.Where(c => complaints.Contains(c.complaint_date)).ToList();*/

            var Specfic = db.specific_complaint.Where(c => userlist.Contains(c.teacher_id)).ToList();

            /*var Specifics = db.specific_complaint.Where(x => Specfic.Contains(x.complaint_date)).ToList();*/


            if (userId == 0)
            {
                // handle situation when user does not have a child associated with their account
                return View("No Found");
            }
            else
            {
                var complaints = db.complaints.Where(c => userlist.Contains(c.sender_id)).ToList();


                var viewModel = new PrincipalCompalintPenalVM
                {
                    complaints = complaints,
                    specific_comps = Specfic,


                };
                /* ViewBag.NewNotificationCount = GetNewNotificationCount();*/
                return View(viewModel);



            }
        }



        public ActionResult AccountsComplaint(string type)
        {
            string username = Session["User-Name"] as string;

            if (username == null)
            {
                Session.Abandon();
                return RedirectToAction("Login", "Home", new { area = "" });

            }
            int newViewFeeNoticeCount = GetViewAccountCount();
            ViewBag.NewAccountCount = newViewFeeNoticeCount;
            Session["Type"] = type;
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;

            var userlist = db.CreateAccounts.Where(x => x.UserType == type).Select(x => x.account_id).ToList();




            /*var complaintss = db.complaints.Where(c => complaints.Contains(c.complaint_date)).ToList();*/

            var Specfic = db.FeeNotices.Where(c => userlist.Contains(c.finance_id)).ToList();

            /*var Specifics = db.specific_complaint.Where(x => Specfic.Contains(x.complaint_date)).ToList();*/


            if (userId == 0)
            {
                // handle situation when user does not have a child associated with their account
                return View("No Found");
            }
            else
            {
               


                var viewModel = new PrincipalCompalintPenalVM
                {
                    FeeNotices=Specfic,


                };
                /* ViewBag.NewNotificationCount = GetNewNotificationCount();*/
                return View(viewModel);



            }
        }






        public ActionResult ViewComplaintDate(DateTime? date,string type)
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

                var unseenComplaintsCount = db.complaints.Where(c => !c.Checked && c.reaciver_id == userId).Count();

                if (unseenComplaintsCount > 0)
                {
                    var unseenComplaints = db.complaints.Where(c => !c.Checked && c.reaciver_id == userId).ToList();
                    foreach (var complaint in unseenComplaints)
                    {
                        complaint.Checked = true;
                    }
                    db.SaveChanges();
                }

                if (date != null)
                {
                    var parsedDate = date.Value;

                    List<string> ReciverName = db.complaints
           .Where(x =>  DbFunctions.TruncateTime(x.complaint_date) == parsedDate.Date )
           .Select(x => x.complaint_date.ToString())
           .ToList();

                    List<string> ReciverNames = db.specific_complaint
           .Where(x => DbFunctions.TruncateTime(x.complaint_date) == parsedDate.Date)
           .Select(x => x.complaint_date.ToString())
           .ToList();

                    var userlist = db.CreateAccounts.Where(x => x.UserType == type).Select(x => x.account_id).ToList();


                    var complaints = db.complaints.Where(c => userlist.Contains(c.sender_id)).Select(x => x.complaint_date).ToList();


                    

                    if (ReciverName != null || ReciverNames!=null)
                    {
                        using (var db = new OSDFinalEntities())
                        {
                            var complaintss = db.complaints.Where(c => complaints.Contains(c.complaint_date)).ToList();

                            var Spec = db.specific_complaint.Where(c => userlist.Contains(c.teacher_id)).ToList();

                            /* var specficcom = db.specific_complaint.Where(c => Spec.Contains(c.complaint_date)).ToList();*/
                            var viewModel = new PrincipalCompalintPenalVM
                            {
                                complaints = complaintss,
                                specific_comps=Spec,
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
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an appropriate error view or redirect to an error page
                return View("Error");
            }
        }


        public ActionResult TutorsComplaint(DateTime? date, string type)
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

                var unseenComplaintsCount = db.complaints.Where(c => !c.Checked && c.reaciver_id == userId).Count();

                if (unseenComplaintsCount > 0)
                {
                    var unseenComplaints = db.complaints.Where(c => !c.Checked && c.reaciver_id == userId).ToList();
                    foreach (var complaint in unseenComplaints)
                    {
                        complaint.Checked = true;
                    }
                    db.SaveChanges();
                }

                if (date != null)
                {
                    var parsedDate = date.Value;

                /*    List<string> ReciverName = db.complaints
           .Where(x => DbFunctions.TruncateTime(x.complaint_date) == parsedDate.Date)
           .Select(x => x.complaint_date.ToString())
           .ToList();*/

                    List<string> ReciverNames = db.specific_complaint
           .Where(x => DbFunctions.TruncateTime(x.complaint_date) == parsedDate.Date)
           .Select(x => x.complaint_date.ToString())
           .ToList();

                    var userlist = db.CreateAccounts.Where(x => x.UserType == type).Select(x => x.account_id).ToList();


                    




                    if ( ReciverNames != null)
                    {
                        using (var db = new OSDFinalEntities())
                        {
                           /* var complaints = db.complaints.Where(c => userlist.Contains(c.sender_id)).ToList();*/

                            var Spec = db.specific_complaint.Where(c => userlist.Contains(c.teacher_id)).ToList();

                            /* var specficcom = db.specific_complaint.Where(c => Spec.Contains(c.complaint_date)).ToList();*/
                            var viewModel = new PrincipalCompalintPenalVM
                            {
                                
                                specific_comps = Spec,
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
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an appropriate error view or redirect to an error page
                return View("Error");
            }
        }
        public ActionResult AccountsComplaintDate(DateTime? date, string type)
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

                var unseenComplaintsCount = db.FeeNotices.Where(c => !c.Checked && c.finance_id == userId).Count();

                if (unseenComplaintsCount > 0)
                {
                    var unseenComplaints = db.complaints.Where(c => !c.Checked && c.reaciver_id == userId).ToList();
                    foreach (var complaint in unseenComplaints)
                    {
                        complaint.Checked = true;
                    }
                    db.SaveChanges();
                }

                if (date != null)
                {
                    var parsedDate = date.Value;

                    /*    List<string> ReciverName = db.complaints
               .Where(x => DbFunctions.TruncateTime(x.complaint_date) == parsedDate.Date)
               .Select(x => x.complaint_date.ToString())
               .ToList();*/

                    List<string> ReciverNames = db.FeeNotices
           .Where(x => DbFunctions.TruncateTime(x.ndate) == parsedDate.Date)
           .Select(x => x.ndate.ToString())
           .ToList();

                    var userlist = db.CreateAccounts.Where(x => x.UserType == type).Select(x => x.account_id).ToList();







                    if (ReciverNames != null)
                    {
                        using (var db = new OSDFinalEntities())
                        {
                            /* var complaints = db.complaints.Where(c => userlist.Contains(c.sender_id)).ToList();*/

                            var Spec = db.FeeNotices.Where(c => userlist.Contains(c.finance_id)).ToList();

                            /* var specficcom = db.specific_complaint.Where(c => Spec.Contains(c.complaint_date)).ToList();*/
                            var viewModel = new PrincipalCompalintPenalVM
                            {

                                FeeNotices = Spec,
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
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return an appropriate error view or redirect to an error page
                return View("Error");
            }
        }






        public ActionResult ViewResponseTeacher(int rollno,int teacherid,string section)
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

                var id = db.AddStudentDatas.FirstOrDefault(x => x.RollNO == rollno);

                var parentid = id.parent_id;

                if (string.IsNullOrEmpty(username))
                {
                    ViewBag.Message = "User name not found in session.";
                    return View();
                }

                var details = db.Whole_response.Where(x => x.sender_id == teacherid && x.receiver_id==parentid).ToList();

                if (details.Any())
                {
                    

                    var viewModel = new PrincipalCompalintPenalVM
                    {
                        whole_Responses = details,
                    };

                    return View(viewModel);
                }
                else
                {
                    var viewModel = new PrincipalCompalintPenalVM
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
                // Return an appropriate error view or redirect to an error page
                return View("Error");
            }
        }

        public ActionResult ViewTutorsResponse(int rollno, int teacherid, string section)
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

                var id = db.AddStudentDatas.FirstOrDefault(x => x.RollNO == rollno);

                var parentid = id.parent_id;

                if (string.IsNullOrEmpty(username))
                {
                    ViewBag.Message = "User name not found in session.";
                    return View();
                }

                var details = db.Whole_response.Where(x => x.receiver_id == teacherid && x.sender_id == parentid).ToList();

                if (details.Any())
                {


                    var viewModel = new PrincipalCompalintPenalVM
                    {
                        whole_Responses = details,
                    };

                    return View(viewModel);
                }
                else
                {
                    var viewModel = new PrincipalCompalintPenalVM
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
                // Return an appropriate error view or redirect to an error page
                return View("Error");
            }
        }
        public ActionResult ViewAccountsResponse(int rollno, int financeid )
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

                var id = db.AddStudentDatas.FirstOrDefault(x => x.RollNO == rollno);

                var parentid = id.parent_id;

                if (string.IsNullOrEmpty(username))
                {
                    ViewBag.Message = "User name not found in session.";
                    return View();
                }

                var details = db.NoticeReplies.Where(x => x.receiver_id == financeid && x.sender_id == parentid).ToList();

                if (details.Any())
                {


                    var viewModel = new PrincipalCompalintPenalVM
                    {
                        NoticeReplies = details,
                    };

                    return View(viewModel);
                }
                else
                {
                    var viewModel = new PrincipalCompalintPenalVM
                    {
                        NoticeReplies = new List<NoticeReply>(),
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
                // Return an appropriate error view or redirect to an error page
                return View("Error");
            }
        }

        private int GetViewTeachersCount()
        {

            try
            {

                var us = db.CreateAccounts.Where(x => x.UserType == "Teacher").Select(x=>x.account_id).ToList();
               

                string Username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userid = user.account_id;
                var users = db.complaints.Where(u => u.Checked==false).ToList();
                int newSpecificCount = 0;
                int zero = 0;
                foreach (var roll in users)
                {
                    int count = db.specific_complaint.Where(c =>us.Contains(c.teacher_id)&&c.Checked == false).Count();
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


                int totalcount = newSpecificCount;

                return totalcount;
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return a default count or an error indicator
                return -1; // Return -1 to indicate an error occurred
            }
        }

        private int GetViewAccountCount()
        {

            try
            {
                var us = db.CreateAccounts.FirstOrDefault(x => x.UserType == "Finance");
                var type = us.account_id;
                string Username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userid = user.account_id;
                var users = db.FeeNotices.Where(u => u.Checked==false).ToList();
                int newSpecificCount = 0;
                int zero = 0;
                foreach (var roll in users)
                {
                    int count = db.FeeNotices.Where(c => c.Checked == false&&c.finance_id==type ).Count();
                    if (count > 0)
                    {
                        newSpecificCount = count;
                    }
                    else
                    {
                        zero = 0;
                        break;
                    }
                }


                int totalcount = newSpecificCount;

                return totalcount;
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return a default count or an error indicator
                return -1; // Return -1 to indicate an error occurred
            }
        }


        private int GetViewTutorsCount()
        {

            try
            {
                var us = db.CreateAccounts.Where(x => x.UserType == "Tutor").Select(x=>x.account_id).ToList();
                
                string Username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
                int userid = user.account_id;
                var users = db.specific_complaint.Where(u => u.Checked == false).ToList();
                
                int newSpecificCount = 0;
                int zero = 0;
                foreach (var roll in users)
                {
                    int count = db.specific_complaint.Where(c =>us.Contains(c.teacher_id)&& c.Checked == false).Count();
                    if (count > 0)
                    {
                        newSpecificCount = count;
                    }
                    else
                    {
                        zero = 0;
                        break;
                    }
                }


                int totalcount =newSpecificCount;

                return totalcount;
            }
            catch (Exception ex)
            {
                // Handle the exception or log the error
                // For example:
                Console.WriteLine("An error occurred: " + ex.Message);
                // Return a default count or an error indicator
                return -1; // Return -1 to indicate an error occurred
            }
        }
    }
}