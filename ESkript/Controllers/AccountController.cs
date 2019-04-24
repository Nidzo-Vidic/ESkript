using ESkript.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;


namespace ESkript.Controllers {


    /// <summary>AccountController
    /// Logik für das Account Model
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller {

        /// <summary>dbConnection
        /// Verbindung zur Datenbank
        /// </summary>
        private ESkriptModel dbConnection = new ESkriptModel();


        /// <summary>Index
        /// Gibt eine Liste aller Accounts aus
        /// </summary>
        /// <returns>View(Account Model (Liste))</returns>
        public ActionResult Index() {
            var account = dbConnection.Account.Include(r => r.Role);
            return View(account.ToList());
        }


        /// <summary>ModList
        /// Gibt eine Liste der Accounts aus, welche die Rolle eines Moderators besitzen
        /// </summary>
        /// <returns>View(Account Model (Liste))</returns>
        [OverrideAuthorization]
        [Authorize(Roles="Admin,Dozent")]
        public ActionResult ModList() {
            var modList = dbConnection.Account.Where(r => r.Role.IdRole == 3).ToList();
            return View(modList);
        }

        /// <summary> StudentList
        /// Gibt eine Liste der Accounts aus, welche die Rolle eines Studenten besitzen
        /// </summary>
        /// <returns>View(Account Model (Liste))</returns>
        [OverrideAuthorization]
        [Authorize(Roles = "Admin,Dozent")]
        public ActionResult StudentList() {
            var studentList = dbConnection.Account.Where(r => r.Role.IdRole == 4).ToList(); 
            return View(studentList);
        }

        /// <summary>Create
        /// Ruft eine View um einen Account zu erstellen und übergibt eine Dropdownliste um eine Rolle für den Account zu wählen
        /// </summary>
        /// <returns>View()</returns>
        public ActionResult Create() {
            ViewBag.AccountRole = new SelectList(dbConnection.Role, "IdRole", "RoleName");
            return View();
        }
        

        /// <summary>Create
        /// Speichert einen neuen Account in der Datenbank
        /// </summary>
        /// <param name="account">Account Model</param>
        /// <returns>View(Index)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdAccount,FirstName,LastName,EMail,Password,AccountRole")] Account account) {
            if (ModelState.IsValid) {
                var twin = dbConnection.Account.Where(e => e.EMail == account.EMail).FirstOrDefault();
                if (twin != null) {
                    ViewBag.Msg = "Ein Benutzer mit dieser Email Adresse existiert schon!";
                    return View();
                }
                dbConnection.Account.Add(account);
                dbConnection.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        /// <summary>Register
        /// Ruft eine View für die Registrierung auf
        /// </summary>
        /// <returns>View</returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register() {
            if (User.Identity.IsAuthenticated) {
                ViewBag.Msg = "Sie sind schon bereits registriert.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        /// <summary>Register
        /// Speichert einen neuen Account in der Datenbank 
        /// </summary>
        /// <param name="registerModel">RegisterModel Model</param>
        /// <returns>Index Methode</returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Exclude = "AccountRole")] RegisterModel registerModel) {
            if (ModelState.IsValid) {
                var account = new Account() {
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    EMail = registerModel.EMail,
                    Password = registerModel.Password,
                    AccountRole = 4
                };
                var twin = dbConnection.Account.Where(e => e.EMail == account.EMail).FirstOrDefault();
                if (twin != null) {
                    ViewBag.Msg = "Ein Benutzer mit dieser Email Adresse existiert schon!";
                    return View();
                }
                dbConnection.Account.Add(account);
                dbConnection.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        /// <summary>Edit
        /// Ruft eine View auf für die Modifikation der Daten eines Accounts
        /// </summary>
        /// <param name="id">Account ID (Integer)</param>
        /// <returns>View(Account Model)</returns>
        [OverrideAuthorization]
        [Authorize(Roles = "Admin,Dozent")]
        [HttpGet]
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = dbConnection.Account.Find(id);
            if (account == null) {
                return HttpNotFound();
            }
            if (User.IsInRole("Admin")) {
                ViewBag.AccountRole = new SelectList(dbConnection.Role, "IdRole", "RoleName", account.AccountRole);
            } else if (User.IsInRole("Dozent")) {
                ViewBag.AccountRole = new SelectList(dbConnection.Role.Where(r => r.IdRole >= 3), "IdRole", "RoleName", account.AccountRole);
            }
            return View(account);
        }

        /// <summary>Edit
        /// Speicher die Veränderungen der Daten eines Accounts
        /// </summary>
        /// <param name="account">Account Model</param>
        /// <returns>Index Methode</returns>
        [OverrideAuthorization]
        [Authorize(Roles = "Admin,Dozent")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdAccount,FirstName,LastName,EMail,Password,AccountRole")] Account account) {
            if (ModelState.IsValid) {
                dbConnection.Entry(account).State = EntityState.Modified;
                dbConnection.SaveChanges();
                if (User.IsInRole("Dozent")) {
                    return RedirectToAction("StudentList");
                }
                return RedirectToAction("Index");
            }
            return View(account);
        }

        /// <summary>EditCurrentUser
        /// Ruft eine View mit den Daten des aktullen Benutzers auf
        /// </summary>
        /// <returns></returns>
        [OverrideAuthorization]
        [Authorize]
        [HttpGet]
        public ActionResult EditCurrentUser() {
            var user = dbConnection.Account.Where(e => e.EMail == User.Identity.Name).FirstOrDefault();
            Account account = dbConnection.Account.Find(user.IdAccount);
            if (account == null) {
                return HttpNotFound();
            }
            ViewBag.AccountRole = new SelectList(dbConnection.Role, "IdRole", "RoleName", account.AccountRole);
            return View(account);
        }

        /// <summary>EditCurrentUser
        /// Speicher die Veränderungen
        /// </summary>
        /// <param name="account">Account Model</param>
        /// <returns>Index Methode, Home Controller</returns>
        [OverrideAuthorization]
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCurrentUser([Bind(Include = "IdAccount,FirstName,LastName,EMail,Password,AccountRole")] Account account) {
            if (ModelState.IsValid) {
                dbConnection.Entry(account).State = EntityState.Modified;
                dbConnection.SaveChanges();
                return RedirectToAction("Index", "Home", null);
            }
            return View(account);
        }

        /// <summary>Delete
        /// Ruft View zum Löschen eines Accounts auf
        /// </summary>
        /// <param name="id">Account ID (Integer)</param>
        /// <returns>View(Account Model)</returns>
        [HttpGet]
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = dbConnection.Account.Find(id);
            if (account == null) {
                return HttpNotFound();
            }
            return View(account);
        }

        /// <summary>DeleteConfirmed
        /// Löscht den Account aus der Datenbank
        /// </summary>
        /// <param name="id">Account ID (Integer)</param>
        /// <returns>Index</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            Account account = dbConnection.Account.Find(id);
            dbConnection.Account.Remove(account);
            dbConnection.SaveChanges();
            return RedirectToAction("Index");
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
