using ESkript.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ESkript.Controllers {

    /// <summary>ScriptContentController
    /// Schnittstelle um Inhalte zum Skript hinzuzufügen
    /// </summary>
    [Authorize(Roles = "Admin,Dozent")]
    public class ScriptContentController : Controller {

        /// <summary>dbConnection
        /// Verbindung zur Datenbank
        /// </summary>
        private ESkriptModel dbConnection = new ESkriptModel();

        /// <summary>messageAutor
        /// Variable für Nachricht
        /// </summary>
        private string messageAutor = "Sie sind nicht der Autor dieses Skripts!";

        /// <summary>Create
        /// Gibt die View aus um ein ScriptContent zu erstellen, welches kein FatherContent enthält
        /// </summary>
        /// <param name="id">Script ID(Integer)</param>
        /// <returns>View()</returns>
        [HttpGet]
        public ActionResult Create(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var script = dbConnection.Script.Where(i => i.IdScript == id).FirstOrDefault();
            var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
            if (script.Author == user.IdAccount || User.IsInRole("Admin")) {
                TempData["IdScript"] = id;
                ViewBag.IdScript = id;
                return View();
            } else {
                ViewBag.Msg = messageAutor;
                return RedirectToAction("Login", "Login");
            }
        }

        /// <summary>Create
        /// Methode um ein ScriptContent zu erstellen und in der Datenbank zu speichern
        /// </summary>
        /// <param name="content">ScriptContent Model</param>
        /// <returns>ReadEditMode(Script ID (Integer))</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ScriptContent content) {

            if (ModelState.IsValid) {
                var id = TempData["IdScript"];

                Script script = dbConnection.Script.Find(id);
                if (script == null) {
                    return HttpNotFound();
                }
                var firstContent = dbConnection.ScriptContent.Where(s => s.Script == script.IdScript).FirstOrDefault();
                ScriptContent freshContent;
                if (firstContent == null) {
                    freshContent = new ScriptContent() {
                        Script = script.IdScript,
                        Titel = content.Titel,
                        FatherContent = content.FatherContent,
                        Predecessor = content.Predecessor
                    };
                } else {
                    var lastContent = dbConnection.ScriptContent.Where(s => s.Script == script.IdScript).ToList().LastOrDefault();
                    freshContent = new ScriptContent() {
                        Script = script.IdScript,
                        Titel = content.Titel,
                        FatherContent = content.FatherContent,
                        Predecessor = lastContent.IdScriptContent
                    };
                }
                dbConnection.ScriptContent.Add(freshContent);
                dbConnection.SaveChanges();
                return RedirectToAction("ReadEditMode", new { id = script.IdScript });
            }
            ViewBag.IdScript = TempData["IdScript"];
            return View();
        }

        /// <summary>CreateSub
        /// Ruft die View auf um ein ScriptContent (Unterkarte) mit FatherContent zu erstellen
        /// </summary>
        /// <param name="id">ScriptContent ID(id)</param>
        /// <returns>View()</returns>
        [HttpGet]
        public ActionResult CreateSub(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempData["IdScriptContent"] = id;
            var content = dbConnection.ScriptContent.Where(i => i.IdScriptContent == id).FirstOrDefault();
            ViewBag.Titel = content.Titel;
            TempData["Id"] = content.Script;
            var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
            if (content.Script1.Author == user.IdAccount || User.IsInRole("Admin")) {
                ViewBag.Id = content.Script;
                return View();
            } else {
                ViewBag.Msg = messageAutor;
                return RedirectToAction("Login", "Login");
            }

        }

        /// <summary>CreateSub
        /// Methode um ScriptContent mit FatherContent in der Datenbank zu speichern
        /// </summary>
        /// <param name="content">ScriptContent (Model)</param>
        /// <returns>ReadEditMode(Script (Integer))</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSub(ScriptContent content) {

            if (ModelState.IsValid) {
                var id = TempData["IdScriptContent"];
                ScriptContent scriptContent = dbConnection.ScriptContent.Where(i => i.IdScriptContent == (int)id).FirstOrDefault();
                if (scriptContent == null) {
                    return HttpNotFound();
                }

                ViewBag.Id = scriptContent.Script;
                var firstContent = dbConnection.ScriptContent.Where(s => s.Script == scriptContent.Script && s.FatherContent == scriptContent.IdScriptContent).FirstOrDefault();
                ScriptContent freshContent;
                if (firstContent == null) {
                    freshContent = new ScriptContent() {
                        Script = scriptContent.Script,
                        FatherContent = scriptContent.IdScriptContent,
                        Titel = content.Titel,
                        Predecessor = null
                    };
                } else {
                    var lastContent = dbConnection.ScriptContent.Where(f => f.FatherContent == scriptContent.IdScriptContent).ToList().LastOrDefault();
                    freshContent = new ScriptContent() {
                        Script = scriptContent.Script,
                        FatherContent = scriptContent.IdScriptContent,
                        Titel = content.Titel,
                        Predecessor = lastContent.IdScriptContent
                    };
                }
                dbConnection.ScriptContent.Add(freshContent);
                dbConnection.SaveChanges();
                return RedirectToAction("ReadEditMode", new { id = scriptContent.Script });

            }
            ViewBag.Id = TempData["Id"];
            return View();
        }


        /// <summary>CreateTwin
        /// View um einen ScriptContent zu klonen und diesen als Subkarte zu speichern
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>ReadEditMode(Script ID (Integer))</returns>
        public ActionResult CreateTwin(int? id) {

            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScriptContent scriptContent = dbConnection.ScriptContent.Where(i => i.IdScriptContent == id).FirstOrDefault();
            if (scriptContent == null) {
                return HttpNotFound();
            }

            var firstContent = dbConnection.ScriptContent.Where(s => s.Script == scriptContent.Script && s.FatherContent == scriptContent.IdScriptContent).FirstOrDefault();
            ScriptContent freshContent;
            if (firstContent == null) {
                freshContent = new ScriptContent() {
                    Script = scriptContent.Script,
                    FatherContent = scriptContent.IdScriptContent,
                    Titel = scriptContent.Titel,
                    Predecessor = null
                };
                if (scriptContent.ContentAttributeList.Count() > 0) {
                    IEnumerable<ContentAttributeList> attributeList = dbConnection.ContentAttributeList.Where(c => c.ScriptContent == scriptContent.IdScriptContent).ToList();
                    foreach (var item in attributeList) {
                        ContentAttributeList newAttribute = new ContentAttributeList() {
                            ScriptContent = freshContent.IdScriptContent,
                            Attribute = item.Attribute
                        };
                        dbConnection.ContentAttributeList.Add(newAttribute);
                    }
                }
            } else {
                var lastContent = dbConnection.ScriptContent.Where(f => f.FatherContent == scriptContent.IdScriptContent).ToList().LastOrDefault();
                freshContent = new ScriptContent() {
                    Script = scriptContent.Script,
                    FatherContent = scriptContent.IdScriptContent,
                    Titel = scriptContent.Titel,
                    Predecessor = lastContent.IdScriptContent
                };
                if (scriptContent.ContentAttributeList.Count() > 0) {
                    IEnumerable<ContentAttributeList> attributeList = dbConnection.ContentAttributeList.Where(c => c.ScriptContent == scriptContent.IdScriptContent).ToList();
                    foreach (var item in attributeList) {
                        ContentAttributeList newAttribute = new ContentAttributeList() {
                            ScriptContent = freshContent.IdScriptContent,
                            Attribute = item.Attribute
                        };
                        dbConnection.ContentAttributeList.Add(newAttribute);
                    }
                }
            }
            dbConnection.ScriptContent.Add(freshContent);
            dbConnection.SaveChanges();
            return RedirectToAction("ReadEditMode", new { id = scriptContent.Script });
        }


        /// <summary>ReadEditMode
        /// Gibt ein Skript im Bearbeitungsmodus aus
        /// </summary>
        /// <param name="id">Script ID (Integer)</param>
        /// <returns>View(ScriptContent Liste)</returns>
        [HttpGet]
        public ActionResult ReadEditMode(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Script script = dbConnection.Script.Find(id);
            if (script == null) {
                return HttpNotFound();
            }
            var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
            if (script.Author == user.IdAccount || User.IsInRole("Admin")) {
                if (script.InWork == false) {
                    return RedirectToAction("Read", "ScriptContent", new { id = script.IdScript });
                }
                ViewBag.ScriptName = script.Subject;
                ViewBag.Id = script.IdScript;
                var content = dbConnection.ScriptContent.Where(s => s.Script == id).OrderBy(s => s.Predecessor).ToList();
                return View(content);
            } else {
                ViewBag.Msg = messageAutor;
                return RedirectToAction("Login", "Login");
            }
        }

        /// <summary>Read
        /// Gibt ein Skript im Lesemodus aus
        /// </summary>
        /// <param name="id">Script ID (Integer)</param>
        /// <returns>View(ScriptContent Liste)</returns>
        [OverrideAuthorization()]
        [Authorize]
        [HttpGet]
        public ActionResult Read(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Script script = dbConnection.Script.Find(id);
            if (script == null) {
                return HttpNotFound();
            }
            ViewBag.ScriptName = script.Subject;
            ViewBag.Id = script.IdScript;
            ViewBag.Status = script.InWork;
            var content = dbConnection.ScriptContent.Where(s => s.Script == id).OrderBy(p => p.Predecessor).ToList();
            return View(content);
        }

        /// <summary>ReadFirst
        /// Gibt das erste SkriptContent eines Skriptes aus
        /// </summary>
        /// <param name="id">Script ID (Integer)</param>
        /// <returns>View(ScriptContent (Model))</returns>
        [OverrideAuthorization()]
        [Authorize]
        [HttpGet]
        public ActionResult ReadFirst(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Script script = dbConnection.Script.Find(id);
            var contentFirst = dbConnection.ScriptContent.Where(s => s.Script == id).FirstOrDefault();
            if (script == null) {
                return HttpNotFound();
            }
            if (contentFirst != null) {
                ViewBag.nextContent = GetNext(contentFirst.IdScriptContent);
                ViewBag.Predecessor = GetPrevious(contentFirst.IdScriptContent);
            }

            ViewBag.ScriptName = script.Subject;
            ViewBag.Id = script.IdScript;
            ViewBag.Status = script.InWork;
            return View(contentFirst);
        }


        /// <summary>ReadNext
        /// Methode um zwischen den einzelnen ScriptContents (Karten) zu navigieren
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>View(ScriptContent (Model))</returns>
        [OverrideAuthorization()]
        [Authorize]
        [HttpGet]
        public ActionResult ReadNext(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var contentFirst = dbConnection.ScriptContent.Where(s => s.IdScriptContent == id).FirstOrDefault();
            Script script = dbConnection.Script.Find(contentFirst.Script);

            if (script == null || contentFirst == null) {
                return HttpNotFound();
            }

            if (contentFirst != null) {
                ViewBag.nextContent = GetNext(contentFirst.IdScriptContent);
                ViewBag.Predecessor = GetPrevious(contentFirst.IdScriptContent);
            }

            ViewBag.ScriptName = script.Subject;
            ViewBag.Id = script.IdScript;
            ViewBag.Status = script.InWork;
            return View(contentFirst);
        }



        /// <summary>GetNext
        /// Methode um die ID des nächsten Elements zu beschaffen
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>Integer</returns>
        public int? GetNext(int? id) {
            if (GetChild(id) != null) {
                return GetChild(id);
            } else if (GetBrother(id) != null) {
                return GetBrother(id);
            } else if (GetChild(id) == null && GetBrother(id) == null && GetFather(id) != null && GetUncle(id) != null) {
                return GetUncle(id);
            } else if (GetChild(id) == null && GetBrother(id) == null && GetFather(id) != null && GetUncle(id) == null) {
                int? fatherId = GetFather(id);
                return GetUncle(fatherId);
            } else {
                return null;
            }
        }

        /// <summary>GetPrevious
        /// Methode um die ID des vorherigen Elements zu beschaffen
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>Integer</returns>        
        public int? GetPrevious(int? id) {
            var node = dbConnection.ScriptContent.Find(id);
            var olderBrother = dbConnection.ScriptContent.Find(node.Predecessor);
            if (olderBrother == null && node.FatherContent != null) {
                return node.FatherContent;
            } else if (olderBrother != null && olderBrother.ScriptContent1.Count() > 0) {
                return GetOldestSon(olderBrother.IdScriptContent);
            } else if (node.FatherContent == null && olderBrother != null && olderBrother.ScriptContent1.Count() == 0) {
                return olderBrother.IdScriptContent;
            } else if (node.FatherContent != null && olderBrother != null && olderBrother.ScriptContent1.Count() == 0) {
                return olderBrother.IdScriptContent;
            } else if (node.FatherContent == null && olderBrother != null && olderBrother.ScriptContent1.Count() > 0) {
                return GetOldestSon(olderBrother.IdScriptContent);
            }
            return null;
        }

        /// <summary>GetOlderBrother
        /// Hilfsmethode für GetPrevious
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>Integer</returns>
        public int? GetOlderBrother(int? id) {
            var node = dbConnection.ScriptContent.Find(id);
            var olderBrother = dbConnection.ScriptContent.Where(s => s.IdScriptContent == node.Predecessor).FirstOrDefault();
            if (olderBrother != null) {
                if (olderBrother.ScriptContent1.Count() == 0) {
                    return olderBrother.IdScriptContent;
                }
            }
            return null;
        }

        /// <summary>GetOldestSon
        /// Hilfsmethode für GetPrevious
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>Integer</returns>
        public int? GetOldestSon(int? id) {
            var node = dbConnection.ScriptContent.Find(id);
            var oldestSon = node.ScriptContent1.Last();
            if (oldestSon.ScriptContent1.Count() > 0) {
                return GetOldestSon(oldestSon.IdScriptContent);
            }
            return oldestSon.IdScriptContent;
        }

        /// <summary>GetChild
        /// Hilfsmethode für GetNext
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>Integer</returns>
        public int? GetChild(int? id) {
            var father = dbConnection.ScriptContent.Find(id);
            if (father.ScriptContent1.Count() == 0) {
                return null;
            }
            var child = dbConnection.ScriptContent.Where(f => f.FatherContent == father.IdScriptContent).FirstOrDefault();
            return child.IdScriptContent;
        }

        /// <summary>GetBrother
        /// Hilfsmethode für GetNext
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>Integer</returns>
        public int? GetBrother(int? id) {
            var child = dbConnection.ScriptContent.Find(id);
            var brother = dbConnection.ScriptContent.Where(p => p.Predecessor == child.IdScriptContent).FirstOrDefault();
            if (brother == null) {
                return null;
            }
            return brother.IdScriptContent;
        }

        /// <summary>GetFather
        /// Hilfsmethode für GetNext
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>Integer</returns>
        public int? GetFather(int? id) {
            var child = dbConnection.ScriptContent.Find(id);
            var father = dbConnection.ScriptContent.Find(child.FatherContent);
            if (father == null) {
                return null;
            }
            return father.IdScriptContent;
        }

        /// <summary>GetUncle
        /// Hilfsmethode für GetNext
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>Integer</returns>
        public int? GetUncle(int? id) {
            var child = dbConnection.ScriptContent.Find(id);
            var father = dbConnection.ScriptContent.Find(child.FatherContent);
            if (GetBrother(father.IdScriptContent) != null) {
                return GetBrother(father.IdScriptContent);
            } else {
                return GetNextUncle(father.IdScriptContent);
            }
        }

        /// <summary>GetChild
        /// Rekursive Hilfsmethode für GetUncle
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>Integer</returns>
        public int? GetNextUncle(int? id) {
            while (GetFather(id) != null) {
                return GetUncle(id);
            }
            return null;
        }

        /// <summary>Edit
        /// Methode die eine View aufruft mit den jeweiligen Daten eines ScriptContents (Karte)
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>View(ScriptContent (Model))</returns>
        [HttpGet]
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScriptContent content = dbConnection.ScriptContent.Find(id);
            if (content == null) {
                return HttpNotFound();
            }
            TempData["content"] = content.IdScriptContent;
            ViewBag.content = content.IdScriptContent;
            TempData["Id"] = content.Script;
            ViewBag.Id = content.Script;
            var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
            if (content.Script1.Author == user.IdAccount || User.IsInRole("Admin")) {
                ViewBag.Script = new SelectList(dbConnection.Script, "IdScript", "Subject", content.Script);
                ViewBag.FatherContent = new SelectList(dbConnection.ScriptContent.Where(s => s.Script == content.Script), "IdScriptContent", "Titel", content.FatherContent);
                ViewBag.Predecessor = new SelectList(dbConnection.ScriptContent.Where(s => s.Script == content.Script &&
                                                                                           s.FatherContent == content.FatherContent &&
                                                                                           s.IdScriptContent != content.IdScriptContent), "IdScriptContent", "Titel", content.Predecessor);
                return View(content);
            } else {
                ViewBag.Msg = messageAutor;
                return RedirectToAction("Login", "Login");
            }

        }

        /// <summary>Edit
        /// Speichert die veränderten Daten des ScriptContents
        /// </summary>
        /// <param name="scriptContent">ScriptContent (Model)</param>
        /// <returns>ReadEditMode(Script ID (Integer))</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdScriptContent,Script,FatherContent,Titel,Predecessor")] ScriptContent scriptContent) {
            if (ModelState.IsValid) {
                dbConnection.Entry(scriptContent).State = EntityState.Modified;
                dbConnection.SaveChanges();
                return RedirectToAction("ReadEditMode", new { id = scriptContent.Script });
            }
            ViewBag.Script = new SelectList(dbConnection.Script, "IdScript", "Subject", scriptContent.Script);
            ViewBag.FatherContent = new SelectList(dbConnection.ScriptContent.Where(s => s.Script == scriptContent.Script), "IdScriptContent", "Titel", scriptContent.FatherContent);
            ViewBag.Predecessor = new SelectList(dbConnection.ScriptContent.Where(s => s.Script == scriptContent.Script &&
                                                                                       s.FatherContent == scriptContent.FatherContent &&
                                                                                       s.IdScriptContent != scriptContent.IdScriptContent), "IdScriptContent", "Titel", scriptContent.Predecessor);
            ViewBag.content = TempData["content"];
            ViewBag.Id = TempData["Id"];
            return View(scriptContent);
        }

        /// <summary>Delete
        /// Ruft View auf mit der gesamten Ansichten des ScriptContents (Karte, Unterkarten und Inhalte)
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>View(ScriptContent (Model))</returns>
        [HttpGet]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScriptContent scriptContent = dbConnection.ScriptContent.Find(id);
            if (scriptContent == null) {
                return HttpNotFound();
            }
            var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
            if (scriptContent.Script1.Author == user.IdAccount || User.IsInRole("Admin")) {
                return View(scriptContent);
            } else {
                ViewBag.Msg = messageAutor;
                return RedirectToAction("Login", "Login");
            }

        }

        /// <summary>DeleteConfirmed
        /// Löscht das ScriptContent samt den unteren ScriptContents und Inhalten aus der Datenbank
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>ReadEditMode(Script ID (Integer))</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            ScriptContent scriptContent = dbConnection.ScriptContent.Find(id);
            int idScript = scriptContent.Script1.IdScript;
            DeleteRecursive(scriptContent.IdScriptContent);
            dbConnection.SaveChanges();
            return RedirectToAction("ReadEditMode", new { id = idScript });
        }


        /// <summary>DeleteRecursive
        /// Rekursive Hilfsmethode von DeleteConfirmed um robust alle Daten zu löschen
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        public void DeleteRecursive(int id) {
            var father = dbConnection.ScriptContent.Where(i => i.IdScriptContent == id).FirstOrDefault();

            if (father.ScriptContent1 != null) {
                var child = dbConnection.ScriptContent.Where(f => f.FatherContent == id).OrderBy(p => p.Predecessor).ToList();
                foreach (var o in child) {
                    this.DeleteRecursive(o.IdScriptContent);
                }
            }
            if (father.TextCard.Count() > 0) {
                IEnumerable<TextCard> text = dbConnection.TextCard.Where(t => t.TextContent == father.IdScriptContent).ToList();
                foreach (var item in text) {
                    dbConnection.TextCard.Remove(item);
                }
            } else if (father.FormulaCard.Count() > 0) {
                IEnumerable<FormulaCard> formula = dbConnection.FormulaCard.Where(f => f.FormulaContent == father.IdScriptContent).ToList();
                foreach (var item in formula) {
                    dbConnection.FormulaCard.Remove(item);
                }
            } else if (father.ImageCard.Count() > 0) {
                IEnumerable<ImageCard> image = dbConnection.ImageCard.Where(i => i.ImageContent == father.IdScriptContent).ToList();
                foreach (var item in image) {
                    dbConnection.ImageCard.Remove(item);
                }
            } else if (father.VideoCard.Count() > 0) {
                IEnumerable<VideoCard> video = dbConnection.VideoCard.Where(v => v.VideoContent == father.IdScriptContent).ToList();
                foreach (var item in video) {
                    dbConnection.VideoCard.Remove(item);
                }
            } else if (father.LinkCard.Count() > 0) {
                IEnumerable<LinkCard> link = dbConnection.LinkCard.Where(l => l.LinkContent == father.IdScriptContent).ToList();
                foreach (var item in link) {
                    dbConnection.LinkCard.Remove(item);
                }
            } else if (father.LiteratureCard.Count() > 0) {
                IEnumerable<LiteratureCard> literature = dbConnection.LiteratureCard.Where(l => l.LiteratureContent == father.IdScriptContent).ToList();
                foreach (var item in literature) {
                    dbConnection.LiteratureCard.Remove(item);
                }
            }

            if (father.CommentCard.Count() > 0) {
                IEnumerable<CommentCard> comment = dbConnection.CommentCard.Where(s => s.ScriptContent == father.IdScriptContent).ToList();
                foreach (var item in comment) {
                    dbConnection.CommentCard.Remove(item);
                }
            }

            if (father.ContentAttributeList.Count() > 0) {
                IEnumerable<ContentAttributeList> attributeList = dbConnection.ContentAttributeList.Where(s => s.ScriptContent == father.IdScriptContent).ToList();
                foreach (var item in attributeList) {
                    dbConnection.ContentAttributeList.Remove(item);
                }
            }

            dbConnection.ScriptContent.Remove(father);
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
