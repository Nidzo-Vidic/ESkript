using ESkript.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ESkript.Controllers {

    /// <summary>LiteratureCardController
    /// Logik für Literaturverweise
    /// </summary>
    public class LiteratureCardController : Controller {

        /// <summary>dbConnection
        /// Verbindung zur Datenbank
        /// </summary>
        private ESkriptModel dbConnection = new ESkriptModel();


        /// <summary>Create
        /// Lädt View zum Erstellen eines Literaturverweises
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
            ViewBag.Titel = content.Titel;
            ViewBag.Id = content.Script;
            TempData["Id"] = content.Script;
            TempData["IdContent"] = content.IdScriptContent;
            ViewBag.LiteratureContent = new SelectList(dbConnection.ScriptContent, "IdScriptContent", "Titel");
            return View();
        }


        /// <summary>Create
        /// Speichert Literaturverweis in Datenbank
        /// </summary>
        /// <param name="literatureCard">LiteratureCard Model</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdLiterature,LiteratureContent,Authors,Titel,Publisher,Year,Location,Edition")] LiteratureCard literatureCard) {
            if (ModelState.IsValid) {
                literatureCard.LiteratureContent = (int)TempData["IdContent"];
                var content = dbConnection.ScriptContent.Find(TempData["IdContent"]);
                dbConnection.LiteratureCard.Add(literatureCard);
                dbConnection.SaveChanges();
                return RedirectToAction("ReadEditMode", "ScriptContent", new { id = content.Script });
            }
            ViewBag.Id = TempData["Id"];
            ViewBag.LiteratureContent = new SelectList(dbConnection.ScriptContent, "IdScriptContent", "Titel");
            return View(literatureCard);
        }


        /// <summary>Edit
        /// Lädt View zum Ändern eines Literaturverweises
        /// </summary>
        /// <param name="id">LiteratureCard ID (Integer)</param>
        /// <returns>View(LiteratureCard Model)</returns>
        [HttpGet]
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiteratureCard literatureCard = dbConnection.LiteratureCard.Find(id);
            if (literatureCard == null) {
                return HttpNotFound();
            }
            TempData["IdScript"] = literatureCard.ScriptContent.Script;
            ViewBag.Id = literatureCard.ScriptContent.Script;
            ViewBag.LiteratureContent = new SelectList(dbConnection.ScriptContent, "IdScriptContent", "Titel", literatureCard.LiteratureContent);
            return View(literatureCard);
        }

        /// <summary>Edit
        /// Speichert Änderung des Literaturverweises
        /// </summary>
        /// <param name="literatureCard">LiteratureCard Model</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdLiterature,LiteratureContent,Authors,Titel,Publisher,Year,Location,Edition")] LiteratureCard literatureCard) {
            if (ModelState.IsValid) {
                dbConnection.Entry(literatureCard).State = EntityState.Modified;
                dbConnection.SaveChanges();
                return RedirectToAction("ReadEditMode", "ScriptContent", new { id = TempData["IdScript"] });
            }
            ViewBag.Id = TempData["IdScript"];
            ViewBag.LiteratureContent = new SelectList(dbConnection.ScriptContent, "IdScriptContent", "Titel", literatureCard.LiteratureContent);
            return View(literatureCard);
        }

        /// <summary>Delete
        /// Lädt View zum Löschen eines Literaturverweises
        /// </summary>
        /// <param name="id">LiteratureCard ID (Integer)</param>
        /// <returns>View(LiteratureCard Model)</returns>
        [HttpGet]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiteratureCard literatureCard = dbConnection.LiteratureCard.Find(id);
            if (literatureCard == null) {
                return HttpNotFound();
            }
            return View(literatureCard);
        }


        /// <summary>DeleteConfirmed
        /// Löscht Literaturverweis aus Datenban
        /// </summary>
        /// <param name="id">LiteratureCard ID (Integer)</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            LiteratureCard literatureCard = dbConnection.LiteratureCard.Find(id);
            int contentId = (int)literatureCard.ScriptContent.Script;
            dbConnection.LiteratureCard.Remove(literatureCard);
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
