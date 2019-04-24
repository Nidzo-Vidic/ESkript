using ESkript.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ESkript.Controllers {

    /// <summary>FormulaCardController
    /// Logik der Formel Inhalte für ScriptContents
    /// </summary>
    [Authorize(Roles = "Admin,Dozent")]
    public class FormulaCardController : Controller {

        /// <summary>dbConnection
        /// Verbindung zur Datenbank
        /// </summary>
        private ESkriptModel dbConnection = new ESkriptModel();

        /// <summary>CreateFormula
        /// View zum erstellen einer Formel
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>View()</returns>
        [HttpGet]
        public ActionResult CreateFormula(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempData["IdContent"] = id;
            var content = dbConnection.ScriptContent.Where(i => i.IdScriptContent == id).FirstOrDefault();
            if (content == null) {
                return HttpNotFound();
            }
            TempData["Id"] = content.Script;
            ViewBag.Titel = content.Titel;
            ViewBag.Id = content.Script;
            return View();
        }

        /// <summary>CreateFormula
        /// Speichert Formel in der Datenbank
        /// </summary>
        /// <param name="formulaCard">FormulaCard Model</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Methode</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFormula([Bind(Include = "IdFormulaCard,FormulaContent,FormulaName,Formula")] FormulaCard formulaCard) {
            if (ModelState.IsValid) {
                int id = (int)TempData["IdContent"];
                ViewBag.Id = id;
                formulaCard.FormulaContent = id;
                dbConnection.FormulaCard.Add(formulaCard);
                dbConnection.SaveChanges();
                var c = dbConnection.ScriptContent.Where(i => i.IdScriptContent == id).FirstOrDefault();
                return RedirectToAction("ReadEditMode", "ScriptContent", new { id = c.Script });
            }
            ViewBag.Id = TempData["Id"];
            return View(formulaCard);
        }

        /// <summary>Edit
        /// View zum Veränder der Formel
        /// </summary>
        /// <param name="id">FormulaCard ID (Integer)</param>
        /// <returns>View(FormulaCard Model)</returns>
        [HttpGet]
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormulaCard formulaCard = dbConnection.FormulaCard.Find(id);
            TempData["IdScript"] = formulaCard.ScriptContent.Script;
            ViewBag.Id = formulaCard.ScriptContent.Script;
            if (formulaCard == null) {
                return HttpNotFound();
            }
            ViewBag.FormulaContent = new SelectList(dbConnection.ScriptContent, "IdScriptContent", "Titel", formulaCard.FormulaContent);
            return View(formulaCard);
        }

        /// <summary>Edit
        /// Speichert Änderung in der Datenbank
        /// </summary>
        /// <param name="formulaCard">FormulaCard Model</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Methode</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdFormulaCard,FormulaContent,Formula")] FormulaCard formulaCard) {
            if (ModelState.IsValid) {
                dbConnection.Entry(formulaCard).State = EntityState.Modified;
                dbConnection.SaveChanges();
                int i = (int)TempData["IdScript"];
                return RedirectToAction("ReadEditMode", "ScriptContent", new { id = i });
            }
            ViewBag.Id = TempData["IdScript"];
            return View(formulaCard);
        }

        /// <summary>Delete
        /// View zum Löschen
        /// </summary>
        /// <param name="id">FormulaCard ID (Integer)</param>
        /// <returns>View(FormulaCard Model)</returns>
        [HttpGet]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FormulaCard formulaCard = dbConnection.FormulaCard.Find(id);
            if (formulaCard == null) {
                return HttpNotFound();
            }
            return View(formulaCard);
        }

        /// <summary>Delete
        /// Löscht Formel aus Datenbank
        /// </summary>
        /// <param name="id">FormulaCard ID (Integer)</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Methode</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            FormulaCard formulaCard = dbConnection.FormulaCard.Find(id);
            int i = (int)formulaCard.ScriptContent.Script;
            dbConnection.FormulaCard.Remove(formulaCard);
            dbConnection.SaveChanges();
            return RedirectToAction("ReadEditMode", "ScriptContent", new { id = i });
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
