﻿using EngineDeStiri.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace EngineDeStiri.Controllers
{
    public class CommentController : Controller
    {

        private ArticleDBContext db = new ArticleDBContext();

        public ActionResult Index()
        {
            var comments = from comment in db.Comments
                           orderby comment.Title
                           select comment;
            ViewBag.Comments = comments;
            return View();
        }

        public ActionResult Show(int id)
        {
            Comment comment = db.Comments.Find(id);
            ViewBag.Comment = comment;
            return View();
        }

        public ActionResult Edit(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (User.Identity.GetUserId() == comment.AuthorId || User.IsInRole("Administrator"))
            {
                ViewBag.Comment = comment;
                return View();
            }
            else
            {
                return RedirectToAction("Show", "Article", new { id = comment.Article.ArticleId });
            }
        }

        [HttpPut]
        public ActionResult Edit(int id, Comment requestComment)
        {
            Comment comment = db.Comments.Find(id);
            try
            {
                if (TryUpdateModel(comment))
                {
                    comment.Title = requestComment.Title;
                    comment.Date = requestComment.Date;
                    comment.Content = requestComment.Content;
                    db.SaveChanges();
                }
                return RedirectToAction("Show", "Article", new { id = comment.Article.ArticleId });
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Comment comment = db.Comments.Find(id);
            int ArticleId = comment.Article.ArticleId;
            if (User.Identity.GetUserId() == comment.AuthorId || User.IsInRole("Administrator"))
            {
                db.Comments.Remove(comment);
                db.SaveChanges();
            }

            return RedirectToAction("Show", "Article", new { id = ArticleId });
        }
    }
}