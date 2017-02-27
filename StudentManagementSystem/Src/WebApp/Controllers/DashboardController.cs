namespace StudentManagementSystem.Web.Controllers
{
    using System.Web.Mvc;

    [Authorize]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "User Dashboard";
            return View();
        }
    }
}
