using EngineDeStiri.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EngineDeStiri.Controllers
{
    public class ArticleSuggestionController : Controller
    {
        private ArticleDBContext db = new ArticleDBContext();
        public ActionResult Index()
        {
            var articleSuggestions = from articleSuggestion in db.ArticleSuggestions
                                     orderby articleSuggestion.Title
                                     select articleSuggestion;
            ViewBag.ArticleSuggestions = articleSuggestions;
            return View();
        }
        public ActionResult Show(int id)
        {
            ArticleSuggestion articleSuggestion = db.ArticleSuggestions.Find(id);
            ViewBag.ArticleSuggestion = articleSuggestion;
            ViewBag.Comments = articleSuggestion.Comments;
            return View();
        }

        public ActionResult New()
        {
            ViewBag.CategoryId = db.Categories;
            return View();
        }
        [HttpPost]
        public ActionResult New(ArticleSuggestion articleSuggestion)
        {
            articleSuggestion.Date = DateTime.Now;
            articleSuggestion.UserId = User.Identity.GetUserId();
            try
            {
                db.ArticleSuggestions.Add(articleSuggestion);
                db.SaveChanges();
                ArticleSuggestion art = null;
                foreach (var x in db.ArticleSuggestions)
                {
                    if ((x.Title == articleSuggestion.Title) && (x.UserId == articleSuggestion.UserId) && (x.Date == articleSuggestion.Date) && (x.Content == articleSuggestion.Content))
                    {
                        art = x;
                        break;
                    }
                }
                //add popup THANK YOU FOR YOU SUGGESTION BLA BLA
                return RedirectToAction("Show", new { id = art.ArticleSuggestionId });
            }
            catch (Exception e)
            {
                ViewBag.CategoryId = db.Categories;
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            ArticleSuggestion articleSuggestion = db.ArticleSuggestions.Find(id);
            ViewBag.ArticleSuggestion = articleSuggestion;
            return View();
        }

        [HttpPut]
        public ActionResult Edit(int id, ArticleSuggestion requestArticleSuggestion)
        {
            try
            {
                ArticleSuggestion articleSuggestion = db.ArticleSuggestions.Find(id);
                if (TryUpdateModel(articleSuggestion))
                {
                    articleSuggestion.Title = requestArticleSuggestion.Title;
                    articleSuggestion.Date = requestArticleSuggestion.Date;
                    articleSuggestion.Content = requestArticleSuggestion.Content;
                    db.SaveChanges();
                }
                return RedirectToAction("Show", new { id = articleSuggestion.ArticleSuggestionId });
            }
            catch (Exception e)
            {
                return View();
            }
        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            ArticleSuggestion articleSuggestion = db.ArticleSuggestions.Find(id);
            db.ArticleSuggestions.Remove(articleSuggestion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddCategory(int id)
        {
            ViewBag.ArticleSuggestionId = id;
            ViewBag.CategoryId = db.Categories;
            return View();
        }

        [HttpPut]
        public ActionResult AddCategory(int id, int CategoryId)
        {
            try
            {
                ArticleSuggestion articleSuggestion = db.ArticleSuggestions.Find(id);
                //if (TryUpdateModel(article))
                //{
                var cat = (from x in db.Categories.OfType<Category>() where x.CategoryId == CategoryId select x).FirstOrDefault();
                articleSuggestion.Categories.Add(cat);
                db.SaveChanges();
                //}

                return RedirectToAction("Show", new { id = articleSuggestion.ArticleSuggestionId });
            }
            catch (Exception e)
            {
                ViewBag.ArticleSuggestionId = id;
                ViewBag.CategoryId = db.Categories;
                return View();
            }
        }

        public ActionResult AddComment(int id)
        {
            ViewBag.ArticleSuggestionId = id;
            return View();
        }

        [HttpPost]
        public ActionResult AddComment(int id, Comment comment)
        {
            ArticleSuggestion articleSuggestion = db.ArticleSuggestions.Find(id); //get current article
            comment.AuthorId = User.Identity.GetUserId();
            comment.AuthorName = User.Identity.Name;
            comment.Date = DateTime.Now;

            try
            {
                articleSuggestion.Comments.Add(comment);
                comment.ArticleSuggestion = articleSuggestion; //might not be necessary
                db.SaveChanges();
                return RedirectToAction("Show", new { id = articleSuggestion.ArticleSuggestionId });
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpPut]
        public ActionResult Accept(int id)
        {
            var articleSuggestion = db.ArticleSuggestions.Find(id);

            if (TryUpdateModel(articleSuggestion))
            {
                articleSuggestion.EditorId = User.Identity.GetUserId();
                articleSuggestion.Accepted = 1;
                ViewBag.ArticleSuggestion = articleSuggestion;
                db.SaveChanges();
                //POP UP THX FOR ACCEPTING THE SUGGESTION
                return RedirectToAction("Show", new { id = articleSuggestion.ArticleSuggestionId });
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        [HttpPut]
        public ActionResult Reject(int id)
        {
            var articleSuggestion = db.ArticleSuggestions.Find(id);

            if (TryUpdateModel(articleSuggestion))
            {
                articleSuggestion.EditorId = User.Identity.GetUserId();
                articleSuggestion.Accepted = 0;
                ViewBag.EditorName = User.Identity.GetUserName();
                ViewBag.ArticleSuggestion = articleSuggestion;
                db.SaveChanges();
                //POP UP THX FOR ACCEPTING THE SUGGESTION
                return RedirectToAction("Show", new { id = articleSuggestion.ArticleSuggestionId });
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
    }
}