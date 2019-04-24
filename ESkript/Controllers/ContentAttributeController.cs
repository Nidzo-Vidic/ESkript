using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ESkript.Models;



namespace ESkript.Controllers {

    /// <summary>ContentAttributeController
    /// Logik für die Attribute für Skripte
    /// </summary>
    [Authorize(Roles = "Admin,Dozent")]
    public class ContentAttributeController : Controller {

        /// <summary>dbConnection
        /// Verbindung zur Datenbank
        /// </summary>
        private ESkriptModel dbConnection = new ESkriptModel();

        /// <summary>Index
        /// Gibt eine Liste aller globalen und aller zum Skript zugehörigen Attribute aus
        /// </summary>
        /// <param name="id">Script ID (Integer)</param>
        /// <returns>View(Attribut Model (Liste))</returns>
        [HttpGet]
        public ActionResult Index(int? id) {
            var script = dbConnection.Script.Find(id);
            var attributes = dbConnection.ContentAttribute.Where(s => s.Script == null || s.Script == id).ToList();
            ViewBag.Id = script.IdScript;
            ViewBag.ScriptName = script.Subject;
            var scriptId = script.IdScript;
            TempData["IdScript"] = scriptId;
            TempData["IdScript2"] = scriptId;
            return View(attributes);
        }

        /// <summary>Create
        /// Ruft eine View zum erstellen eines Attributs für ein jeweiliges Skript auf
        /// </summary>
        /// <param name="id">Script ID (Integer)</param>
        /// <returns>View()</returns>
        [HttpGet]
        public ActionResult Create(int? id) {
            var script = dbConnection.Script.Find(id);
            ViewBag.Id = id;
            ViewBag.ScriptName = script.Subject;
            TempData["Id"] = id;
            TempData["Scriptname"] = script.Subject;
            return View();
        }

        /// <summary>Create
        /// Speichert das neue Attribut in Datenbank
        /// </summary>
        /// <param name="contentAttribute">ContentAttribut Model</param>
        /// <returns>Index(Script ID (Integer))</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdAttribute,AttributeName,Script")] ContentAttribute contentAttribute) {
            if (ModelState.IsValid) {
                contentAttribute.Script = (int)TempData["IdScript2"];
                var twin = dbConnection.ContentAttribute.Where(a => a.AttributeName == contentAttribute.AttributeName).FirstOrDefault();
                if (twin != null) {
                    ViewBag.Msg = "Ein Attribut mit diesen Namen existiert schon!";
                    TempData["IdScript2"] = twin.Script;
                    ViewBag.Id = twin.Script;
                    return View(contentAttribute);
                }
                dbConnection.ContentAttribute.Add(contentAttribute);
                dbConnection.SaveChanges();
                return RedirectToAction("Index", new { id = contentAttribute.Script });
            }
            ViewBag.Id = TempData["Id"];
            ViewBag.ScriptName = TempData["Scriptname"];
            return View(contentAttribute);
        }

        /// <summary>CreateGlobal
        /// Ruft eine View auf um ein globales Attribut zu erstellen
        /// </summary>
        /// <param name="id">Script ID (Integer)</param>
        /// <returns>View()</returns>
        [OverrideAuthorization]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateGlobal(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Id = id;
            TempData["ScriptId"] = id;
            TempData["Id"] = id;
            return View();
        }

        /// <summary>CreateGlobal
        /// Speichert eine globales Attribut in der Datenbank
        /// </summary>
        /// <param name="contentAttribute">ContentAttribut Model</param>
        /// <returns>Index(Script ID (Integer))</returns>
        [OverrideAuthorization]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGlobal([Bind(Include = "IdAttribute,AttributeName,Script")] ContentAttribute contentAttribute) {
            if (ModelState.IsValid) {
                int id = (int)TempData["ScriptId"];
                var twin = dbConnection.ContentAttribute.Where(a => a.AttributeName == contentAttribute.AttributeName).FirstOrDefault();
                if (twin != null) {
                    ViewBag.Msg = "Ein Attribut mit diesen Namen existiert schon!";
                    ViewBag.Id = id;
                    return View("CreateGlobal",contentAttribute);
                }
                ViewBag.Id = id;
                dbConnection.ContentAttribute.Add(contentAttribute);
                dbConnection.SaveChanges();
                return RedirectToAction("Index", new { id = id});
            }
            ViewBag.Id = TempData["Id"];
            return View(contentAttribute);
        }

        /// <summary>Edit
        /// Ruft View zum bearbeiten eines Attributs
        /// </summary>
        /// <param name="id">ContentAttribute ID (Integer)</param>
        /// <returns>View(ContentAttribute Model))</returns>
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentAttribute contentAttribute = dbConnection.ContentAttribute.Find(id);
            if (contentAttribute == null) {
                return HttpNotFound();
            }
            TempData["ScriptId"] = contentAttribute.Script;
            if (contentAttribute.Script != null) {
                ViewBag.Id = contentAttribute.Script;
            } else {
                var scriptId = TempData["IdScript"];
                ViewBag.Id = scriptId;
                TempData["Id"] = scriptId;
            }
            return View(contentAttribute);
        }

        /// <summary>Edit
        /// Speicher Veränderungen in der Datenbank
        /// </summary>
        /// <param name="contentAttribute">ContentAttribute Model</param>
        /// <returns>Index(Script ID (Integer))</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdAttribute,AttributeName,Script")] ContentAttribute contentAttribute) {
            if (ModelState.IsValid) {
                var script = dbConnection.Script.Find(contentAttribute.Script);
                dbConnection.Entry(contentAttribute).State = EntityState.Modified;
                dbConnection.SaveChanges();
                if (script != null) {
                    return RedirectToAction("Index", new { id = script.IdScript });
                } else {
                    return RedirectToAction("Index", new { id = TempData["Id"] });
                }
            }
            ViewBag.Id = TempData["ScriptId"];
            return View(contentAttribute);
        }

        /// <summary>Delete
        /// View zum Löschen
        /// </summary>
        /// <param name="id">ContentAttribute ID (Integer)</param>
        /// <returns>View(ContentAttribute Model)</returns>
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentAttribute contentAttribute = dbConnection.ContentAttribute.Find(id);
            if (contentAttribute == null) {
                return HttpNotFound();
            }
            if (contentAttribute.Script != null) {
                ViewBag.Id = contentAttribute.Script;
            } else {
                var scriptId = TempData["IdScript"];
                ViewBag.Id = scriptId;
                TempData["Id"] = scriptId;
            }
            return View(contentAttribute);
        }

        /// <summary>Delete
        /// Löscht Attribut aus Datenbank
        /// </summary>
        /// <param name="id">ContentAttribute ID (Integer)</param>
        /// <returns>Index(Script ID (Integer))</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            ContentAttribute contentAttribute = dbConnection.ContentAttribute.Find(id);
            var script = dbConnection.Script.Find(contentAttribute.Script);
            dbConnection.ContentAttribute.Remove(contentAttribute);
            dbConnection.SaveChanges();
            if (script != null) {
                return RedirectToAction("Index", new { id = script.IdScript });
            } else {
                return RedirectToAction("Index", new { id = TempData["Id"] });
            }
        }


        /// <summary>
        /// Gibt nicht verwaltete Ressourcen und optional verwaltete Ressourcen frei
        /// </summary>
        /// <param name="disposing">Bool</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                dbConnection.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
