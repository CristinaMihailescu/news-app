using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EngineDeStiri.Models;

namespace EngineDeStiri.Controllers
{
    
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new ArticleDBContext())
            {
                var articles = from article in db.Articles
                               orderby article.Title
                               select article;

                return View(articles.OrderByDescending(art => art.Date).ToList());
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}