using EngineDeStiri.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EngineDeStiri.Controllers
{
    public class ArticleController : Controller
    {
        private ArticleDBContext db = new ArticleDBContext();
        public ActionResult Index()
        {
            System.Diagnostics.Debug.WriteLine("HELLO!!!!!!!!!!!!!!!!");
            var articles = from article in db.Articles
                           orderby article.Title
                           select article;

            ViewBag.Articles = articles;

            /*
            foreach (var art in articles)
            {
                if (art.Categories.Any() == false)
                {
                    System.Diagnostics.Debug.WriteLine("NULL");
                }
            }
            */
            return View();
        }
        public ActionResult Show(int id)
        {
            Article article = db.Articles.Find(id);
            ViewBag.Article = article;
            return View();
        }

        public ActionResult New()
        {
            return View();
        }
        [HttpPost]
        public ActionResult New(Article article)
        {
            article.Date = DateTime.Now;
            article.Author = "Test";
            try
            {
                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            Article article = db.Articles.Find(id);
            ViewBag.Article = article;
            return View();
        }

        [HttpPut]
        public ActionResult Edit(int id, Article requestArticle)
        {
            try
            {
                Article article = db.Articles.Find(id);
                if (TryUpdateModel(article))
                {
                    article.Title = requestArticle.Title;
                    article.Date = requestArticle.Date;
                    article.Author = requestArticle.Author;
                    article.Thumbnail = requestArticle.Thumbnail;
                    article.Content = requestArticle.Content;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddCategory(int id)
        {
            ViewBag.ArticleId = id;
            ViewBag.CategoryId = db.Categories;
            return View();
        }

        [HttpPut]
        public ActionResult AddCategory(int id, int CategoryId)
        {
            try
            {
                Article article = db.Articles.Find(id);
                //if (TryUpdateModel(article))
                //{
                    var cat = (from x in db.Categories.OfType<Category>() where x.CategoryId == CategoryId select x).FirstOrDefault();
                    /*
                    if (article.Categories == null)
                    {
                        article.Categories = new Collection<Category>();
                    }
                    */
                    article.Categories.Add(cat);
                    db.SaveChanges();
                //}

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewBag.ArticleId = id;
                ViewBag.CategoryId = db.Categories;
                return View();
            }
        }
    }
}