namespace StudentManagementSystem.Web.Controllers
{
    using System.Web.Mvc;

    // [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home";
            return this.View();
        }

        public ActionResult About()
        {
            ViewBag.Title = "About US";
            return this.View();
        }


        public ActionResult Schools()
        {
            ViewBag.Title = "Schools";
            return this.View();
        }


        public ActionResult Curriculum()
        {
            ViewBag.Title = "Curriculum";
            return this.View();
        }

        public ActionResult Calendar()
        {
            ViewBag.Title = "Calendar";
            return this.View();
        }

        public ActionResult Faq()
        {
            ViewBag.Title = "FAQ";
            return this.View();
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "Contact US";
            return this.View();
        }
    }
}
