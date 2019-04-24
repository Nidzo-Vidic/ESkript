using ESkript.Models;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ESkript.Controllers {

    /// <summary>ImageCardController
    /// Logik für Bilder
    /// </summary>
    [Authorize(Roles = "Admin,Dozent")]
    public class ImageCardController : Controller {

        /// <summary>dbConnection
        /// Verbindung zur Datenbank
        /// </summary>
        private ESkriptModel dbConnection = new ESkriptModel();

        /// <summary>UploadImage
        /// Lädt View zum Hochladen von Bildern
        /// </summary>
        /// <param name="id">ScriptContent ID (Integer)</param>
        /// <returns>View()</returns>
        [HttpGet]
        public ActionResult UploadImage(int? id) {
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

        /// <summary>UploadImage
        /// Lädt Bild hoch und speichert es in der Datenbank
        /// </summary>
        /// <param name="image">HttPostedFileBase Model</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadImage(HttpPostedFileBase image) {
            ImageCard picture = new ImageCard();
            int id = (int)TempData["IdContent"];
            var content = dbConnection.ScriptContent.Where(i => i.IdScriptContent == id).FirstOrDefault();
            if (image != null) {
                picture.Image = new byte[image.ContentLength];
                if (image.ContentLength > 5242880) {
                    ViewBag.Msg = "Das Bild darf nicht größer als 10 MB sein!";
                    return View();
                }
                image.InputStream.Read(picture.Image, 0, image.ContentLength);
                picture.ImageContent = content.IdScriptContent;
                dbConnection.ImageCard.Add(picture);
                dbConnection.SaveChanges();
                return RedirectToAction("ReadEditMode", "ScriptContent", new { id = content.Script });
            }
            return RedirectToAction("ReadEditMode", "ScriptContent", new { id = content.Script });
        }

        /// <summary>Delete
        /// Lädt View zum Löschen
        /// </summary>
        /// <param name="id">ImageCard ID (Integer)</param>
        /// <returns>View(ImageCard Model)</returns>
        [HttpGet]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImageCard imageCard = dbConnection.ImageCard.Find(id);
            var content = dbConnection.ScriptContent.Where(i => i.IdScriptContent == id).FirstOrDefault();
            if (imageCard == null) {
                return HttpNotFound();
            }
            return View(imageCard);
        }

        /// <summary>DelteConfirmed
        /// Löscht Bild aus Datenbank
        /// </summary>
        /// <param name="id">ImageCard ID (Integer)</param>
        /// <returns>ReadEditMode(Script ID (Integer)), ScriptContent Controller</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            ImageCard imageCard = dbConnection.ImageCard.Find(id);
            int idScript = (int)imageCard.ScriptContent.Script;
            dbConnection.ImageCard.Remove(imageCard);
            dbConnection.SaveChanges();
            return RedirectToAction("ReadEditMode", "ScriptContent", new { id = idScript});
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
