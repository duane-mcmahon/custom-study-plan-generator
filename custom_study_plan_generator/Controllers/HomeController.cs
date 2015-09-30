using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace custom_study_plan_generator.Controllers
{
    [Authorize]
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

        public ActionResult CreatePlan(string loadDefault, string next)
        {

            /* Test databse connection, pulling connection string from web.config in the project root */
            
            /*System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/custom_study_plan_generator");
            System.Configuration.ConnectionStringSettings connString;
            connString = rootWebConfig.ConnectionStrings.ConnectionStrings["CPT331"];
            
            using (var con = new SqlConnection(connString.ToString()))
            {

                con.Open();
            }*/

            /* ************** END TEST DATABASE *************** */

            /* Create a list to send to default plan - Temporary list*/
            List<string> Courses = new List<string>()
            {
                "Course 1",
                "Course 2",
                "Course 3"

            };

            MultiSelectList CourseList = new MultiSelectList(Courses);

            ViewBag.courseList = CourseList;

            /* If "loadDefault" button is pressed, return the list of units to the view */
            if (!string.IsNullOrEmpty(loadDefault))
            {
               

                return View();
            }

            /* If "next" button is pressed, go to the next step in create plan (Exemptions) */
            else if (!string.IsNullOrEmpty(next))
            {
                return RedirectToAction("Exemptions", "Home");
            }

            return View();

            
        }

        public ActionResult Exemptions(string removeExemptions, string next)
        {

            /* If "loadDefault" button is pressed, return the list of updated units to the view */
            if (!string.IsNullOrEmpty(removeExemptions))
            {


                return View();
            }

            else if (!string.IsNullOrEmpty(next))
            {
                return RedirectToAction("Modify", "Home");
            }

            return View();

    
        }

        [HttpPost]
        public void RemoveExemptions()
        {
            /* Remove exemptions from plan, return true or false */
            /* Receives a string of unit id's to remove from the plan in the format of a string: "1,2,3,4,5" etc */
            string data = Request["data[]"];



        }

        public ActionResult Modify()
        {

            /* Create a list of dummy units to send to modify page- Temporary list*/
            List<string> Units = new List<string>()
            {
                "Unit 1",
                "Unit 2",
                "Unit 3"

            };

            MultiSelectList UnitList = new MultiSelectList(Units);

            ViewBag.unitList = UnitList;

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