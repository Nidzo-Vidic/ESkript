using ESkript.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ESkript.Controllers {

    /// <summary>ContentAttributeListController
    /// Logik der Attribute für ScriptContents
    /// </summary>
    public class ContentAttributeListController : Controller {

        /// <summary>dbConnection
        /// Verbindung zur Datenbank
        /// </summary>
        private ESkriptModel db = new ESkriptModel();


        /// <summary>Create
        /// Ruft View auf um Attribut zu einem ScriptContent hinzuzufügen
        /// </summary>
        /// <param name="id">Script ID (Integer)</param>
        /// <returns>View()</returns>
        public ActionResult Create(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var content = db.ScriptContent.Find(id);
            if (content == null) {
                return HttpNotFound();
            }
            ViewBag.Attribute = new SelectList(db.ContentAttribute.Where(s => s.Script == content.Script || s.Script == null), "IdAttribute", "AttributeName");
            TempData["IdContent"] = id;
            ViewBag.Id = id;

            return View();
        }

        
        /// <summary>Create
        /// Speichert Attribut für ScriptContent
        /// </summary>
        /// <param name="contentAttributeList">ContentAttributeList Model</param>
        /// <returns>Edit(ScriptContent ID (Integer)) Methode, ScriptContent Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdContentAttributeList,Attribute,ScriptContent")] ContentAttributeList contentAttributeList) {
            if (ModelState.IsValid) {
                int idContent = (int) TempData["IdContent"];
                contentAttributeList.ScriptContent = idContent;
                db.ContentAttributeList.Add(contentAttributeList);
                db.SaveChanges();
                return RedirectToAction("Edit", "ScriptContent", new { id = idContent});
            }
            return View(contentAttributeList);
        }

        /// <summary>Edit
        /// View zum ändern des Attributs
        /// </summary>
        /// <param name="id">ContentAttributeList ID (Integer)</param>
        /// <returns>View(ContentAttribute Model)</returns>
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentAttributeList contentAttributeList = db.ContentAttributeList.Find(id);
            if (contentAttributeList == null) {
                return HttpNotFound();
            }
            ViewBag.Attribute = new SelectList(db.ContentAttribute, "IdAttribute", "AttributeName", contentAttributeList.Attribute);
            ViewBag.ScriptContent = new SelectList(db.ScriptContent, "IdScriptContent", "Titel", contentAttributeList.ScriptContent);
            return View(contentAttributeList);
        }

        /// <summary>Edit
        /// Speichert die Änderung des Attributs
        /// </summary>
        /// <param name="contentAttributeList">ContentAttributeList</param>
        /// <returns>Edit(ScriptContent ID (Integer)) Methode, ScriptContent Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdContentAttributeList,Attribute,ScriptContent")] ContentAttributeList contentAttributeList) {
            if (ModelState.IsValid) {
                db.Entry(contentAttributeList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", "ScriptContent", new { id = contentAttributeList.ScriptContent});
            }
            return View(contentAttributeList);
        }

        
        /// <summary>Delete
        /// Entfernt Attribut vom ScriptContent
        /// </summary>
        /// <param name="id">ContentAttributeList ID (Integer)</param>
        /// <returns>Edit(ScriptContent ID (Integer)) Methode, ScriptContent Controller</returns>
        public ActionResult Delete(int id) {
            ContentAttributeList contentAttributeList = db.ContentAttributeList.Find(id);
            int contentId = (int)contentAttributeList.ScriptContent;
            db.ContentAttributeList.Remove(contentAttributeList);
            db.SaveChanges();
            return RedirectToAction("Edit", "ScriptContent", new { id = contentId});
        }


        /// <summary>
        /// Gibt nicht verwaltete Ressourcen und optional verwaltete Ressourcen frei
        /// </summary>
        /// <param name="disposing">Bool</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
