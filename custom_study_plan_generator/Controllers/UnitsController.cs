using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using custom_study_plan_generator.Models;
using custom_study_plan_generator.MetaObjects;
using System.Data.Entity.Infrastructure;

namespace custom_study_plan_generator.Controllers
{
    public class UnitsController : Controller
    {
        private custom_study_plan_generatorEntities db = new custom_study_plan_generatorEntities();

        // GET: Units
        [Authorize]
        public ActionResult Index()
        {
            var units = db.Units.Include(u => u.UnitType);
            return View(units.ToList());
        }

        // GET: Units/Create
        [Authorize]
        public ActionResult Create()
        {

            return View();
        }

        // POST: Units/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "unit_code,name,semester1,semester2")] UnitMeta unit)
        {

            var unitCheck = from un in db.Units
                               where un.unit_code == unit.unit_code
                               select un;

            if (unitCheck.Count() > 0)
            {
                Session["unitExists"] = "true";
                return View(unit);
            }

            Unit unitAdd = new Unit();
            unitAdd.unit_code = unit.unit_code;
            unitAdd.name = unit.name;
            /* Hard coded, does is exist but not used by the web application */
            unitAdd.type_code = "c";
            unitAdd.semester1 = unit.semester1;
            unitAdd.semester2 = unit.semester2;
            
            if (ModelState.IsValid)
            {

                db.Units.Add(unitAdd);
                
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

            return View(unit);
        }

        // GET: Units/Edit/5
        [Authorize]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = db.Units.Find(id);

            UnitMeta unitEdit = new UnitMeta();
            unitEdit.unit_code = unit.unit_code;
            unitEdit.name = unit.name;
            unitEdit.semester1 = unit.semester1;
            unitEdit.semester2 = unit.semester2;

            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unitEdit);
        }

        // POST: Units/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "unit_code,name,semester1,semester2")] UnitMeta unit)
        {
            if (ModelState.IsValid)
            {
                Unit unitAdd = new Unit();
                unitAdd.unit_code = unit.unit_code;
                unitAdd.name = unit.name;
                unitAdd.semester1 = unit.semester1;
                unitAdd.semester2 = unit.semester2;
                unitAdd.type_code = "c";
                
                db.Entry(unitAdd).State = EntityState.Modified;
                
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

            return View(unit);
        }

        // GET: Units/Delete/5
        [Authorize]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = db.Units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        // POST: Units/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(string id)
        {
            Unit unit = db.Units.Find(id);
            db.Units.Remove(unit);

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
