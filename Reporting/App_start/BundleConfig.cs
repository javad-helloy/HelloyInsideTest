using System.Web.Optimization;

namespace InsideReporting
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/core")
                .Include("~/ScriptsExternal/jquery-2.1.0.js")
                .Include("~/ScriptsExternal/jquery-ui-factory.min.js")
                .Include("~/ScriptsExternal/bootstrap.min.js")
                .Include("~/ScriptsExternal/jquery.validate*")
                .IncludeDirectory("~/ScriptsExternal", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/minify")
                .Include("~/widget/shared/jquery.hi.reportdefinition.js")
                .Include("~/widget/shared/jquery.hi.reportwidget.js")
                .Include("~/widget/shared/jquery.hi.contactdata.js")
                .Include("~/widget/shared/jquery.hi.summarychart.js")
                .Include("~/widget/shared/jquery.hi.screenresize.js")
                .Include("~/widget/shared/jquery.hi.datedependentwidget.js")
                .Include("~/widget/shared/jquery.hi.selecteddata.js")
                .IncludeDirectory("~/widget/plugin", "*.js", false)
                .IncludeDirectory("~/widget", "*.js", false));
        }
    }
}
