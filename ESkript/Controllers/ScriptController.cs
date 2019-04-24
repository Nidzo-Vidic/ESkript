using ESkript.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;


namespace ESkript.Controllers {


    /// <summary>ScriptController
    /// Logik für Skripte
    /// </summary>
    [Authorize(Roles = "Admin,Dozent")]
    public class ScriptController : Controller {

        /// <summary>dbConnection
        /// Verbindung zur Datenbank
        /// </summary>
        private ESkriptModel dbConnection = new ESkriptModel();

        /// <summary>messageAutor
        /// Variable für Nachricht
        /// </summary>
        private string messageAutor = "Sie sind nicht der Autor dieses Skripts!";


        /// <summary>ScriptList
        /// Gibt eine Liste der Skripte aus die nicht bearbeitet werden
        /// </summary>
        /// <returns>View(Script Model (Liste))</returns>
        [OverrideAuthorization()]
        [Authorize]
        [HttpGet]
        public ActionResult ScriptList() {
            var script = dbConnection.Script.Where(i => i.InWork == false);
            return View(script.ToList());
        }


        /// <summary>ScriptList
        /// Gibt eine Liste der Skripte die dem jeweiligen Autor gehören und noch bearbeitet werden
        /// </summary>
        /// <returns>View(Script Model (Liste))</returns>
        [HttpGet]
        public ActionResult InWorkList() {
            var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
            var script = dbConnection.Script.Where(a => a.Author == user.IdAccount && a.InWork == true).ToList();
            return View(script);
        }

        /// <summary>ScriptList
        /// Gibt eine Liste aller Skripte aus 
        /// </summary>
        /// <returns>View(Script Model (Liste))</returns>
        [HttpGet]
        public ActionResult ScriptListGodMode() {
            if (User.IsInRole("Admin")) {
                var script = dbConnection.Script.ToList();
                return View(script);
            } else {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>Publish
        /// Lädt View zum Veröffentlichen eines Skripts
        /// </summary>
        /// <param name="id">Script ID (Integer)</param>
        /// <returns>View(Script Model)</returns>
        [HttpGet]
        public ActionResult Publish(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
            Script script = dbConnection.Script.Find(id);
            if (script.Author == user.IdAccount || User.IsInRole("Admin")) {
                if (script == null) {
                    return HttpNotFound();
                }
                TempData["IdScript"] = id;
                return View(script);
            } else {
                ViewBag.Msg = messageAutor;
                return RedirectToAction("Login", "Login");
            }
        }

        /// <summary>Publish
        /// Ändert den Wert InWork  zu false und dadurch wird das Skript öffentlich zugänglich
        /// </summary>
        /// <param name="script">Script Model</param>
        /// <returns>Index Methode, Home Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Publish(Script script) {
            int id = (int)TempData["IdScript"];
            script = dbConnection.Script.Where(i => i.IdScript == id).FirstOrDefault();
            script.InWork = false;
            dbConnection.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>Revoke
        /// Lädt View zum Sperren eines Skriptes 
        /// </summary>
        /// <param name="id">Script ID (Integer)</param>
        /// <returns>View(Script Model)</returns>
        [HttpGet]
        public ActionResult Revoke(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Script script = dbConnection.Script.Find(id);
            var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
            if (script.Author == user.IdAccount || User.IsInRole("Admin")) {
                if (script == null) {
                    return HttpNotFound();
                }
                TempData["IdScript"] = id;
                return View(script);
            } else {
                ViewBag.Msg = messageAutor;
                return RedirectToAction("Login", "Login");
            }

        }

        /// <summary>Revoke
        /// Öndert den Wert InWork zu true und sperrt dadurch das Skript
        /// </summary>
        /// <param name="id">Script Model</param>
        /// <returns>Index Methode, Home Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Revoke(Script script) {
            int id = (int)TempData["IdScript"];
            script = dbConnection.Script.Where(i => i.IdScript == id).FirstOrDefault();
            script.InWork = true;
            dbConnection.SaveChanges();
            return RedirectToAction("Index", "Home");
        }


        /// <summary>Create
        /// Lädt View zum Erstellen eines Skriptes
        /// </summary>
        /// <returns>View()</returns>
        [Authorize(Roles = "Admin,Dozent")]
        public ActionResult Create() {
            return View();
        }

        /// <summary>Create
        /// Erstellt ein Skript und speichert es in Datenbank
        /// </summary>
        /// <param name="script">Script model</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdScript,Subject,Author,CreationDate")] Script script) {
            if (ModelState.IsValid) {
                var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
                script.Author = user.IdAccount;
                script.InWork = true;
                script.CreationDate = DateTime.Now;
                var twin = dbConnection.Script.Where(s => s.Subject == script.Subject).FirstOrDefault();
                if (twin != null) {
                    ViewBag.Msg = "Ein Skript mit diesem Namen existiert schon";
                    return View(script);
                }
                dbConnection.Script.Add(script);
                dbConnection.SaveChanges();
                return RedirectToAction("ReadEditMode", "ScriptContent", new { id = script.IdScript });
            }
            return View(script);
        }

