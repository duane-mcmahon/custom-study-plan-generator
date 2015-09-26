using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

            /* Test databse connection, pulling connection string from web.config in the project root */
            
            System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/custom_study_plan_generator");
            System.Configuration.ConnectionStringSettings connString;
            connString = rootWebConfig.ConnectionStrings.ConnectionStrings["CPT331"];
            
            using (var con = new SqlConnection(connString.ToString()))
            {

                con.Open();
            }

            List<string> Courses = new List<string>()
            {
                "Course 1",
                "Course 2",
                "Course 3"

            };

            MultiSelectList CourseList = new MultiSelectList(Courses);

            ViewBag.courseList = CourseList;

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