using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            ViewBag.Message = "Your Services page.";

            return View();
        }

        public ActionResult CustomerInfo()
        {
            return View();
        }
        //public JsonResult GetPicture()
        //{
        //    DatabaseClass db = new DatabaseClass();

        //    var picture = db.GetPicture();

        //    return Json(picture);
        //}




    }
}