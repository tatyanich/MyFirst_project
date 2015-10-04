//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Blog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Post
    {
        public Post()
        {
            this.Comments = new HashSet<Comment>();
            this.Tags = new HashSet<Tag>();
        }
    
        public int Id { get; set; }
            [Required(ErrorMessage = "Position title is required.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Position datetime is required.")]
        public System.DateTime DateTime { get; set; }
        [Required(ErrorMessage = "Position body is required.")]
        public string Body { get; set; }
    
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
