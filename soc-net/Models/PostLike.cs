using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace soc_net.Models
{
	public class PostLike
	{
        [Key]
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }

        [Key]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}

