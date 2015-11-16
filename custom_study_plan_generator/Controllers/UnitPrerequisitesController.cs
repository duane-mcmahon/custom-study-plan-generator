using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using custom_study_plan_generator.Models;
using custom_study_plan_generator.Views.UnitPrerequisites;
using System.Diagnostics;

namespace custom_study_plan_generator.Controllers
{
    public class UnitPrerequisitesController : Controller
    {
        private custom_study_plan_generatorEntities db = new custom_study_plan_generatorEntities();

        // GET: UnitPrerequisites
        public ActionResult Index()
        {
            var unitPrerequisites = db.UnitPrerequisites.Include(u => u.Unit).Include(u => u.Unit1).Include(u => u.Course);
            return View(unitPrerequisites.ToList());
        }

        // GET: UnitPrerequisites/Create
        public ActionResult Create()
        {


            List<string> units = new List<string>();
            List<string> prereqs = new List<string>();


            ViewBag.prereq_code = new SelectList(prereqs, "unit_code", "name");
            ViewBag.unit_code = new SelectList(units, "unit_code", "name");
            ViewBag.course_code = new SelectList(db.Courses, "course_code", "name");

            return View();
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Select")]
        public ActionResult Select(string course_code)
        {

            if (course_code == "")
            {
                List<string> units = new List<string>();
                List<string> prereqs = new List<string>();
                SelectList courseSelectList = new SelectList(db.Courses, "course_code", "name");

                ViewBag.prereq_code = new SelectList(units, "unit_code", "name");
                ViewBag.unit_code = new SelectList(units, "unit_code", "name");
                ViewBag.course_code = courseSelectList;

                return View();
            }

            else
            {
                SelectList courseSelectList = new SelectList(db.Courses, "course_code", "name");

                courseSelectList.First(item => item.Value.Equals(course_code)).Selected = true;

                var unitsInPlan = from dp in db.DefaultPlans
                                  where dp.course_code == course_code
                                  select dp.unit_code;

                var units = from u in db.Units
                            where unitsInPlan.Contains(u.unit_code)
                            select u;

                ViewBag.prereq_code = new SelectList(units, "unit_code", "name");
                ViewBag.unit_code = new SelectList(units, "unit_code", "name");
                ViewBag.course_code = courseSelectList;
            }

            return View();
        }

        // POST: UnitPrerequisites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MultipleButton(Name = "action", Argument = "CreateSubmit")]
        public ActionResult CreateSubmit([Bind(Include = "unit_code,prereq_code,mutiple_required,course_code")] UnitPrerequisite unitPrerequisite)
        {
            if (ModelState.IsValid)
            {
                var unitPrerequisiteCheck = from up in db.UnitPrerequisites
                                            where up.course_code == unitPrerequisite.course_code
                                            where up.unit_code == unitPrerequisite.unit_code
                                            where up.prereq_code == unitPrerequisite.prereq_code
                                            select up;

                if (unitPrerequisiteCheck.Count() > 0)
                {
                    Session["prereqExists"] = "true";
                    return RedirectToAction("Create");
                }
                else
                {
                    Session["prereqExists"] = null;
                    db.UnitPrerequisites.Add(unitPrerequisite);

                    try
                    {
                        db.SaveChanges();
                    }

                    catch (Exception ex)
                    {
                        Session["SaveDBError"] = true;
                    }

                }

                return RedirectToAction("Index");
            }

            ViewBag.prereq_code = new SelectList(db.Units, "unit_code", "name", unitPrerequisite.prereq_code);
            ViewBag.unit_code = new SelectList(db.Units, "unit_code", "name", unitPrerequisite.unit_code);
            ViewBag.course_code = new SelectList(db.Courses, "course_code", "name", unitPrerequisite.course_code);
            return View(unitPrerequisite);
        }

        [HttpGet]
        public ActionResult Delete(string course_code, string unit_code, string prereq_code)
        {

            var unitPrerequisite = from up in db.UnitPrerequisites
                                   where up.course_code == course_code
                                   where up.unit_code == unit_code
                                   where up.prereq_code == prereq_code
                                   select up;

            db.UnitPrerequisites.Remove(unitPrerequisite.FirstOrDefault());

            try
            {
                db.SaveChanges();
            }

            catch (Exception ex)
            {
                Session["SaveDBError"] = true;
            }

            return RedirectToAction("Index");
        }

    }
}
