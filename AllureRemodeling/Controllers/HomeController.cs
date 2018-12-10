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


        public ActionResult Estimate()
        {
            return View();
        }

        public JsonResult GetEstimateQuestion()
        {
            var questions = db.GetEstimateQuestions();

            return Json(questions);
        }

        public JsonResult SubmitAnswers(List<Estimates> estimateAnswers)
        {
            var success = false;
            string empty = $@"A Client has sent the following request for estimate";
            for (var i = 0; i < estimateAnswers.Count; i++)
            {
                success = db.InsertAnswerData(estimateAnswers[i]);
                empty += estimateAnswers[i].Question + " : <br/>" + estimateAnswers[i].Answer + ". <br/>";
            }

            Email email = new Email();
            email.sendEmail(empty, "Estimate Request");

            //SaveRecord();
            return Json(success);
        }

    }
}
