using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace custom_study_plan_generator.Controllers
{
	[RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session["dummy"] = "dummy";
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
    }
}