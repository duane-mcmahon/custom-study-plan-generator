using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using custom_study_plan_generator.Models;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;
using custom_study_plan_generator.MetaObjects;

namespace custom_study_plan_generator.Controllers
{
    public class StudentsController : Controller
    {
        private custom_study_plan_generatorEntities db = new custom_study_plan_generatorEntities();

        // GET: Students
        public ActionResult Index()
        {
            return View(db.Students.ToList());
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "student_id,firstname,lastname")] StudentMeta student)
        {

            var studentCheck = from stud in db.Students
                               where stud.student_id == student.student_id
                               select stud;

            if (studentCheck.Count() > 0)
            {
                Session["studentExists"] = "true";
                return View(student);
            }

            if (ModelState.IsValid)
            {
                
                Student studentAdd = new Student();
                studentAdd.student_id = student.student_id;
                studentAdd.firstname = student.firstname;
                studentAdd.lastname = student.lastname;
                
                db.Students.Add(studentAdd);
                
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

            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);

            StudentMeta studentMeta = new StudentMeta();
            studentMeta.student_id = student.student_id;
            studentMeta.firstname = student.firstname;
            studentMeta.lastname = student.lastname;

            if (student == null)
            {
                return HttpNotFound();
            }
            return View(studentMeta);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "student_id,firstname,lastname")] StudentMeta student)
        {
            if (ModelState.IsValid)
            {

                Student studentEdit = new Student();
                studentEdit.student_id = student.student_id;
                studentEdit.firstname = student.firstname;
                studentEdit.lastname = student.lastname;

                db.Entry(studentEdit).State = EntityState.Modified;

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
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
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
                else
                {
                    Session["SaveDBError"] = true;
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
