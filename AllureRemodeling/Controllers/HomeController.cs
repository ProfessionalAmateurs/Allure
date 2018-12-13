using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AllureRemodeling.Models;
using System.Data;
using System.Data.SqlClient;
using System.Web.Helpers;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace AllureRemodeling.Controllers
{
    

    public class HomeController : Controller
    {
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
            var success = false;

            if(exists == false)
            {
                 success = db.InsertSystemUser(user);
            }

            return Json(success);
        }
    }
}
