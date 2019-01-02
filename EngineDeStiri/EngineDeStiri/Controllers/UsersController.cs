using EngineDeStiri.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EngineDeStiri.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        // GET: Users
        [MyAuthorize(Roles = "Administrator, Editor, User")]
        public ActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;
            ViewBag.UsersList = users;
            return View();
        }

        [MyAuthorize(Roles = "Administrator")]
        public ActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            return View(user);
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();
            var roles = from role in db.Roles select role;
            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }

        [HttpPut]
        [MyAuthorize(Roles = "Administrator")]
        public ActionResult Edit(string id, ApplicationUser newData)
        {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            try
            {
               ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new
               RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new
               UserStore<ApplicationUser>(context));
                System.Diagnostics.Debug.WriteLine("OK1");
                if (TryUpdateModel(user))
                {
                    System.Diagnostics.Debug.WriteLine("OK2");
                    user.UserName = newData.UserName;
                    var roles = from role in db.Roles select role;
                    foreach (var role in roles)
                    {
                        UserManager.RemoveFromRole(id, role.Name);
                    }
                    System.Diagnostics.Debug.WriteLine("OK3");
                    var selectedRole = db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
                    System.Diagnostics.Debug.WriteLine(selectedRole.Name);
                    UserManager.AddToRole(id, selectedRole.Name);
                    System.Diagnostics.Debug.WriteLine("OK4");
                    db.SaveChanges();
                    System.Diagnostics.Debug.WriteLine("OK5");
                }
                Response.Write("Changes saved!");
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
                return View(user);
            }
            

        }

        [MyAuthorize(Roles = "Administrator")]
        public ActionResult Delete(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}