        /// <summary>Edit
        /// Lädt View zum Verändern
        /// </summary>
        /// <param name="id">Script ID (Integer)</param>
        /// <returns>View(Script Model)</returns>
        [HttpGet]
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Script script = dbConnection.Script.Find(id);
            if (script == null) {
                return HttpNotFound();
            }
            ViewBag.Id = script.IdScript;
            TempData["Id"] = script.IdScript;
            var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
            if (script.Author == user.IdAccount || User.IsInRole("Admin")) {
                ViewBag.Author = new SelectList(dbConnection.Account.Where(r => r.Role.IdRole <= 2), "IdAccount", "EMail", script.Author);
                return View(script);
            } else {
                ViewBag.Msg = messageAutor;
                return RedirectToAction("Login", "Login");
            }
        }

        /// <summary>Edit
        /// Speichert Änderung
        /// </summary>
        /// <param name="script">Script Model</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdScript,Subject,Author,CreationDate")] Script script) {
            if (ModelState.IsValid) {
                var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
                dbConnection.Entry(script).State = EntityState.Modified;
                script.InWork = true;
                dbConnection.SaveChanges();
                return RedirectToAction("ReadEditMode", "ScriptContent", new { id = script.IdScript });
            }
            ViewBag.Author = new SelectList(dbConnection.Account.Where(r => r.Role.IdRole <= 2), "IdAccount", "EMail", script.Author);
            ViewBag.Id = TempData["Id"];
            return View(script);
        }

        /// <summary>Delete
        /// Lädt View zum Löschen eines Skriptes
        /// </summary>
        /// <param name="id">Script ID (Integer)</param>
        /// <returns>View(Script Model)</returns>
        [HttpGet]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
            Script script = dbConnection.Script.Find(id);
            if (script == null) {
                return HttpNotFound();
            }

            if (script.Author == user.IdAccount || User.IsInRole("Admin")) {
                return View(script);
            } else {
                ViewBag.Msg = messageAutor;
                return RedirectToAction("Login", "Login");
            }
        }


        /// <summary>DeleteConfirmed
        /// Löscht Skript, samt Attributen aus Datenbank
        /// </summary>
        /// <param name="id">Script ID (Integer)</param>
        /// <returns>Index Methode, Home Controller</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            Script script = dbConnection.Script.Find(id);
            
            IEnumerable<ContentAttribute> contentAttribute = dbConnection.ContentAttribute.Where(s => s.Script == script.IdScript).ToList();
            foreach (var item in contentAttribute) {
                dbConnection.ContentAttribute.Remove(item);
            }
            if (script.ScriptContent != null) {
                var content = dbConnection.ScriptContent.Where(s => s.Script == id).OrderBy(s => s.FatherContent).ThenBy(s => s.Predecessor).ToList();
                foreach (var o in content) {
                    if (o.TextCard.Count() > 0) {
                        IEnumerable<TextCard> text = dbConnection.TextCard.Where(t => t.TextContent == o.IdScriptContent).ToList();
                        foreach (var item in text) {
                            dbConnection.TextCard.Remove(item);
                        }
                    } else if (o.FormulaCard.Count() > 0) {
                        IEnumerable<FormulaCard> formula = dbConnection.FormulaCard.Where(f => f.FormulaContent == o.IdScriptContent).ToList();
                        foreach (var item in formula) {
                            dbConnection.FormulaCard.Remove(item);
                        }
                    } else if (o.ImageCard.Count() > 0) {
                        IEnumerable<ImageCard> image = dbConnection.ImageCard.Where(i => i.ImageContent == o.IdScriptContent).ToList();
                        foreach (var item in image) {
                            dbConnection.ImageCard.Remove(item);
                        }
                    } else if (o.VideoCard.Count() > 0) {
                        IEnumerable<VideoCard> video = dbConnection.VideoCard.Where(v => v.VideoContent == o.IdScriptContent).ToList();
                        foreach (var item in video) {
                            dbConnection.VideoCard.Remove(item);
                        }
                    } else if (o.LinkCard.Count() > 0) {
                        IEnumerable<LinkCard> link = dbConnection.LinkCard.Where(l => l.LinkContent == o.IdScriptContent).ToList();
                        foreach (var item in link) {
                            dbConnection.LinkCard.Remove(item);
                        }
                    } else if (o.LiteratureCard.Count() > 0) {
                        IEnumerable<LiteratureCard> literature = dbConnection.LiteratureCard.Where(l => l.LiteratureContent == o.IdScriptContent).ToList();
                        foreach (var item in literature) {
                            dbConnection.LiteratureCard.Remove(item);
                        }
                    }

                    if (o.CommentCard.Count() > 0) {
                        IEnumerable<CommentCard> comment = dbConnection.CommentCard.Where(s => s.ScriptContent == o.IdScriptContent).ToList();
                        foreach (var item in comment) {
                            dbConnection.CommentCard.Remove(item);
                        }
                    }

                    if (o.ContentAttributeList.Count() > 0) {
                        IEnumerable<ContentAttributeList> attributeList = dbConnection.ContentAttributeList.Where(s => s.ScriptContent == o.IdScriptContent).ToList();
                        foreach (var item in attributeList) {
                            dbConnection.ContentAttributeList.Remove(item);
                        }
                    }

                    dbConnection.ScriptContent.Remove(o);
                }
            }
            dbConnection.Script.Remove(script);
            dbConnection.SaveChanges();
            return RedirectToAction("Index","Home");
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
