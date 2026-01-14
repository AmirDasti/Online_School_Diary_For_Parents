using FinalProject.Areas.Parent.Controllers;
using FinalProject.Models;
using FinalProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;

namespace FinalProject.Areas.Tutor.Controllers
{
    public class RequestsController : Controller
    {
        // GET: Tutor/Resquests
        OSDFinalEntities db = new OSDFinalEntities();
        public ActionResult Index()
        {
            string username = Session["User-Name"] as string;

            if (username == null)
            {
                Session.Abandon();
                return RedirectToAction("Login", "Home", new { area = "" });

            }
            var tutor=db.CreateAccounts.SingleOrDefault(x=> x.Username == username);
            int tutorid = tutor.account_id;

            var requests=db.Assign_Tutor_Stds.Where(x=>x.Tutor_ID==tutorid && x.Request==false).ToList();
            var viewModel = new ResquestsVM
            {
                Requests = requests
            };

            return View(viewModel);


           
        }
        [HttpPost]
        public ActionResult Index(ResquestsVM obj,int Acceptid,string reject)
        {
            string username = Session["User-Name"].ToString();
            var tutor = db.CreateAccounts.SingleOrDefault(x => x.Username == username);
            int tutorid = tutor.account_id;

            // Retrieve the record from the database
            var request = db.Assign_Tutor_Stds.SingleOrDefault(x => x.Std_Name == obj.Std_Name && x.Tutor_ID == tutorid);

            var Accept = db.Assign_Tutor_Stds.FirstOrDefault(x => x.Tutor_ID == Acceptid);

            var Reject = db.Assign_Tutor_Stds.FirstOrDefault(x => x.Std_Name == reject);

            if (Accept != null)
            {
                // Update the request status to true (accepted)
                request.Request = true;
                // Save the changes to the database
                db.SaveChanges();

                // Perform any additional actions or redirect to a different page
                // For example:
                return View();
            }
            else if(Reject != null)
            {
                request.Request = false;
                // Save the changes to the database
                db.SaveChanges();
            }



            return  View();
        }

        // Assuming you have an action method in your controller to handle the accept and reject requests

        public ActionResult AcceptRequest(bool requestId)
        {
            // Retrieve the request from the database using the requestId
            var request = db.Assign_Tutor_Stds.FirstOrDefault(x => x.Request == requestId);

            if (request != null)
            {
                // Update the request status to true (accepted)
                request.Request = true;
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

        public ActionResult RejectRequest(bool reject)
        {
            // Retrieve the request from the database using the requestId
            var request = db.Assign_Tutor_Stds.FirstOrDefault(x => x.Request == reject);

            if (request != null)
            {
                // Update the request status to false (rejected)
                db.Assign_Tutor_Stds.Remove(request);
                // Save the changes to the database
                db.SaveChanges();
                TempData["Message"] = "Request Deleted";
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





    }
}