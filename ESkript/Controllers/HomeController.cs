using System.Web.Mvc;

namespace ESkript.Controllers {


    /// <summary>
    /// Home Controller, verweist je nach Rolle auf eine View
    /// </summary>
    [AllowAnonymous]
    public class HomeController : Controller {
        // GET: Home
        
        /// <summary>Index
        /// Gibt je nach Rolle eine andere View aus
        /// </summary>
        /// <returns>View()</returns>
        public ActionResult Index() {
            if (User.Identity.IsAuthenticated) {
                if (User.IsInRole("Admin")) {
                    return RedirectToAction("ScriptListGodMode", "Script", null);
                } else if (User.IsInRole("Dozent")) {
                    return RedirectToAction("InWorkList", "Script", null);
                } else if (User.IsInRole("Student") || User.IsInRole("Mod")) {
                    return RedirectToAction("ScriptList", "Script", null);
                } else
                    return View();
            } else {
                return View();
            }
        }
    }
}