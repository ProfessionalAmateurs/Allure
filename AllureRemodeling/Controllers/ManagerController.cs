using AllureRemodeling.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AllureRemodeling.Controllers
{
    public class ManagerController : Controller
    {
        // GET: Manager
        public ActionResult Index()
        {
            DatabaseClass db = new DatabaseClass();
            return View(db.GetMaterialDetails());
        }

        // GET: Manager/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Manager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Manager/Create
        [HttpPost]
        public ActionResult Create(Materials material)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DatabaseClass db = new DatabaseClass();
                    if (db.AddMaterial(material))
                    {
                        ViewBag.Message = "Material Details Added Successfully";
                        ModelState.Clear();
                    }
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Manager/Edit/5
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DatabaseClass db = new DatabaseClass();
            Materials details = db.GetMaterialDetails().Find(Materials => Materials.MaterialID == Id);
            if (details == null)
            {
                return HttpNotFound();
            }
            return View(details);

        }

        // POST: Manager/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaterialId,Description,Price")] Materials material)
        {
            if (ModelState.IsValid)
            {
                DatabaseClass db = new DatabaseClass();
                db.UpdateMaterialDetails(material);
                return RedirectToAction("Index");
            }
            return View(material);
        }
    


        // GET: Manager/Delete/5
        public ActionResult Delete(int materialId)
        {
            DatabaseClass db = new DatabaseClass();
            if (db.DeleteMaterial(materialId))
            {
                ViewBag.AlertMsg = "Material Deleted Successfully";
            }
            return RedirectToAction("Index");
        }

        // POST: Manager/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
