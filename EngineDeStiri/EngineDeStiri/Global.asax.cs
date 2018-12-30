using EngineDeStiri.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;

namespace EngineDeStiri
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private ArticleDBContext db = new ArticleDBContext();
        protected void Application_Start()
        {
            Database.SetInitializer<ArticleDBContext>(new DropCreateDatabaseIfModelChanges<ArticleDBContext>());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
