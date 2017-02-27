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

        [HttpGet]
        [Route("About")]
        public ActionResult About()
        {
            ViewBag.Title = "About US";
            return this.View();
        }

        [Route("Schools")]
        public ActionResult Schools()
        {
            ViewBag.Title = "Schools";
            return this.View();
        }

        [Route("Curriculum")]
        public ActionResult Curriculum()
        {
            ViewBag.Title = "Curriculum";
            return this.View();
        }

        [Route("Calendar")]
        public ActionResult Calendar()
        {
            ViewBag.Title = "Calendar";
            return this.View();
        }

        [Route("Faq")]
        public ActionResult Faq()
        {
            ViewBag.Title = "FAQ";
            return this.View();
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            ViewBag.Title = "Contact US";
            return this.View();
        }
    }
}
