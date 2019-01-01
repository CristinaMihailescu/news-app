using EngineDeStiri.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(EngineDeStiri.Startup))]
namespace EngineDeStiri
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createAdminUserAndApplicationRoles();
        }

        private void createAdminUserAndApplicationRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            // Se adauga rolurile aplicatiei
            if (!roleManager.RoleExists("Administrator"))
            {
                // Se adauga rolul de administrator
                var role = new IdentityRole();
                role.Name = "Administrator";
                roleManager.Create(role);
                // se adauga utilizatorul administrator
                var user = new ApplicationUser();
                user.UserName = "admin@admin.com";
                user.Email = "admin@admin.com";
                var adminCreated = UserManager.Create(user, "P@rola1");
                if (adminCreated.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Administrator");
                }

                ArticleDBContext db = new ArticleDBContext();

                //adding default categories
                Category cat = new Category();
                var categories = from category in db.Categories
                                 orderby category.Name
                                 select category;

                string[] catString = { "World", "Business", "Technology",
                                   "Entertainment", "Sports", "Science",
                                   "Health" };
                foreach (var name in catString)
                {
                    var found = (from x in categories.OfType<Category>() where x.Name == name select x).FirstOrDefault();
                    if (found == null)
                    {
                        cat.Name = name;
                        db.Categories.Add(cat);
                        db.SaveChanges();
                    }
                }

                //adding some random articles
                Article art = new Article();
                var articles = from article in db.Articles
                               orderby article.Title
                               select article;
                var admin = UserManager.FindByName("admin@admin.com");

                //first
                art.Title = "Out with a Bang! New Year's Eve fireworks from around the world";
                art.Date = DateTime.Now;
                art.Author = user.Id;
                art.Username = user.UserName;
                art.Thumbnail = "https://cdn.images.express.co.uk/img/dynamic/1/590x/New-Years-Eve-fireworks-1065509.jpg?r=1546273653627";
                art.Headline = "Amazing!!";
                art.URL = "https://cdn.images.express.co.uk/img/dynamic/1/590x/New-Years-Eve-fireworks-1065509.jpg?r=1546273653627";

                db.Articles.Add(art);
                db.SaveChanges();

                //second
                art = new Article();
                art.Title = "North Korea's Kim Warns Trump Talks at Risk Over Sanctions";
                art.Date = DateTime.Now;
                art.Author = user.Id;
                art.Username = user.UserName;
                art.Thumbnail = "https://assets.bwbx.io/images/users/iqjWHBFdfxIU/iVQmYqsv.yOQ/v0/-1x-1.jpg";
                art.Headline = "Amazing!!";
                art.Content = "Kim Jong Un used his New Year’s address to issue a pointed warning to President Donald Trump, saying North Korea would take a “new path” in nuclear talks if the U.S. didn’t relax economic sanctions. While Kim affirmed his willingness to meet Trump again, his nationally televised speech offered no new initiatives to advance talks that have sputtered since their first summit in June. Instead, Kim said his patience with the U.S.-led sanctions regime designed as punishment for his nuclear weapons program was running out.";

                db.Articles.Add(art);
                db.SaveChanges();

                //third
                art = new Article();
                art.Title = "Google is primed to go big at CES again";
                art.Date = DateTime.Now;
                art.Author = user.Id;
                art.Username = user.UserName;
                art.Thumbnail = "https://lh3.googleusercontent.com/proxy/ogwfaF0iwa05OnTNQFyD0rZ384sAN74p5xwJE6qfJmrEFcmgxlXo4zg22lrlaLcaS_hp9pFCu8s8QZ-GgDy37DxWVOHpq2B4IV35vb4wgHBWfJiYqI_AVARVMaguPane4Raedg=w530-h212-p";
                art.Headline = "Last year was the search giant's first official appearance in years at the massive tech show. With round two, Google looks to make an even bigger splash.";
                art.Content = "Last year at CES, Google set up a three-story Wonka factory of smart home devices. In a massive booth near the Las Vegas Convention Center, the search giant showed off how its Google Assistant could work with everything from washing machines to miniature train sets. There was a giant, voice-controlled gumball machine full of giveaway devices. A big, blue slide spiraled to the ground. Elsewhere at the conference, white-suited Google workers greeted people in booths across the show floor, and the company plastered the words Hey Google -- one of the trigger phrases for the Google Assistant -- over the Las Vegas Monorail. The message was clear: After years of laying low at the world's largest tech show, Google had finally arrived at CES. The company is primed to go even bigger at this year's show, which starts next week. Before last year's CES, the company had happily stayed on the sidelines and let its manufacturing partners, including Samsung and LG, grab all the attention. Now Google is using the trade show to trumpet its software and hardware, complete with an official stage. That's because CES has become an important staging ground for the tech giant as it pushes devices to consumers that compete against the likes of Amazon, Apple and Samsung. In the next three years, Google's hardware division -- which includes its Google Home smart speakers, Nest thermostats and Chromecast streaming devices -- could hit $20 billion in revenue, RBC Capital Markets said last month.";

                db.Articles.Add(art);
                db.SaveChanges();

                //forth
                art = new Article();
                art.Title = "Queen rocker Brian May releases space song dedicated to Ultima Thule";
                art.Date = DateTime.Now;
                art.Author = user.Id;
                art.Username = user.UserName;
                art.Thumbnail = "https://i.amz.mshcdn.com/X5uLdPupVxHAqh1DMT8lE5faKwQ=/950x534/filters:quality(90)/https%3A%2F%2Fblueprint-api-production.s3.amazonaws.com%2Fuploads%2Fcard%2Fimage%2F910620%2F3dd52838-69ad-4531-b6fd-6e15e729b92d.jpg";
                art.Headline = "Amazing!!";
                art.URL = "https://mashable.com/article/ultima-thule-brian-may-rock-song-new-horizons/?europe=true#ziCYBFKhvqqo";

                db.Articles.Add(art);
                db.SaveChanges();

            }
            if (!roleManager.RoleExists("Editor"))
            {
                var role = new IdentityRole();
                role.Name = "Editor";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole();
                role.Name = "User";
                roleManager.Create(role);
            }
        }
    }
}
