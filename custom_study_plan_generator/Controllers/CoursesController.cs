using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using custom_study_plan_generator.Models;
using System.Data.Entity.Infrastructure;
using custom_study_plan_generator.MetaObjects;

namespace custom_study_plan_generator.Controllers
{
    public class CoursesController : Controller
    {
        private custom_study_plan_generatorEntities db = new custom_study_plan_generatorEntities();

        // GET: Courses
        public ActionResult Index()
        {
            return View(db.Courses.ToList());
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "course_code,name,num_units,duration,max_credit")] CourseMeta course)
        {

            var courseCheck = from crs in db.Courses
                               where crs.course_code == course.course_code
                               select crs;

            if (courseCheck.Count() > 0)
            {
                Session["courseExists"] = "true";
                return View(course);
            }            
            
            if (ModelState.IsValid)
            {

                Course courseAdd = new Course();
                courseAdd.course_code = course.course_code;
                courseAdd.name = course.name;
                courseAdd.num_units = course.num_units;
                courseAdd.max_credit = course.max_credit;

                /* Remove once database property is removed */
                courseAdd.duration = 0;
                
                db.Courses.Add(courseAdd);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);

            CourseMeta courseMeta = new CourseMeta();
            courseMeta.course_code = course.course_code;
            courseMeta.name = course.name;
            courseMeta.num_units = course.num_units;
            courseMeta.max_credit = course.max_credit;

            if (course == null)
            {
                return HttpNotFound();
            }
            return View(courseMeta);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "course_code,name,num_units,duration,max_credit")] CourseMeta course)
        {
            if (ModelState.IsValid)
            {

                Course courseEdit = new Course();
                courseEdit.course_code = course.course_code;
                courseEdit.name = course.name;
                courseEdit.num_units = course.num_units;
                courseEdit.max_credit = course.max_credit;

                /* Remove once database property is removed */
                courseEdit.duration = 0;

                db.Entry(courseEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.ToString().Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    Session["ForeignKeyConstraint"] = "true";
                }
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
