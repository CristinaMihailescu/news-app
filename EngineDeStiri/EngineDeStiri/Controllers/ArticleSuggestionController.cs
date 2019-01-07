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
            if (User.IsInRole("Administrator") || User.Identity.GetUserId() == articleSuggestion.UserId)
            {
                ViewBag.ShowAdditionalButtons = "Yes";
            }
            if (User.IsInRole("Administrator") || User.IsInRole("Editor"))
            {
                ViewBag.AR = "Yes";
            }
            ViewBag.UserId = User.Identity.GetUserId();
            if (User.IsInRole("Administrator"))
            {
                ViewBag.IsAdmin = "Yes";
            }
            if (User.IsInRole("User") || User.IsInRole("Editor") || User.IsInRole("Administrator"))
            {
                ViewBag.IsLoggedIn = "Yes";
            }

            return View();
        }

        [MyAuthorize(Roles = "Administrator, Editor, User")]
        public ActionResult New()
        {
            ViewBag.CategoryId = db.Categories;
            return View();
        }

        [HttpPost]
        [MyAuthorize(Roles = "Administrator, Editor, User")]
        public ActionResult New(ArticleSuggestion articleSuggestion)
        {
            articleSuggestion.Date = DateTime.Now;
            articleSuggestion.UserId = User.Identity.GetUserId();
            articleSuggestion.Accepted = -1;
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

        [MyAuthorize(Roles = "Administrator, Editor, User")]
        public ActionResult Edit(int id)
        {
            ArticleSuggestion articleSuggestion = db.ArticleSuggestions.Find(id);
            if (articleSuggestion.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                ViewBag.ArticleSuggestion = articleSuggestion;
                return View();
            }
            else
            {
                return View("Unauthorized");
            }
        }

        [HttpPut]
        [MyAuthorize(Roles = "Administrator, Editor, User")]
        public ActionResult Edit(int id, ArticleSuggestion requestArticleSuggestion)
        {
            if (requestArticleSuggestion.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                try
                {
                    ArticleSuggestion articleSuggestion = db.ArticleSuggestions.Find(id);
                    if (TryUpdateModel(articleSuggestion))
                    {
                        articleSuggestion.Title = requestArticleSuggestion.Title;
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
            else
            {
                return View("Unauthorized");
            }
            
        }

        [MyAuthorize(Roles = "Administrator, Editor, User")]
        public ActionResult Delete(int id)
        {
            ArticleSuggestion articleSuggestion = db.ArticleSuggestions.Find(id);
            if (articleSuggestion.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                db.ArticleSuggestions.Remove(articleSuggestion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Unauthorized");
        }

        [MyAuthorize(Roles = "Administrator, Editor, User")]
        public ActionResult AddCategory(int id)
        {
            ArticleSuggestion articleSuggestion = db.ArticleSuggestions.Find(id);
            if (articleSuggestion.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                ViewBag.ArticleSuggestionId = id;
                ViewBag.CategoryId = db.Categories;
                return View();
            }
            else
            {
                return View("Unauthorized");
            }
            
        }

        [HttpPut]
        [MyAuthorize(Roles = "Administrator, Editor, User")]
        public ActionResult AddCategory(int id, int CategoryId)
        {
            ArticleSuggestion articleSuggestion = db.ArticleSuggestions.Find(id);
            if (articleSuggestion.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                try
                {
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
            else
            {
                return View("Unauthorized");
            }
            
        }

        [MyAuthorize(Roles = "Administrator, Editor, User")]
        public ActionResult AddComment(int id)
        {
            if (User.IsInRole("User") || User.IsInRole("Editor") || User.IsInRole("Administrator"))
            {
                ViewBag.ArticleSuggestionId = id;
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
            else
            {
                return RedirectToAction("Login", "AccountController");
            }
            
        }

        [MyAuthorize(Roles = "Administrator, Editor")]
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

        [MyAuthorize(Roles = "Administrator, Editor")]
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