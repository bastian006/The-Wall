using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models.Models
{
    public class Message
    {
        [Key]
        public int MessageId {get;set;}
        [Required]
        public string Text {get;set;}

        public int UserId{get;set;}
        public User WroteBy {get;set;}
        public List<Comment> Comments {get;set;}
        
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}