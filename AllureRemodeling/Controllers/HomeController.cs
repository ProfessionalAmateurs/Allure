using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AllureRemodeling.Models;
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
    }
}