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
            /* force ASP.NET session initialsiation, needed for Google OAuth */
            Session["dummy"] = "dummy";

            return View();
        }

        public ActionResult CreateEdit(string create)
        {
 
            if (!string.IsNullOrEmpty(create))
            {
                return RedirectToAction("CreatePlan", "Home");
            }
            else
            {
                return RedirectToAction("EditPlan", "Home");
            }

        }

        public ActionResult CreatePlan()
        {
            

            return View();
        }

        public ActionResult EditPlan()
        {
            

            return View();
        }

        public ActionResult EditCourse()
        {

            return View();
        }
    }
}