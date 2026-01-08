using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace FinalProject.Areas.Parent.Controllers
{
    public class TutorListController : Controller
    {
        // GET: Parent/TutorList
        OSDFinalEntities db = new OSDFinalEntities();
        public ActionResult Index(string Searchby, string search, int? page)
        {
            try {
                // only search text box
                /*var model = db.Assign_Tutor_Stds.Where(emp => emp.Std_Name == search || search == null&& emp.Request==true).ToList();*/
                string Username = Session["User-Name"] as string;

                if (Username == null)
                {
                    Session.Abandon();
                    return RedirectToAction("Login", "Home", new { area = "" });

                }
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == Username);
            int userId = user.account_id;
                //this code search with using radiobutton task

                /*  if (Searchby == "std_name")
                  {
                      var model = db.assign_std_to_tutor.Where(emp => emp.std_name == search || search == null).ToList();
                      return View(model);

                  }
                 *//* else
                  {
                      var model = db.assign_std_to_tutor.Where(emp => emp.subject.StartsWith(search) || search == null).ToList();*//*.ToPagedList(page ?? 1, 7);*//*
                      return View(model);
                  }*/
                /* string parentUsername = Session["User-Name"].ToString();
                 var children = db.assign_std_to_tutor
                                 .Where(a => a.parent_uname == parentUsername)
                                 .Select(a => new SelectListItem
                                 {
                                     Value = a.id.ToString(),
                                     Text = a.std_name
                                 })
                                 .ToList();
                 ViewBag.SubjectList = new SelectList(children, "Value", "Text");*/



                var tutorlist = db.Assign_Tutor_Stds
      .Where(x => x.parent_id == userId)
      .Select(x => new
      {
          x.Std_Name,
          x.Tutor_ID,
          x.Fee,
          x.subject,
          x.Status,

      })
      .Distinct()
      .ToList();

                var assignTutorStdsList = tutorlist.Select(item => new Assign_Tutor_Std
                {
                    Std_Name = item.Std_Name,
                    Tutor_ID = item.Tutor_ID,
                    Fee = item.Fee,
                  subject=item.subject,
                  Status=item.Status,
                  
                }).ToList();
                var vm = new HireTutorVM
                {


                    assign_Tutor_Stds= assignTutorStdsList,


                };
                
           

            return View(vm);
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




        [HttpGet]
        public ActionResult PayFee(string subject, int tutorid, string Std,decimal Fee)
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
                    .Where(u => u.account_id == tutorid)
                    .Select(u => u.Username)
                    .FirstOrDefault();

                // Store the section and Username parameters in session variables
                Session["Details"] = subject;
                Session["Student"] = Std;
                Session["Fee"] = Fee;
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

        public ActionResult PayFee(TutorFee obj, string teacherName, string Std, decimal Fee)
        {
            try
            {
                string username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
                int userId = user.account_id;

                var rollno = db.AddStudentDatas.Where(x => x.Name == Std).Select(x => x.RollNO).FirstOrDefault();

                var teacher = db.CreateAccounts.SingleOrDefault(u => u.Username == teacherName);
                int teacherid = teacher.account_id;

                int feeInt = Decimal.ToInt32(Fee);

               /* var assignTutorStdList = db.Assign_Tutor_Stds.Where(x => x.Std_Name == Std).ToList();

                if (assignTutorStdList != null)
                {
                    foreach (var assignTutorStd in assignTutorStdList)
                    {
                        assignTutorStd.Status = "Paid";
                    }

                    db.SaveChanges();
                }
*/

                // Create a new TutorFee object and save it to the database
                TutorFee sd = new TutorFee()
                {
                    Sender_id = userId,
                    RollNo = rollno,
                    Receiver_id = teacherid,
                    subject = obj.subject,
                    Fee = feeInt,
                    Statuss = "Pending",
                };

                db.TutorFees.Add(sd);
                db.SaveChanges();

                sd.Receiver_id = 0;
                sd.Sender_id = 0;

                ModelState.Clear();

                // Return the view with the new TutorFee object as the model
                return RedirectToAction("Index",sd);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Json("An error occurred", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult GiveRating(int tutorid, int rating)
        {
            var tutor = db.CreateAccounts.SingleOrDefault(u => u.account_id == tutorid);
            if (tutor != null)
            {
                TutorRating sd = new TutorRating();
                    sd.Rating = rating; 
                    sd.TutorId = tutorid;
                db.TutorRatings.Add(sd);
                db.SaveChanges();
                return RedirectToAction("Index"); // Replace "Index" with the appropriate action/method name to display the updated list of tutors
            }
            else
            {
                // Handle tutor not found error
                return RedirectToAction("Error"); // Replace "Error" with the appropriate action/method name to handle errors
            }
        }

    }



}
