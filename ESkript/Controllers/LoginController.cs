using ESkript.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace ESkript.Controllers {

    /// <summary>LoginController
    /// Login Logik
    /// </summary>
    [AllowAnonymous]
    public class LoginController : Controller {

        /// <summary>dbConnection
        /// Verbindung zur Datenbank
        /// </summary>
        private ESkriptModel dbConnection = new ESkriptModel();
        

        /// <summary>Login
        /// Lädt View zum Login
        /// </summary>
        /// <returns>View()</returns>
        public ActionResult Login() {
            return View();
        }

        /// <summary>Login
        /// Methode zum Einloggen eines Benutzers
        /// </summary>
        /// <param name="u">LoginModel Model</param>
        /// <returns>Index Methode, Home Controller</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel u) {
            if (ModelState.IsValid) {
                var user = dbConnection.Account.Where(e => e.EMail.Equals(u.EMail) && e.Password.Equals(u.Password)).FirstOrDefault();
                if (user != null) {
                    FormsAuthentication.SetAuthCookie(u.EMail, false);
                    return RedirectToAction("Index", "Home");
                } else {
                    ViewBag.Msg = "Invalid User";
                    return View();
                }
            }
            return View();
        }

        /// <summary>Logout
        /// Loggt aktuellen Benutzer aus
        /// </summary>
        /// <returns>Index Methode, Home Controller</returns>
        [OverrideAuthorization]
        [Authorize]
        public ActionResult Logout() {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}