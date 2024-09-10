using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace soc_net.Models
{
	public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string textcontent { get; set; }
        public DateTime date { get; set; }
        public int userId { get; set; }
        public int postId { get; set; }


        [JsonIgnore]
        [ForeignKey("userId")]
        public User user { get; set; }

        [JsonIgnore]
        [ForeignKey("postId")]
        public Post post { get; set; }

        public Comment() {}
	}
}

