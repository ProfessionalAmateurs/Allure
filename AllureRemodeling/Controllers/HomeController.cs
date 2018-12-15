﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AllureRemodeling.Models;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Web.Helpers;
using System.Collections;

namespace AllureRemodeling.Controllers
{
    public class HomeController : Controller
    {
        DatabaseClass db = new DatabaseClass();

        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated == true && Convert.ToInt32(Session["RoleID"]) > 0)
            {
                // User is already logged in -- redirect 
                return RedirectToAction("Index", "Home");
            }

            // Check for error message on redirect
            if (TempData["errorMessage"] != null)
            {
                if (TempData["errorMessage"].ToString().Length > 0)
                {
                    ModelState.AddModelError("", TempData["errorMessage"].ToString());
                }
            }

            return View();
        }


        [HttpPost]
        public ActionResult Login(FormCollection formValues)
        {
            if (ModelState.IsValid)
            {
                // Create new user object 
                User user = new User();

                // Set the values of the user object with values from the form 
                user.UserName = formValues["customerUsername"];
                user.Password = formValues["customerPassword"];
                user.AccountType = 2;

                // Database/dataset object creation/instantiation
                DatabaseClass db = new DatabaseClass();
                DataSet ds = new DataSet();

                var userList = new List<User>();

                // Get info about user based on the object created for the user 
                db.GetUser(ref ds, ref user);

                // Were there any results?
                if (ds.Tables[0].Rows.Count > 0)
                {
                    // Yes, add the user to the list 
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var UserInfo = new User();
                        UserInfo.UserName= dr.Field<string>("UserName");


                        userList.Add(UserInfo);
                    }

                    var response = Request["g-recaptcha-response"];
                    string secretKey = "6LevTFEUAAAAAFPFTFmf_GWQJQtofGvMhUCkNoNQ";
                    var client = new WebClient();
                    var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
                    var obj = JObject.Parse(result);
                    var status = (bool)obj.SelectToken("success");
                    ViewBag.Message = status ? "Google reCaptcha validation success" : "Google reCaptcha validation failed";

                    if (status == true)
                    {
                        // Authenticate user
                        FormsAuthentication.SetAuthCookie(user.UserName, true);

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    // No, Inform user 
                    ModelState.AddModelError("", "Login data is incorrect!");
                }
            }

            return View();
        }

        // ------------------------------------------------------------------------------------------
        // Name: PasswordReset
        // Abstract: show password reset page
        // -----------------------------------------------------------------------------------------
        public ActionResult PasswordReset()
        {
            // Manually log user out when they navigate to this page 
            FormsAuthentication.SignOut();
            Session.Abandon();

            return View();
        }


        // ------------------------------------------------------------------------------------------
        // Name: PasswordReset
        // Abstract: The name says it all
        // -----------------------------------------------------------------------------------------
        [HttpPost]
        public ActionResult PasswordReset(string email)
        {

            // Verify Email ID
            DatabaseClass db = new DatabaseClass();
            DataSet ds = new DataSet();
            bool result = db.EmailCheck(ref ds, email);

            if (result == true)
            {
                //Send email
                string resetCode = Guid.NewGuid().ToString();
                SendVerificationEmail(email, resetCode);
                db.UpdateResetPasswordCode(email, resetCode);
            }

            return RedirectToAction("Login", "User", new { update = result });
        }

        // ------------------------------------------------------------------------------------------
        // Name: SendVerificationEmail
        // Abstract: Send email to change password
        // -----------------------------------------------------------------------------------------
        [NonAction]
        public void SendVerificationEmail(string EmailId, string resetCode)
        {

            var verifyURL = "/User/ChangePassword/" + resetCode + "?code=" + resetCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyURL);

            var fromEmail = new MailAddress("admin@govcollect.com", "Capital Software");
            var toEmail = new MailAddress(EmailId);
            var fromPassword = "GovC4231";
            string subject = "Password Reset";

            string body = "</br></br>We received a request to reset your password.  Please click on the link to reset your password. " +
                                        "<a href='" + link + "'> Reset Password</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromPassword)

            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,

            })
                smtp.Send(message);
        }



        // ------------------------------------------------------------------------------------------
        // Name: ChangePasword
        // Abstract: The name says it all
        // ------------------------------------------------------------------------------------------
        public ActionResult ChangePassword(Guid id)
        {
            DatabaseClass db = new DatabaseClass();
            DataSet ds = new DataSet();
            var model = new List<SystemUsers>();

            db.CheckResetCode(ref ds, id);

            if (ds.Tables[0].Rows.Count < 1)
            {
                return HttpNotFound();
            }

            else
            {
                ResetPasswordModel resetPassword = new ResetPasswordModel();
                resetPassword.ResetCode = id.ToString();
                return View(resetPassword);
            }

        }



        // ------------------------------------------------------------------------------------------
        // Name: ChangePassword
        // Abstract: Updates the password in the system
        // ------------------------------------------------------------------------------------------
        public JsonResult UpdatePassword(string updateCode, string newPassword)
        {
            DatabaseClass db = new DatabaseClass();

            bool success = db.UpdateUserPassword(updateCode, newPassword);

            return Json(success);
        }


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
            ViewBag.Message = "Your Services page.";

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
        {
            DatabaseClass db = new DatabaseClass();

            var success = db.AddCustomerAccount(customerAccount);

            return Json(success);
        }

        public JsonResult AddSystemUser(SystemUsers user)
        {
            DatabaseClass db = new DatabaseClass();

            var exists = db.CheckForExistingUser(user.Username);

            var success = false;

            if(exists == false)
            {
                 success = db.InsertSystemUser(user);
            }

            return Json(success);
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