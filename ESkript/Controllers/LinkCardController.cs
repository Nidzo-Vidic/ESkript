using ESkript.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ESkript.Controllers {

    /// <summary>LinkCardController
    /// Logik für Links
    /// </summary>
    public class LinkCardController : Controller {

        /// <summary>dbConnection
        /// Verbindung zur Datenbank
        /// </summary>
        private ESkriptModel dbConnection = new ESkriptModel();

        /// <summary>Create
        /// Lädt View zum Erstellen eines Links
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>View()</returns>
        [HttpGet]
        public ActionResult Create(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var content = dbConnection.ScriptContent.Find(id);
            if (content == null) {
                return HttpNotFound();
            }
            ViewBag.Id = content.Script;
            TempData["IdContent"] = id;
            TempData["Id"] = content.Script;
            return View();
        }

        /// <summary>Create
        /// Speichert Link in Datenbank
        /// </summary>
        /// <param name="linkCard">LinkCard Model</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdLink,LinkContent,Link")] LinkCard linkCard) {
            if (ModelState.IsValid) {
                linkCard.LinkContent = (int)TempData["IdContent"];
                var content = dbConnection.ScriptContent.Find(TempData["IdContent"]);
                dbConnection.LinkCard.Add(linkCard);
                dbConnection.SaveChanges();
                return RedirectToAction("ReadEditMode", "ScriptContent", new { id = content.Script});
            }
            ViewBag.Id = TempData["Id"];
            return View(linkCard);
        }

        /// <summary>Edit
        /// Lädt View zum Verändern
        /// </summary>
        /// <param name="id">LinkCard ID (Integer)</param>
        /// <returns>View(LinkCard Model)</returns>
        [HttpGet]
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LinkCard linkCard = dbConnection.LinkCard.Find(id);
            TempData["IdScript"] = linkCard.ScriptContent.Script;
            if (linkCard == null) {
                return HttpNotFound();
            }
            ViewBag.LinkContent = new SelectList(dbConnection.ScriptContent, "IdScriptContent", "Titel", linkCard.LinkContent);
            return View(linkCard);
        }

        /// <summary>Edit
        /// Speichert Änderung des Links
        /// </summary>
        /// <param name="linkCard">LinkCard Model</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdLink,LinkContent,Link")] LinkCard linkCard) {
            if (ModelState.IsValid) {
                dbConnection.Entry(linkCard).State = EntityState.Modified;
                dbConnection.SaveChanges();
                return RedirectToAction("ReadEditMode", "ScriptContent", new { id = TempData["IdScript"] });
            }
            ViewBag.LinkContent = new SelectList(dbConnection.ScriptContent, "IdScriptContent", "Titel", linkCard.LinkContent);
            ViewBag.Id = TempData["IdScript"];
            return View(linkCard);
        }

        /// <summary>Delete
        /// Lädt View zum Löschen von Link
        /// </summary>
        /// <param name="id">LinkCard ID (Integer)</param>
        /// <returns>View(LinkCard Model)</returns>
        [HttpGet]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LinkCard linkCard = dbConnection.LinkCard.Find(id);
            if (linkCard == null) {
                return HttpNotFound();
            }
            return View(linkCard);
        }

        /// <summary>DelteConfirmed
        /// Löscht Link aus Datenbank
        /// </summary>
        /// <param name="id">LinkCard ID (Integer)</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            LinkCard linkCard = dbConnection.LinkCard.Find(id);
            int contentId = (int)linkCard.ScriptContent.Script;
            dbConnection.LinkCard.Remove(linkCard);
            dbConnection.SaveChanges();
            return RedirectToAction("ReadEditMode", "ScriptContent", new { id = contentId });
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
