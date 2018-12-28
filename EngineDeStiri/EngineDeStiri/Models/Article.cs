using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EngineDeStiri.Models
{
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        //<!-- MODIFICA AICI !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! -->
        public string Author { get; set; }
        //<!-- MODIFICA AICI !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! -->
        public string Thumbnail { get; set; }
        public string Content { get; set; }

        public ICollection<Category> Categories { get; set; }
    }

    public class ArticleDBContext : DbContext
    {
        public ArticleDBContext() : base("DBConnectionString") { }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}