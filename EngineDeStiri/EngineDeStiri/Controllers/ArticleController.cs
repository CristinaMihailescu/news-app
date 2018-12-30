using EngineDeStiri.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Globalization;

namespace EngineDeStiri.Controllers
{
    public class ArticleController : Controller
    {
        private ArticleDBContext db = new ArticleDBContext();
        public ActionResult Index()
        {
            var articles = from article in db.Articles
                           orderby article.Title
                           select article;
            ViewBag.Articles = articles;
            return View();
        }
        public ActionResult Show(int id)
        {
            Article article = db.Articles.Find(id);
            ViewBag.Article = article;
            ViewBag.Comments = article.Comments;
            return View();
        }

        public ActionResult New()
        {
            ViewBag.CategoryId = db.Categories;
            return View();
        }
        [HttpPost]
        public ActionResult New(Article article)
        {
            article.Date = DateTime.Now;
            article.Author = User.Identity.GetUserId();
            try
            {
                db.Articles.Add(article);
                db.SaveChanges();
                Article art = null;
                foreach (var x in db.Articles)
                {
                    if((x.Title == article.Title) && (x.Author == article.Author) && (x.Date == article.Date) && (x.Content == article.Content) && (x.Thumbnail == article.Thumbnail)) {
                        art = x;
                        break;
                    }
                }
                return RedirectToAction("Show", new { id = art.ArticleId });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("OOPS");
                ViewBag.CategoryId = db.Categories;
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
                    article.Thumbnail = requestArticle.Thumbnail;
                    article.Content = requestArticle.Content;
                    db.SaveChanges();
                }
                return RedirectToAction("Show", new { id = article.ArticleId });
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
                    article.Categories.Add(cat);
                    db.SaveChanges();
                //}

                    return RedirectToAction("Show", new { id = article.ArticleId });
            }
            catch (Exception e)
            {
                ViewBag.ArticleId = id;
                ViewBag.CategoryId = db.Categories;
                return View();
            }
        }

        public ActionResult AddComment(int id)
        {
            ViewBag.ArticleId = id;
            return View();
        }

        [HttpPost]
        public ActionResult AddComment(int id, Comment comment)
        {
            Article article = db.Articles.Find(id); //get current article
            comment.AuthorId = User.Identity.GetUserId();
            comment.AuthorName = User.Identity.Name;
            comment.Date = DateTime.Now;
            
            try
            {
                article.Comments.Add(comment);
                comment.Article = article; //might not be necessary
                db.SaveChanges();
                return RedirectToAction("Show", new { id = article.ArticleId });
            }
            catch (Exception e)
            {
                return View();
            }
        }
    }
}