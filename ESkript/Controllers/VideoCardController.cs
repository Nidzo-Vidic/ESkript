using ESkript.Models;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ESkript.Controllers {

    /// <summary>VideoCardLogik
    /// Logik für Videos
    /// </summary>
    [Authorize(Roles = "Admin,Dozent")]
    public class VideoCardController : Controller {

        /// <summary>dbConnection
        /// Verbindung zur Datenbank
        /// </summary>
        private ESkriptModel dbConnection = new ESkriptModel();

        /// <summary>UploadVideo
        /// View zum Hochladen von Videos
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>View()</returns>
        [HttpGet]
        public ActionResult UploadVideo(int? id) {
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


        /// <summary>UploadVideo
        /// Speichert Video in der Datenbank
        /// </summary>
        /// <param name="video">VideoCard Model</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadVideo(HttpPostedFileBase video) {
            VideoCard vid = new VideoCard();
            int id = (int)TempData["IdContent"];
            var content = dbConnection.ScriptContent.Where(i => i.IdScriptContent == id).FirstOrDefault();
            if (video != null) {
                vid.Video = new byte[video.ContentLength];
                video.InputStream.Read(vid.Video, 0, video.ContentLength);
                if (video.ContentLength > 67108864) {
                    ViewBag.Msg = "Das Video darf nicht größer als 64 MB sein!";
                    return View();
                }
                vid.VideoContent = content.IdScriptContent;
                dbConnection.VideoCard.Add(vid);
                dbConnection.SaveChanges();
                return RedirectToAction("ReadEditMode", "ScriptContent", new { id = content.Script });
            }
            return RedirectToAction("ReadEditMode", "ScriptContent", new { id = content.Script });
        }


        /// <summary>Delete
        /// Lädt View zum Löschen vom Video 
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>View(VideoCard Model)</returns>
        [HttpGet]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VideoCard videoCard = dbConnection.VideoCard.Find(id);
            ViewBag.Id = videoCard.ScriptContent.Script;
            if (videoCard == null) {
                return HttpNotFound();
            }
            return View(videoCard);
        }

        /// <summary>DeleteConfirmed
        /// Löscht Video aus Datenbank
        /// </summary>
        /// <param name="id">VideoCard ID (Integer)</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            VideoCard videoCard = dbConnection.VideoCard.Find(id);
            int idScript = (int)videoCard.ScriptContent.Script;
            dbConnection.VideoCard.Remove(videoCard);
            dbConnection.SaveChanges();
            return RedirectToAction("ReadEditMode", "ScriptContent", new { id = idScript });
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