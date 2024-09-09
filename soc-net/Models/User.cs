using System;
using System.ComponentModel.DataAnnotations;

namespace soc_net.Models
{
	public class User
	{
        [Key]
        public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string? Image { get; set; }


		public ICollection<Post>? posts { get; set; }

		public User()
		{
		}
	}
}

