namespace StudentManagementSystem.Web.Controllers
{
    using System.Web.Mvc;

    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            this.ViewBag.Title = "Administration Dashboard";
            return this.View();
        }
    }
}