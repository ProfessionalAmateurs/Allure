using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        DatabaseClass db = new DatabaseClass();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {

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


        public ActionResult Estimate()
        {
            return View();
        }

        public JsonResult GetEstimateQuestion()
        {
            var questions = db.GetEstimateQuestions();

            return Json(questions);
        }

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

        public JsonResult EmailNewContact(EmailClass NewContact)
        {

            var success = SendEmail(NewContact);
            return Json(success);

        }

        [HttpPost]
        public ActionResult SendEmail(EmailClass NewContact)
        {

            try
            {
                //Configuring webMail class to send emails  
                //gmail smtp server  
                WebMail.SmtpServer = "smtp.gmail.com";
                //gmail port to send emails  
                WebMail.SmtpPort = 587;
                WebMail.SmtpUseDefaultCredentials = true;
                //sending emails with secure protocol  
                WebMail.EnableSsl = true;
                //EmailId used to send emails from application  
                WebMail.UserName = "justlia86@gmail.com";
                WebMail.Password = "Reesa1986";

                //Sender email address.  
                WebMail.From = "justlia86@gmail.com";
                string EmailSubject = "A New Allure Request";
                string EMailBody =
                $@"First Name: " + NewContact.FirstName + "<br/>" +
                "Last Name: " + NewContact.LastName + "<br/>" +
                "Address 1: " + NewContact.Address1 + "<br/>" +
                "Address 2: " + NewContact.Address2 + "<br/>" +
                "City: " + NewContact.City + "<br/>" +
                "State: " + NewContact.State + "<br/>" +
                "Zip Code: " + NewContact.Zip + "<br/>" +
                "Additional Comments: " + NewContact.AdditionalComments;
                string OwnerEmailAddress = "justlia86@gmail.com";
                string ContactAddress = NewContact.EmailAddress;

                //Send email  
                WebMail.Send(to: OwnerEmailAddress, subject: EmailSubject, body: EMailBody, cc: NewContact.EmailAddress, bcc: "", isBodyHtml: true);
            }
            catch (Exception)
            {
                ViewBag.Message = "Problem while sending email, Please check details.";

            }
            return View();
        }
    }
}
