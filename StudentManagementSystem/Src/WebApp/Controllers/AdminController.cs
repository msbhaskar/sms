namespace StudentManagementSystem.Web.Controllers
{
    using System.Web.Mvc;

    [Authorize]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            this.ViewBag.Title = "Admin Dashboard";
            return this.View();
        }
    }
}