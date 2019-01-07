﻿using EngineDeStiri.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EngineDeStiri.Controllers
{
    public class CategoryController : Controller
    {
        private ArticleDBContext db = new ArticleDBContext();

        public ActionResult Index()
        {
            var categories = from category in db.Categories
                           orderby category.Name
                           select category;
            ViewBag.Categories = categories;
            if (User.IsInRole("Administrator"))
            {
                ViewBag.ShowAdditionalButtons = "Yes";
            }
            return View();
        }

        public ActionResult Show(int id, string SortingOption, string SearchData)
        {
            if (User.IsInRole("Administrator"))
            {
                ViewBag.ShowAdditionalButtons = "Yes";
            }
            if (String.IsNullOrEmpty(SortingOption))
            {
                SortingOption = "Name";
            }

            Category category = db.Categories.Find(id);
            ViewBag.Category = category;
            ViewBag.Articles = category.Articles;
            var cat = category.Articles;

            if (!String.IsNullOrEmpty(SearchData)) //IF SearchingData IS NOT empty
            {
                try
                {
                    cat = cat.Where(art => art.Title.ToUpper().Contains(SearchData.ToUpper())
                    || art.Author.ToUpper().Contains(SearchData.ToUpper())
                    || art.Content.ToUpper().Contains(SearchData.ToUpper())).ToList();

                    switch (SortingOption)
                    {
                        case "Date":
                            return View(cat.OrderByDescending(art => art.Date).ToList());
                            break;
                        case "Name":
                            return View(cat.OrderBy(art => art.Title).ToList());
                            break;
                        default:
                            return View(cat.ToList());
                            break;
                    }
                }
                catch (Exception e)
                {
                    //nothing was found
                    cat = new List<Article>();
                    return View(cat);
                }
            }

            return View(cat.ToList());
        }

        [MyAuthorize(Roles = "Administrator")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [MyAuthorize(Roles = "Administrator")]
        public ActionResult New(Category category)
        {
            try
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [MyAuthorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {
            if (id < 8)
            {
                return View("DefaultCategoryDeleteError");
            }
            else
            {
                Category category = db.Categories.Find(id);
                ViewBag.Category = category;
                return View();
            }
        }

        [HttpPut]
        [MyAuthorize(Roles = "Administrator")]
        public ActionResult Edit(int id, Category requestCategory)
        {
            if (id < 8)
            {
                return View("DefaultCategoryDeleteError");
            }
            else
            {
                try
                {
                    Category category = db.Categories.Find(id);
                    if (TryUpdateModel(category))
                    {
                        category.Name = requestCategory.Name;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    return View();
                }
            }
        }

        [MyAuthorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            if (id < 8)
            {
                return View("DefaultCategoryDeleteError");
            }
            else
            {
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}