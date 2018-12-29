using EngineDeStiri.Models;
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
            return View();
        }

        
        public ActionResult Show(int id, string SortingOption, string SearchData)
        {

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
                cat = cat.Where(art => art.Title.ToUpper().Contains(SearchData.ToUpper())
                    || art.Author.ToUpper().Contains(SearchData.ToUpper())
                    || art.Content.ToUpper().Contains(SearchData.ToUpper())).ToList();
            }

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
        
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
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

        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            ViewBag.Category = category;
            return View();
        }

        [HttpPut]
        public ActionResult Edit(int id, Category requestCategory)
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

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}