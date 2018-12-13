using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AllureRemodeling.Models;
using System.Data;
using System.Data.SqlClient;
using System.Web.Helpers;
using System.Collections;

namespace AllureRemodeling.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Services()
        {
           

            return View();
        }

        public ActionResult CreateCustomerProfile()
        {
            try
            {
                return View();
            }

            catch (Exception exc)
            {
                // manually logs exception to Elmah
                Elmah.ErrorSignal.FromCurrentContext().Raise(exc);

                throw new Exception(exc.Message);
            }
        }

        public JsonResult AddCustomerAccount(Users customerAccount)
        [HttpPost]
        public ActionResult SubmitAnswers(List<Estimates> estimateAnswers, Estimates est)
        {
            var success = false;
          
            string emailbody = $@"A Client has sent the following request for estimate <br/>";
            for (var i = 0; i < estimateAnswers.Count; i++)
            {
                success = db.InsertAnswerData(estimateAnswers[i]);
                emailbody += estimateAnswers[i].Question + " : <br/>" + estimateAnswers[i].Answer + ". <br/>";
            }
          
            Email email = new Email();
            email.sendEmail(emailbody, "Estimate Request");
            
            DatabaseClass db = new DatabaseClass();

            var success = db.AddCustomerAccount(customerAccount);

            return Json(success);

            //SaveRecord();}
        }

        public ActionResult Testimonials()
        {
            
                var Reviews = db.GetReviews();

               
            return View(Reviews);
        }

        //GET: testimonial/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Testimonials/Create
        [HttpPost]
        public ActionResult Create(Testimonials reviews)
        {

            try
            {
                if (ModelState.IsValid)
                {
                  
                    if (db.InsertReviewData(reviews))
                    {
                        ViewBag.Message = "Thank you for your testimonial";
                        ModelState.Clear();
                    }
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        }
        //public JsonResult GetPicture()
        //{
        //    DatabaseClass db = new DatabaseClass();

        //    var picture = db.GetPicture();

        //    return Json(picture);
        //}




    }
}