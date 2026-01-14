using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject.Areas.Tutor.Controllers
{
    public class AssignedScheduleController : Controller
    {
        // GET: Tutor/AssignedSchedule

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

                var getschedule = db.InsertSchedules.Where(x => x.Tutor_id == userId).ToList();

                var vm = new ScheduleVM
                {

                    InsertSchedules = getschedule,
                };


                return View(vm);

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return View();
            }
        }


        public ActionResult EditSchedule(string startTime, string endTime, string[] checkedDays)
        {
            string username = Session["User-Name"] as string;
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;



            // Convert start time to 24-hour format
            var startDateTime = DateTime.ParseExact(startTime, "hh:mm tt", CultureInfo.InvariantCulture);
            var convertedStartTime = startDateTime.ToString("HH:mm");

            // Convert end time to 24-hour format
            var endDateTime = DateTime.ParseExact(endTime, "hh:mm tt", CultureInfo.InvariantCulture);
            var convertedEndTime = endDateTime.ToString("HH:mm");
            var getschedule = db.InsertSchedules.Where(x => x.Tutor_id == userId
            && x.StartTime_AMPM == convertedStartTime
            && x.ENDTime_AMPM == convertedEndTime).ToList();
            var model = new InsertSchedule
            {
                InsertSchedules = getschedule,
                StartTime_AMPM = convertedStartTime,
                ENDTime_AMPM = convertedEndTime
            };

            foreach (var day in checkedDays)
            {
                model.Day = day;
                // Perform the desired logic with the assigned day value
            }

            // Pass the model object to the view
            return View(model);
        }

        [HttpPost]
        public ActionResult EditSchedule(InsertSchedule obj, string startTime, string endTime, string[] checkedDays)
        {
            string username = Session["User-Name"] as string;
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;

            var startDateTime = DateTime.ParseExact(startTime, "hh:mm tt", CultureInfo.InvariantCulture);
            var convertedStartTime = startDateTime.ToString("HH:mm");

            // Convert end time to 24-hour format
            var endDateTime = DateTime.ParseExact(endTime, "hh:mm tt", CultureInfo.InvariantCulture);
            var convertedEndTime = endDateTime.ToString("HH:mm");

            // Retrieve the InsertSchedules associated with the user
            var insertSchedules = db.InsertSchedules.Where(s => s.Tutor_id == userId).ToList();

            foreach (var day in checkedDays)
            {
                // Check if an InsertSchedule already exists for the day
                var insertSchedule = insertSchedules.FirstOrDefault(s => s.Day == day);

                if (insertSchedule != null)
                {
                    // Update the existing InsertSchedule
                    insertSchedule.StartTime_AMPM = convertedStartTime;
                    insertSchedule.ENDTime_AMPM = convertedEndTime;
                    db.SaveChanges();
                }
                else
                {
                    // Create a new InsertSchedule for the day
                    var newInsertSchedule = new InsertSchedule
                    {
                        Tutor_id = userId,
                        Day = day,
                        StartTime_AMPM = convertedStartTime,
                        ENDTime_AMPM = convertedEndTime
                    };
                    db.InsertSchedules.Add(newInsertSchedule);
                    db.SaveChanges();
                }
                var uncheckedDays = checkedDays.Except(insertSchedules.Select(s => s.Day));
                foreach (var dayy in uncheckedDays)
                {
                    var newInsertSchedule = new InsertSchedule
                    {
                        Tutor_id = userId,
                        Day = dayy,
                        StartTime_AMPM = convertedStartTime,
                        ENDTime_AMPM = convertedEndTime
                    };
                    db.InsertSchedules.Add(newInsertSchedule);
                }

            }

            return RedirectToAction("Index", "AssignedSchedule");
        }

        public ActionResult GetTeachSchedule()
        {
            string username = Session["User-Name"] as string;
            var user = db.CreateAccounts.SingleOrDefault(u => u.Username == username);
            int userId = user.account_id;
            

            var getschedule = db.Assign_Tutor_Stds.Where(x => x.Tutor_ID == userId).ToList();

            var vm = new ScheduleVM
            {

                Assign_Tutor_Stds= getschedule,
            };


            return View(vm);




        }
    }
}








    
