using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EngineDeStiri.Models
{
    public class Category
    {
        /*
        public Category()
        {
            this.Articles = new Collection<Article>();
        }
        */

        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
    }
}