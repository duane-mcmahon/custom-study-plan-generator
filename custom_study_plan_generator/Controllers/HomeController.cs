using custom_study_plan_generator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data.Entity;
using custom_study_plan_generator.MetaObjects;
using System.Diagnostics;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using custom_study_plan_generator.App_Start;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Download;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using File = Google.Apis.Drive.v2.Data.File;

using custom_study_plan_generator.StudyPlanAlgorithm;
using Microsoft.Ajax.Utilities;

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

            /* Reset the unit list if the course dropdown list changes */
            if (Session["CurrentCourse"] != null)
            {
                if (courseSelect != Session["CurrentCourse"].ToString())
                {
                    Session["DefaultPlanList"] = null;
                }
            }
            /* Store the currently selected course */
            Session["CurrentCourse"] = courseSelect;

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

                ViewBag.numUnits = 24;

                /* If there has been a course selected and submitted in the drop down list */
                if (!String.IsNullOrEmpty(courseSelect))
                {

                    Session["CourseSelect"] = courseSelect;
                    
                    /* Get the matching course and put it into a meta object */
                    var course = (from c in db.Courses
                                    where c.name == courseSelect
                                    select new CourseDTO
                                    {
                                        course_code = c.course_code,
                                        duration = c.duration,
                                        name = c.name,
                                        num_units = c.num_units,
                                        max_credit = c.max_credit
                                    }).FirstOrDefault();

                    /* Send the number of units to the view for correct table size generation */
                    ViewBag.numUnits = course.num_units;

                    /* Select the plan that matches the meta course */
                    plans = plans.Where(u => u.course_code == course.course_code).OrderBy(u => u.unit_no);

                    /* join the units and plans tables to make them sortable by semester */
                    var query = db.Units.Join(plans, u => u.unit_code, p => p.unit_code, (order, plan) => new { plan.unit_no, order.name });

                    /* sort the query by semester */
                    query = query.OrderBy(u => u.unit_no);

                    /* Convert the matched units to only represent unit names */
                    var unitNamesFiltered = from u in query
                                            select u.name;

                    /* Convert the list of unit names to a seperate list which is usable by eager loading
                        * (This step is needed for when the database is disposed of */
                    var selectedList = new List<string>(unitNamesFiltered);

                    for (var x = 0; x < ViewBag.numUnits; x++)
                    {
                        if (selectedList.ElementAtOrDefault(x) == null)
                        {
                            selectedList.Insert(x, "");
                        }
                    }

                    if (Session["DefaultPlanList"] == null)
                    {
                        /* Pass the unit list to a session variable */
                        Session["DefaultPlanList"] = selectedList;
                    }
                    
                    /* Alert the view that a course has been selected, otherwise a blank page will be loaded */
                    ViewBag.courseSelected = true;

                    /* Check if any of the current units in the unit list are missing their prerequisites */
                    /* ********************************************************************************** */

                    /* Get the list of default plan units from session */
                    var unitList = Session["DefaultPlanList"] as List<string>;
                    
                    /* Initialise two lists required for checking and returning the problem units */
                    List<string> violatedList = new List<string>();
                    List<string> unitsChecked = new List<string>();

                    /* Get the course code from the session stroed selected course */
                    var courseSelected = Session["CourseSelect"].ToString();
                    var courseCode = from c in db.Courses
                                     where c.name == courseSelected
                                     select c.course_code;
                    
                    /* Loop through the unit list */
                    foreach (var unit in unitList)
                    {

                        /* Add current unit to the list of units that have been checked for violations */
                        unitsChecked.Add(unit);

                        /* Get the unit code of the unit currently being checked */
                        var unitToCheck = from u in db.Units
                                          where u.name == unit
                                          select u.unit_code;
                        
                        /* Get the unit prereq codes of the unit being checked (if any) */
                        var prereqs = from p in db.UnitPrerequisites
                                      where unitToCheck.Contains(p.unit_code)
                                      where courseCode.Contains(p.course_code)
                                      select p.prereq_code;

                        /* Convert the prereq codes to unit names */
                        var prereqNames = from u in db.Units
                                          where prereqs.Contains(u.unit_code)
                                          select u.name;
                        
                        /* If the unit has both it's prereqs before it, do nothing, else add it to the violated list */
                        if (prereqNames.Count() > 0)
                        {
                            if (!prereqNames.Except(unitsChecked).Any())
                            {

                            }
                            else
                            {
                                violatedList.Add(unit);
                            }
                        }

                        ViewBag.violatedList = violatedList;

                    }

                }

                else
                {
                    /* No course is selected, load a blank page */
                    ViewBag.courseSelected = false;
                    Session["DefaultPlanList"] = null;
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

        [HttpPost]
        public void DefaultPlanAdd()
        {

            var data = Request["data"].ToString();
            string[] values = data.Split(',');
            var element = Convert.ToInt32(values[0]) - 1;
            var unitName = values[1].ToString();

            var unitList = Session["DefaultPlanList"] as List<string>;

            unitList[element] = unitName;

            Session["DefaultPlanList"] = unitList;
            
        }

        [HttpPost]
        public void DefaultPlanRemove()
        {

            var data = Request["data"].ToString();
            var element = Convert.ToInt32(data) - 1;

            var unitList = Session["DefaultPlanList"] as List<string>;

            unitList[element] = "";

            Session["DefaultPlanList"] = unitList;

        }

        [HttpPost]
        public void DefaultPlanReset()
        {

            Session["DefaultPlanList"] = null;

        }

        [HttpPost]
        public void DefaultPlanSave()
        {

            using (custom_study_plan_generatorEntities db = new custom_study_plan_generatorEntities())
            {

                var unitList = Session["DefaultPlanList"] as List<string>;
                var courseSelect = Session["CourseSelect"].ToString();
                
                Dictionary<Int32, string> listOrdered = new Dictionary<Int32, string>();
                var count = 0;
                foreach (var item in unitList) 
                {
                    listOrdered.Add(count , item);
                    count++;
                }


                
                /* Get the matching course and put it into a meta object */
                var course = from c in db.Courses
                             where c.name == courseSelect
                             select c.course_code;

                var units = from a in listOrdered
                            join u in db.Units
                            on new { name = a.Value } equals
                            new { u.name }
                            orderby a.Key
                            select u;

                var defaultPlan = from dp in db.DefaultPlans
                                  where dp.course_code == course.FirstOrDefault()
                                  select dp;

                foreach (var unit in defaultPlan)
                {
                    db.DefaultPlans.Remove(unit);                 
                }


                count = 1;
                foreach (var u in units)
                {
                    DefaultPlan plan = new DefaultPlan();
                    plan.unit_code = u.unit_code;
                    plan.course_code = course.FirstOrDefault();
                    plan.unit_no = count;
                    plan.semester = (int)Math.Ceiling((double)count/4);
                    count++;
                    db.DefaultPlans.Add(plan);
                }

                db.SaveChanges();

            }
 
        }

        [HttpPost]
        public string GetPrerequisites()
        {

            using (custom_study_plan_generatorEntities db = new custom_study_plan_generatorEntities())
            {
                    
                    /* Get unit name from ajax POST */ 
                    var unit = Request["data"].ToString();
                
                    /* Get course code fron name stored in session */
                    var courseSelected = Session["CourseSelect"].ToString();
                    var courseCode = from c in db.Courses
                                     where c.name == courseSelected
                                     select c.course_code;

                    /* Get the unit code of the unit */
                    var unitToCheck = from u in db.Units
                                      where u.name == unit
                                      select u.unit_code;
                        
                    /* Get the unit prereq codes of the unit being checked (if any) */
                    var prereqs = from p in db.UnitPrerequisites
                                    where unitToCheck.Contains(p.unit_code)
                                    where courseCode.Contains(p.course_code)
                                    select p.prereq_code;

                    /* Convert the prereq codes to unit names */
                    var prereqNames = from u in db.Units
                                        where prereqs.Contains(u.unit_code)
                                        select u.name;

                    var prereqList = "";
                    var count = 0;
                    foreach (var prereq in prereqNames) {
                        if (count == 0) 
                        {
                            prereqList += prereq.ToString();
                        }
                        if (count > 0)
                        {
                            prereqList = prereqList + "," + prereq.ToString();
                        }
                        
                        count++;
                    }

                    return prereqList;
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

        public ActionResult CreatePlan(string courseSelect)
        {

            /* open database so that it will be autmoatically disposed */
            using (custom_study_plan_generatorEntities db = new custom_study_plan_generatorEntities())
            {
                /* Reset any session variables */
                Session["StudentPlan"] = null;
                Session["numUnits"] = null;

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

                ViewBag.numUnits = 24;

                /* If there has been a course selected and submitted in the drop down list */
                if (!String.IsNullOrEmpty(courseSelect))
                {
                    /* Get the matching course and put it into a meta object */
                    var course = (from c in db.Courses
                                  where c.name == courseSelect
                                  select new CourseDTO
                                  {
                                      course_code = c.course_code,
                                      duration = c.duration,
                                      name = c.name,
                                      num_units = c.num_units,
                                      max_credit = c.max_credit
                                  }).FirstOrDefault();

                    /* Send the number of units to the view for correct table size generation */
                    ViewBag.numUnits = course.num_units;
                    Session["numUnits"] = course.num_units;
                    Session["Course"] = course;

                    /* Select the plan that matches the meta course */
                    plans = plans.Where(u => u.course_code == course.course_code).OrderBy(u => u.unit_no);

                    /* join the units and plans tables to make them sortable by semester */
                    var query = db.Units.Join(plans, u => u.unit_code, p => p.unit_code, (order, plan) => new { plan.unit_no, order.name });

                    /* sort the query by semester */
                    query = query.OrderBy(u => u.unit_no);

                    /* Convert the matched units to only represent unit names */
                    var unitNamesFiltered = from u in query
                                            select u.name;

                    /* Convert the list of unit names to a seperate list which is usable by eager loading
                     * (This step is needed for when the database is disposed of */
                    var selectedList = new List<string>(unitNamesFiltered);

                    /* Pass the unit list to the view */
                    ViewBag.unitListSelected = selectedList;
                    /* Alert the view that a course has been selected, otherwise a blank page will be loaded */
                    ViewBag.courseSelected = true;

                    /* Create a list of combined unit/default plan (CoursePlan type) objects and add them to a session variable */
                    /* The list will be in order, and accessible by element number */
                    /* The list can be used to track changes to the unit position */

                    var sessionQuery = db.Units.Join(plans, u => u.unit_code, p => p.unit_code, (order, plan) => new CoursePlan { position = plan.unit_no, semester = plan.semester, unit_code = order.unit_code, name = order.name, type_code = order.type_code, semester1 = order.semester1, semester2 = order.semester2, exempt = false, preferred_year = order.preferred_year });

                    sessionQuery = sessionQuery.OrderBy(u => u.position);

                    List<CoursePlan> sessionList = sessionQuery.ToList();

                    Session["StudentPlan"] = sessionList;

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

        public ActionResult Exemptions(string removeExemptions, string next)
        {
            // Check a valid DefaultPlan is in the Session variable.
            if (Session["StudentPlan"] == null)
            {
                // No Course has been selected - Redirect back to the course selection page.
                return RedirectToAction("CreatePlan", "Home");
            }

            // If "loadDefault" button is pressed, return the list of updated units to the view.
            if (!string.IsNullOrEmpty(removeExemptions))
            {
                return View();
            }
            else if (!string.IsNullOrEmpty(next))
            {
                // Check Exemptions have been selected before proceeding.
                int countExempt = 0;

                // Count Exemptions.
                foreach (CoursePlan unit in (List<CoursePlan>)Session["StudentPlan"])
                {
                    if (unit.exempt == true)
                    {
                        countExempt++;
                    }
                }

                // Proceed if at least 1 Exemption is selected.
                if (countExempt > 0)
                {
                    return RedirectToAction("Modify", "Home");
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult RemoveExemptions()
        {
            // Remove exemptions from plan, return true or false.
            // Receives a string of unit id's to remove from the plan in the format of a string: "1,2,3,4,5" etc.
            // Check if Exemptions selected are valid. 
            if (!string.IsNullOrEmpty(Request["data[]"]))
            {
                var data = Request["data[]"].ToString();
                string[] exemptions = data.Split(',');
                int countExistingExempt = 0;
                int totalExempt = 0;

                // Check for previously selected Exemptions removed.
                foreach (CoursePlan unit in (List<CoursePlan>)Session["StudentPlan"])
                {
                    if (unit.exempt == true)
                    {
                        countExistingExempt++;
                    }
                }

                // Add total of exemptions in Session variable and newly selected. 
                totalExempt = countExistingExempt + exemptions.Length;

                // Make sure Exemption Limit has not been reached. 
                if (totalExempt > 0 && totalExempt <= ((CourseDTO)Session["Course"]).max_credit)
                {
                    // Valid number of Exemptions has been selected - mark the Exemptions in the session variable. 
                    foreach (string id in exemptions)
                    {
                        // Convert string to int.
                        int pos = Convert.ToInt32(id);

                        // Mark each selected unit as Exempt in the Plan session variable.
                        foreach (CoursePlan unit in (List<CoursePlan>)Session["StudentPlan"])
                        {
                            if (unit.position == pos)
                            {
                                unit.exempt = true;
                                break;
                            }
                        }
                    }

                    // Return Success.
                    return Content("Exemptions successfully removed.", MediaTypeNames.Text.Plain);
                }
                else
                {
                    // Exemption Limit has been exceeded.
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Content("Exemption Limit exceeded: Please select less units.", MediaTypeNames.Text.Plain);
                }
            }
            else
            {
                // No Exemptions selected.
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Content("Please select exemptions to remove.", MediaTypeNames.Text.Plain);
            }
        }

        public ActionResult Modify()
        {
            // Check a valid DefaultPlan is in the Session variable.
            if (Session["StudentPlan"] == null)
            {
                // No Course has been selected - Redirect back to the Index page.
                return RedirectToAction("Index", "Home");
            }

            // Retrieve sessionList of coursePlan (units) from Session variable
            // StudentPlan.
            /*List<CoursePlan> sessionList = (List<CoursePlan>)Session["StudentPlan"];
            // Create StudyPlanAlgorithm object;
            StudyPlanAlgorithm.StudyPlanAlgorithm algorithm = new StudyPlanAlgorithm.StudyPlanAlgorithm();
            // Update sessionList by passing it to newly create StudyPlanAlgorithm.
            sessionList = algorithm.RunAlgorithm(sessionList);
            // Update Session["StudentPlan"]
            Session["StudentPlan"] = sessionList;*/

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

        
        public ActionResult submitPlan()
        {
       
            return View();
        }

        [HttpPost]
        public ActionResult submitPlan(FileModel model)
        {

            if (ModelState.IsValid)
            {
                Session["Step1"] = model;
                return RedirectToAction("submitPlanAsync");
            }

            // errors
            return View(model);
        }
        [Authorize]
        public async Task<ActionResult> submitPlanAsync(CancellationToken cancellationToken)
        {
            ViewBag.Message = "Plan Submission Page.";

            var step1 = Session["Step1"] as FileModel;

            Session.Remove("Step1");

            var result = await new AuthorizationCodeMvcApp(this, new AppAuthFlowMetadata()).
                    AuthorizeAsync(cancellationToken);

            if (result.Credential == null)
                return new RedirectResult(result.RedirectUri);

            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = result.Credential,
                ApplicationName = "AuthExample"
            });

            var folderListReq = driveService.Files.List();
            folderListReq.Fields = "items/title,items/id";
            // Set query
            folderListReq.Q = "mimeType = 'application/vnd.google-apps.folder' and title ='" + StudyPlanModel.StudyPlanDirectory + "' and trashed = false";
            FileList folderList = await folderListReq.ExecuteAsync();


            File returnedFile = null;

            
            if (folderList.Items.Count >= 1)
            {
                // If multiple folders with StudyPlanModel.StudyPlanDirectory title always choose first one
                File studyPlanFolder = folderList.Items.First();

                // TODO figure out if page token is necessary here
                var fileListReq = driveService.Files.List();
                fileListReq.Fields = "items/title,items/id";
                // Get all spreadsheets in the studyPlanFolder
                fileListReq.Q = "'" + studyPlanFolder.Id + "' in parents and mimeType = 'application/vnd.google-apps.spreadsheet' and trashed = false";
                FileList fileList = await fileListReq.ExecuteAsync();

                returnedFile = StudyPlanModel.generateGoogleSpreadSheet(driveService, step1.Title, studyPlanFolder.Id, fileList);

            }
            else 
            {
                var folder = StudyPlanModel.createDirectory(driveService, StudyPlanModel.StudyPlanDirectory, "RMIT", "root");

                returnedFile = StudyPlanModel.generateGoogleSpreadSheet(driveService, step1.Title, folder.Id);

            }
            // Permission args are currently hardcoded. Uncomment and replace STUDENTNUMBER to enable sharing of the file.
            //StudyPlanModel.addPermission(driveService, returnedFile.Id, STUDENTNUMBER + "@student.rmit.edu.au", "user", "reader");


            //todo...

            // For javascript sharing popup
            ViewBag.UserAccessToken = result.Credential.Token.AccessToken;
            ViewBag.FileId = returnedFile.Id;
            return View(step1);

        }


        [Authorize]
        public async Task<ActionResult> driveAsync(CancellationToken cancellationToken)
        {
            ViewBag.Message = "Your drive page.";

            var result = await new AuthorizationCodeMvcApp(this, new AppAuthFlowMetadata()).
                    AuthorizeAsync(cancellationToken);

            if (result.Credential == null)
                return new RedirectResult(result.RedirectUri);

            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = result.Credential,
                ApplicationName = "Custom Study Plan Generator"
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