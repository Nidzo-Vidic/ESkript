using ESkript.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ESkript.Controllers {


    /// <summary>CommentCardController
    /// Logik für Kommentare
    /// </summary>
    [Authorize]
    public class CommentCardController : Controller {

        /// <summary>dbConnection
        /// Verbindung zur Datenbank
        /// </summary>
        private ESkriptModel dbConnection = new ESkriptModel();

        /// <summary>Create
        /// Lädt View um einen Kommentar zu erstellen
        /// </summary>
        /// <returns>View()</returns>
        public ActionResult Create(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var content = dbConnection.ScriptContent.Find(id);
            if (content == null) {
                return HttpNotFound();
            }
            TempData["IdContent"] = content.IdScriptContent;
            TempData["IdScript"] = content.Script;
            ViewBag.Id = content.Script;
            return View();
        }


        /// <summary>Create
        /// Erstellt und Speichert Kommentar in der Datenbank
        /// </summary>
        /// <param name="commentCard">CommentCard Model</param>
        /// <returns>ReadEditMode(Script ID (Integer))</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdComment,ScriptContent,Author,CreationDate,Text")] CommentCard commentCard) {
            if (ModelState.IsValid) {
                var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
                commentCard.Author = user.IdAccount;
                commentCard.CreationDate = DateTime.Now;
                commentCard.ScriptContent = (int)TempData["IdContent"];
                dbConnection.CommentCard.Add(commentCard);
                dbConnection.SaveChanges();
                return RedirectToAction("ReadEditMode", "ScriptContent", new { id = TempData["IdScript"] });
            }
            ViewBag.Id = TempData["IdScript"];
            ViewBag.Author = new SelectList(dbConnection.Account, "IdAccount", "FirstName", commentCard.Author);
            ViewBag.ScriptContent = new SelectList(dbConnection.ScriptContent, "IdScriptContent", "Titel", commentCard.ScriptContent);
            return View(commentCard);
        }


        /// <summary>Edit
        /// Ruft View zum Bearbeiten eines Kommentars
        /// </summary>
        /// <param name="id">CommentCard ID (Integer)</param>
        /// <returns>View(CommentCard Model)</returns>
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentCard commentCard = dbConnection.CommentCard.Find(id);
            if (commentCard == null) {
                return HttpNotFound();
            }
            ViewBag.Id = commentCard.ScriptContent1.Script;
            TempData["IdScript"] = commentCard.ScriptContent1.Script;
            ViewBag.Author = new SelectList(dbConnection.Account, "IdAccount", "FirstName", commentCard.Author);
            ViewBag.ScriptContent = new SelectList(dbConnection.ScriptContent, "IdScriptContent", "Titel", commentCard.ScriptContent);
            if (User.IsInRole("Admin") || User.IsInRole("Admin") || User.IsInRole("Dozent") || commentCard.Account.EMail != User.Identity.Name) {
                return View(commentCard);
            } else {
                ViewBag.Msg = "Sie Sind nicht der Autor des Kommentars oder Sie besitzen nicht die nötigen Rechte!";
                return RedirectToAction("Login", "Login");
            }
        }

        /// <summary>Edit
        /// Speichert die Änderungen in der Datenbank
        /// </summary>
        /// <param name="commentCard">CommentCard Model</param>
        /// <returns>Read(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdComment,ScriptContent,Author,CreationDate,Text")] CommentCard commentCard) {
            if (ModelState.IsValid) {
                dbConnection.Entry(commentCard).State = EntityState.Modified;
                dbConnection.SaveChanges();
                return RedirectToAction("Read", "ScriptContent", new { id = TempData["IdScript"] });
            }
            ViewBag.Author = new SelectList(dbConnection.Account, "IdAccount", "FirstName", commentCard.Author);
            ViewBag.ScriptContent = new SelectList(dbConnection.ScriptContent, "IdScriptContent", "Titel", commentCard.ScriptContent);
            ViewBag.Id = TempData["IdScript"];
            return View(commentCard);
        }

        /// <summary>Delete
        /// Ruft View zum Löschen eines Kommtars auf
        /// </summary>
        /// <param name="id">CommentCard ID (Integer)</param>
        /// <returns>View(CommentCard Model)</returns>
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentCard commentCard = dbConnection.CommentCard.Find(id);
            if (commentCard == null) {
                return HttpNotFound();
            }


            if (User.IsInRole("Admin") || User.IsInRole("Admin") || User.IsInRole("Dozent") || commentCard.Account.EMail != User.Identity.Name) {
                return View(commentCard);
            } else {
                ViewBag.Msg = "Sie Sind nicht der Autor des Kommentars oder Sie besitzen nicht die nötigen Rechte!";
                return RedirectToAction("Login", "Login");
            }
        }

        /// <summary>Delete
        /// Löscht einen Kommentar aus der Datenbakn
        /// </summary>
        /// <param name="id">CommentCard ID (Integer)</param>
        /// <returns>Read(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            CommentCard commentCard = dbConnection.CommentCard.Find(id);
            int? IdScript = commentCard.ScriptContent1.Script;
            dbConnection.CommentCard.Remove(commentCard);
            dbConnection.SaveChanges();
            return RedirectToAction("Read", "ScriptContent", new { id = IdScript});
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
