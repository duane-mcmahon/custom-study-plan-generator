﻿using custom_study_plan_generator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data.Entity;
using custom_study_plan_generator.MetaObjects;
using System.Diagnostics;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using custom_study_plan_generator.App_Start;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Download;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using File = Google.Apis.Drive.v2.Data.File;

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

        public ActionResult DefaultPlan(string courseSelect)
        {
 
            /* open database so that it will be autmoatically disposed */
            using (custom_study_plan_generatorEntities db = new custom_study_plan_generatorEntities())
            {
                /* Set the default blank course option on page load */
                ViewBag.listValue = "Select Course";
                
                /* Set the list of course units to be blank (will be tailored to a course when course is selected in view */
                ViewBag.unitListSelected = "";
                
                /* Initialise the courses list */
                var courseList = new List<string>();
                
                /* Query the course names from the database */
                var courseQry = from d in db.Courses
                                orderby d.name
                                select d.name;

                /* Add distinct courses to the course list */
                courseList.AddRange(courseQry.Distinct());

                /* Supply the list of courses to the view (to be used in the drop down list) */
                ViewBag.courseSelect = new SelectList(courseList);

                /* Get all available plans */
                var plans = from p in db.DefaultPlans
                            select p;

                /* Get all available units */
                var units = from u in db.Units
                            select u;   

                /* If there has been a course selected and submitted in the drop down list */
                if (!String.IsNullOrEmpty(courseSelect))
                {
                    /* Get the martching course and put it into a meta object */
                    var course = (from c in db.Courses
                                 where c.name == courseSelect
                                 select new CourseDTO
                                 {
                                     course_code = c.course_code,
                                     duration = c.duration,
                                     name = c.name,
                                     num_units = c.num_units
                                 }).FirstOrDefault();

                    /* Select the plan that matches the meta course */
                    plans = plans.Where(u => u.course_code == course.course_code);

                    /* Select units from the complete units list where codes match those in the selected plan */
                    units = units.Where(u => plans.Any(p => p.unit_code == u.unit_code));

                    /* Convert the matched units to only represent unit names */
                    var unitNamesFiltered = from u in units
                                    select u.name;
                    
                    /* Convert the list of unit names to a seperate list which is usable by eager loading
                     * (This step is needed for when the database is disposed of */
                    var selectedList = new List<string>(unitNamesFiltered);
                    
                    /* Pass the unit list to the view */
                    ViewBag.unitListSelected = selectedList;
                    /* Alert the view that a course has been selected, otherwise a blank page will be loaded */
                    ViewBag.courseSelected = true;
                    
                }

                else
                {
                    /* No course is selected, load a blank page */
                    ViewBag.courseSelected = false;
                }

                /* Create a list of all availabe units (at the moment this is aesthetic,
                   this list may actually be hidden from view, but this will prevent an error
                   on selecting no course. This may also be required if a new or incomplete course is loaded 
                   into the view */
                var unitNames = from u in units
                            select u.name;

                /* Convert the unit names to a list, usable by eager loading */
                var list = new List<string>(unitNames);
                /* Sort the list alphabetically */
                list.Sort();
                /* Pass the list to the view */
                ViewBag.unitList = new SelectList(list);

                return View();
            }

            
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


        public ActionResult SubmitPlan()
        {

            return View();
        }


        [Authorize]
        public async Task<ActionResult> DriveAsync(CancellationToken cancellationToken)
        {
            ViewBag.Message = "Your drive page.";

            var result = await new AuthorizationCodeMvcApp(this, new AppAuthFlowMetadata()).
                    AuthorizeAsync(cancellationToken);

            if (result.Credential == null)
                return new RedirectResult(result.RedirectUri);

            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = result.Credential,
                ApplicationName = "ASP.NET Google APIs MVC Sample"
            });

            var listReq = driveService.Files.List();
            listReq.Fields = "items/title,items/id,items/createdDate,items/downloadUrl,items/exportLinks";
            var list = await listReq.ExecuteAsync();
            var items =
                (from file in list.Items
                 select new FileModel
                 {
                     Title = file.Title,
                     Id = file.Id,
                     CreatedDate = file.CreatedDate,
                     DownloadUrl = file.DownloadUrl ??
                                               (file.ExportLinks != null ? file.ExportLinks["application/pdf"] : null),
                 }).OrderBy(f => f.Title).ToList();
            return View(items);
        }
    }

 

}