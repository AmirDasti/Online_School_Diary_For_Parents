using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Globalization;

using System.Web.Mvc;

namespace FinalProject.Areas.Tutor.Controllers
{
    public class ScheduleController : Controller
    {
        // GET: Tutor/Schedule
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

                var tutorid = db.AssignSubjectTutors.Where(x => x.Tutor_id == userId).Select(x => x.s_id).ToList();

               
                var subjects = db.SubjecTutors.Where(x => tutorid.Contains(x.s_id)).Select(x => x.Subject).Distinct().ToList();

                var getschedule = db.InsertSchedules.Where(x => x.Tutor_id == userId).ToList();

                var vm = new ScheduleVM
                {
                    AllSubjects = subjects,
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

        [HttpPost]
        public ActionResult Index(ScheduleVM obj, string[] Day)
        {
            try
            {
                string username = Session["User-Name"].ToString();
                var user = db.CreateAccounts.FirstOrDefault(u => u.Username == username);
                int userId = user.account_id;

                



                foreach (string day in Day)
                {
                    /*   var lastEndTime = db.InsertSchedules
         .Where(d => d.Day == day && d.Tutor_id == userId)
         .OrderByDescending(d => d.ENDTime_AMPM) // Assuming ID is the primary key or an identifier that represents the order of insertion
         .Select(d => d.ENDTime_AMPM)
         .FirstOrDefault();

                       if (lastEndTime != null)
                       {
                           // Add 30 minutes to the lastEndTime
                           DateTime endTimeWithMargin = DateTime.ParseExact(lastEndTime, "HH:mm", CultureInfo.InvariantCulture).AddMinutes(30);



                           bool isDuplicate = db.InsertSchedules
                           .Where(d => d.Day == day && d.Tutor_id == userId)
                           .Any(d =>
                               (d.StartTime_AMPM.CompareTo(obj.Start) <= 0 && d.ENDTime_AMPM.CompareTo(obj.Start) >= 0) ||
                               (d.StartTime_AMPM.CompareTo(obj.END) <= 0 && d.ENDTime_AMPM.CompareTo(endTimeWithMargin) >= 0) ||
                               (d.StartTime_AMPM.CompareTo(obj.Start) >= 0 && d.ENDTime_AMPM.CompareTo(endTimeWithMargin) <= 0)
                           );*/

                    bool isDuplicate = db.InsertSchedules.Any(d => d.Day == day &&
      ((d.StartTime_AMPM.CompareTo(obj.Start) <= 0 && d.ENDTime_AMPM.CompareTo(obj.Start) >= 0) ||
      (d.StartTime_AMPM.CompareTo(obj.END) <= 0 && d.ENDTime_AMPM.CompareTo(obj.END) >= 0) ||
      (d.StartTime_AMPM.CompareTo(obj.Start) >= 0 && d.ENDTime_AMPM.CompareTo(obj.END) <= 0)) &&
      d.Tutor_id == userId);

                    if (isDuplicate)
                        {
                            TempData["Message"] = "Day Time Already inserted.";
                        }
                        else
                        {
                            DateTime startTime;
                            DateTime endTime;

                            if (DateTime.TryParseExact(obj.Start, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime) &&
                            DateTime.TryParseExact(obj.END, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out endTime))

                            {

                                // Calculation and validation with TimeSpan values
                                TimeSpan duration = endTime - startTime;

                                // Check if the duration is at least 1 hour
                                if (duration >= TimeSpan.FromHours(1))
                                {
                                    InsertSchedule d = new InsertSchedule();
                                    d.Tutor_id = userId;

                                    d.Day = day;
                                    d.StartTime_AMPM = obj.Start;
                                    d.ENDTime_AMPM = obj.END;

                                    d.Address = "Ghouri Town,Islambad";
                                    d.Fee = "4500";
                                    d.Status = "Avialble";
                                    d.Subjects = "English";

                                    db.InsertSchedules.Add(d);
                                    db.SaveChanges();


                                    /*d.StartTime_AMPM = startTime.ToString(@"hh\:mm tt");
                                    d.ENDTime_AMPM = endTime.ToString(@"hh\:mm tt");*/

                                    // Rest of your code to save the record
                                    // ...
                                }
                                else
                                {
                                    TempData["Message"] = "Minimum 1-hour time slot required.";
                                }
                            }
                            else
                            {
                                TempData["Message"] = "Invalid start or end time format.";
                            }
                        }
                    

                    // Check if a record with the same day and timing already exists
                }


                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in the ViewResponse action: " + ex.Message);
                // Handle the error in an appropriate manner, such as displaying an error message to the user or redirecting to an error page
                return RedirectToAction("Index");
            }
        }



    }
}
