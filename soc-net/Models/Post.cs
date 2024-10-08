﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace soc_net.Models
{
	public class Post
	{
        [Key]
        public int Id { get; set; }
        public string Textcontent { get; set; }
		public DateTime date { get; set; }
        public int userId { get; set; }

      
        public List<PostLike> Likes { get; set; }
        public List<Comment> Comments { get; set; }

        [ForeignKey("userId")]
        public User user { get; set; }

       

        public Post()
		{
		}
	}
}

