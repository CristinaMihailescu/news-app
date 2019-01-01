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

        [MyAuthorize(Roles = "Editor, Administrator")]
        public ActionResult New()
        {
            ViewBag.CategoryId = db.Categories;
            return View();

        }

        [HttpPost]
        [MyAuthorize(Roles = "Editor, Administrator")]
        public ActionResult New(Article article)
        {
            article.Date = DateTime.Now;
            article.Author = User.Identity.GetUserId();
            article.Username = User.Identity.GetUserName();
            try
            {
                db.Articles.Add(article);
                db.SaveChanges();
                Article art = null;
                foreach (var x in db.Articles)
                {
                    if ((x.Title == article.Title) && (x.Headline == article.Headline) && (x.Author == article.Author) && (x.Date == article.Date) && (x.Content == article.Content) && (x.Thumbnail == article.Thumbnail))
                    {
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

        [Authorize(Roles = "Editor, Administrator")]
        public ActionResult Quick()
        {
            ViewBag.CategoryId = db.Categories;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Editor, Administrator")]
        public ActionResult Quick(Article article)
        {
            article.Date = DateTime.Now;
            article.Author = User.Identity.GetUserId();
            article.Username = User.Identity.GetUserName();
            try
            {
                db.Articles.Add(article);
                db.SaveChanges();
                Article art = null;
                foreach (var x in db.Articles)
                {
                    if ((x.Title == article.Title) && (x.Headline == article.Headline) && (x.Author == article.Author) && (x.Date == article.Date) && (x.URL == article.URL) && (x.Thumbnail == article.Thumbnail))
                    {
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

        [Authorize(Roles = "Editor, Administrator")]
        public ActionResult Edit(int id)
        {
            Article article = db.Articles.Find(id);
            if (article.Author == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                ViewBag.Article = article;
                return View();
            }
            else
            {
                return View("Unauthorized");
            }

        }

        [HttpPut]
        [Authorize(Roles = "Editor, Administrator")]
        public ActionResult Edit(int id, Article requestArticle)
        {
            if (requestArticle.Author == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                try
                {
                    Article article = db.Articles.Find(id);
                    if (TryUpdateModel(article))
                    {
                        if (requestArticle.Title != null)
                        {
                            article.Title = requestArticle.Title;
                        }
                        if (requestArticle.Date != null)
                        {
                            article.Date = requestArticle.Date;
                        }
                        if (requestArticle.Thumbnail != null)
                        {
                            article.Thumbnail = requestArticle.Thumbnail;
                        }
                        if (requestArticle.Headline != null)
                        {
                            article.Headline = requestArticle.Headline;
                        }
                        if (requestArticle.Content != null)
                        {
                            article.Content = requestArticle.Content;
                        }
                        if (requestArticle.URL != null)
                        {
                            article.URL = requestArticle.URL;
                        }
                        db.SaveChanges();
                    }
                    return RedirectToAction("Show", new { id = article.ArticleId });
                }
                catch (Exception e)
                {
                    return View();
                }
            }
            else
            {
                return View("Unauthorized");
            }

        }

        [HttpDelete]
        [Authorize(Roles = "Editor, Administrator")]
        public ActionResult Delete(int id)
        {
            Article article = db.Articles.Find(id);
            if (article.Author == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                db.Articles.Remove(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View("Unauthorized");
            }
            
        }

        [Authorize(Roles = "Administrator, Editor, User")]
        public ActionResult AddCategory(int id)
        {
            Article article = db.Articles.Find(id);
            if (article.Author == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                ViewBag.ArticleId = id;
                ViewBag.CategoryId = db.Categories;
                return View();
            }
            else
            {
                return View("Unauthorized");
            }

        }

        [HttpPut]
        [Authorize(Roles = "Administrator, Editor, User")]
        public ActionResult AddCategory(int id, int CategoryId)
        {
            Article article = db.Articles.Find(id);
            if (article.Author == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                try
                {
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
            else
            {
                return View("Unauthorized");
            }

        }

        public ActionResult AddComment(int id)
        {
            if (User.IsInRole("User") || User.IsInRole("Editor") || User.IsInRole("Administrator"))
            {
                ViewBag.ArticleId = id;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "AccountController");
            }
            
        }

        [HttpPost]
        public ActionResult AddComment(int id, Comment comment)
        {
            if (User.IsInRole("User") || User.IsInRole("Editor") || User.IsInRole("Administrator"))
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
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }
    }
}