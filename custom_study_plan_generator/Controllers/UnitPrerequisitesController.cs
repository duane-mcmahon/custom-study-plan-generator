using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using custom_study_plan_generator.Models;

namespace custom_study_plan_generator.Controllers
{
    public class UnitPrerequisitesController : Controller
    {
        private custom_study_plan_generatorEntities db = new custom_study_plan_generatorEntities();

        // GET: UnitPrerequisites
        public ActionResult Index()
        {
            return View(db.UnitPrerequisites.ToList());
        }

        // GET: UnitPrerequisites/Details/5
        public ActionResult Details(string unit, string prereq)
        {
            if (unit == null || prereq == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitPrerequisite unitPrerequisite = db.UnitPrerequisites.Find(unit, prereq);
            if (unitPrerequisite == null)
            {
                return HttpNotFound();
            } 
            return View(unitPrerequisite);
        }

        // GET: UnitPrerequisites/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UnitPrerequisites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "unit_code,prereq_code,mutiple_required")] UnitPrerequisite unitPrerequisite)
        {
            if (ModelState.IsValid)
            {
                db.UnitPrerequisites.Add(unitPrerequisite);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(unitPrerequisite);
        }

        // GET: UnitPrerequisites/Edit/5
        public ActionResult Edit(string unit, string prereq)
        {
            if (unit == null || prereq == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitPrerequisite unitPrerequisite = db.UnitPrerequisites.Find(unit, prereq);
            if (unitPrerequisite == null)
            {
                return HttpNotFound();
            }
            return View(unitPrerequisite);
        }

        // POST: UnitPrerequisites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "unit_code,prereq_code,mutiple_required")] UnitPrerequisite unitPrerequisite)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unitPrerequisite).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(unitPrerequisite);
        }

        // GET: UnitPrerequisites/Delete/5
        public ActionResult Delete(string unit, string prereq)
        {
            if (unit == null || prereq == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnitPrerequisite unitPrerequisite = db.UnitPrerequisites.Find(unit, prereq);
            if (unitPrerequisite == null)
            {
                return HttpNotFound();
            }
            return View(unitPrerequisite);
        }

        // POST: UnitPrerequisites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string unit, string prereq)
        {
            UnitPrerequisite unitPrerequisite = db.UnitPrerequisites.Find(unit, prereq);
            db.UnitPrerequisites.Remove(unitPrerequisite);
            db.SaveChanges();
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
