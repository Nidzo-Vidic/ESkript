using ESkript.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ESkript.Controllers {

    /// <summary>TextCardController
    /// Text Inhalt Logik
    /// </summary>
    [Authorize(Roles = "Admin,Dozent")]
    public class TextCardController : Controller {

        /// <summary>dbConnection
        /// Verbindung zur Datenbank
        /// </summary>
        private ESkriptModel dbConnection = new ESkriptModel();


        /// <summary>CreateText
        /// Lädt View zum erstellen eines Textes
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>View()</returns>
        [HttpGet]
        public ActionResult CreateText(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempData["IdContent"] = id;
            var content = dbConnection.ScriptContent.Where(i => i.IdScriptContent == id).FirstOrDefault();
            if (content == null) {
                return HttpNotFound();
            }
            ViewBag.Titel = content.Titel;
            ViewBag.Id = content.Script;
            return View();
        }

        /// <summary>CreateText
        /// Speichert Text in der Datenbank
        /// </summary>
        /// <param name="textCard">TextCard Model</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateText([Bind(Include = "IdTextCard,TextContent,Text")] TextCard textCard) {
            if (ModelState.IsValid) {
                int id = (int)TempData["IdContent"];
                textCard.TextContent = id;
                dbConnection.TextCard.Add(textCard);
                dbConnection.SaveChanges();
                var c = dbConnection.ScriptContent.Where(i => i.IdScriptContent == id).FirstOrDefault();
                return RedirectToAction("ReadEditMode", "ScriptContent", new { id = c.Script });
            }
            return View(textCard);
        }

        /// <summary>Edit
        /// Lädt View zum Ändern eines Textes
        /// </summary>
        /// <param name="id">TextCard ID (Integer)</param>
        /// <returns>View(TextCard Model)</returns>
        [HttpGet]
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextCard textCard = dbConnection.TextCard.Find(id);
            TempData["IdContent"] = textCard.ScriptContent.Script;
            if (textCard == null) {
                return HttpNotFound();
            }
            ViewBag.TextContent = new SelectList(dbConnection.ScriptContent, "IdScriptContent", "Titel", textCard.TextContent);
            return View(textCard);
        }

        /// <summary>Edit
        /// Speichert Änderung des Textes
        /// </summary>
        /// <param name="textCard">TextCard Model</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdTextCard,TextContent,Text")] TextCard textCard) {
            if (ModelState.IsValid) {
                dbConnection.Entry(textCard).State = EntityState.Modified;
                dbConnection.SaveChanges();
                int i = (int)TempData["IdContent"];
                return RedirectToAction("ReadEditMode", "ScriptContent", new { id = i });
            }
            return View(textCard);
        }

        /// <summary>Delete
        /// Lädt View zum Löschen eines Textes
        /// </summary>
        /// <param name="id">TextCard ID (Integer)</param>
        /// <returns>View(TextCard Model)</returns>
        [HttpGet]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextCard textCard = dbConnection.TextCard.Find(id);
            if (textCard == null) {
                return HttpNotFound();
            }
            return View(textCard);
        }

        /// <summary>DeleteConfirmed
        /// Löscht Text aus Datenbank
        /// </summary>
        /// <param name="id">TextCard ID (Integer)</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            TextCard textCard = dbConnection.TextCard.Find(id);
            int i = (int)textCard.ScriptContent.Script;
            dbConnection.TextCard.Remove(textCard);
            dbConnection.SaveChanges();
            return RedirectToAction("ReadEditMode", "ScriptContent", new { id = i }); ;
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
