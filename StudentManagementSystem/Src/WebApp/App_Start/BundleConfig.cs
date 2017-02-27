namespace StudentManagementSystem.Web
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                 "~/Content/bootstrap.css",
                 "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/Themes/Mentor/css").Include(
                "~/Content/Themes/Mentor/Css/bootstrap.min.css",
                "~/Content/Themes/Mentor/Css/font-awesome.min.css",
                "~/Content/Themes/Mentor/Css/imagehover.min.css",
                "~/Content/Themes/Mentor/Css/style.css"));

            bundles.Add(new StyleBundle("~/Content/Themes/Moderna/css").Include(
                "~/Content/Themes/Moderna/Css/bootstrap.min.css",
                "~/Content/Themes/Moderna/Css/fancybox/jquery.fancybox.css",
                "~/Content/Themes/Moderna/Css/jcarousel.css",
                "~/Content/Themes/Moderna/Css/flexslider.css",
                "~/Content/Themes/Moderna/Css/style.css",
                "~/Content/Themes/Moderna/skins/default.css"));

            bundles.Add(new ScriptBundle("~/bundles/themes/moderna/js").Include(
                "~/Content/Themes/Moderna/js/jquery.easing.1.3.js",
                "~/Content/Themes/Moderna/js/bootstrap.min.js",
                "~/Content/Themes/Moderna/js/jquery.fancybox.pack.js",
                "~/Content/Themes/Moderna/js/jquery.fancybox-media.js",
                "~/Content/Themes/Moderna/js/google-code-prettify/prettify.js",
                "~/Content/Themes/Moderna/js/portfolio/jquery.quicksand.js",
                "~/Content/Themes/Moderna/js/portfolio/setting.js",
                "~/Content/Themes/Moderna/js/jquery.flexslider.js",
                "~/Content/Themes/Moderna/js/animate.js",
                "~/Content/Themes/Moderna/js/custom.js"));
        }
    }
}
