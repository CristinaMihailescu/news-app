using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EngineDeStiri.Models
{
    public class ArticleSuggestion
    {
        [Key]
        public int ArticleSuggestionId { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        //<!-- MODIFICA AICI !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! -->
        public string UserId { get; set; }
        //<!-- MODIFICA AICI !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! -->
        public string Content { get; set; }
        public string EditorId { get; set; }
        public int Accepted { get; set; } //0 = not accepted, 1 = accepted, -1 = not reviewed

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}