using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AllureRemodeling.Models;
using System.Data;
using System.Data.SqlClient;




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
            for (var i = 0; i < estimateAnswers.Count; i++)
            {
                db.
            }
        }
        public ActionResult SaveRecord(Estimates estimates, string action, FormCollection frm)
        {
            //foreach (string s in Request.Form.Keys)
            //{
            //    Response.Write(s.ToString() + ":" + Request.Form[s] + 
            //}
            //if (action == "Submit")
            //{

                //    }
                //    List<string> list = new List<string>();
                //    for (int i = 0; i < frm.AllKeys.Count(); ++i)
                //    {
                //        //list.Add(frm.Get(i));
                //        estimates.Answer = frm.Get(i);
                //        db.InsertAnswerData(key, estimates.Answer);
                //    }

                //    return RedirectToAction("Index");
                //}
                //else
                //{
                return ViewBag.message("Please try again.");
            //}
        }


    }
}
