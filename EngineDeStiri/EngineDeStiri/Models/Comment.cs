using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EngineDeStiri.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }

        public virtual Article Article { get; set; }
        public virtual ArticleSuggestion ArticleSuggestion { get; set; }
    }
}