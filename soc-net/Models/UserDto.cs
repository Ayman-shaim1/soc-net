﻿using System;
namespace soc_net.Models
{
	public class UserDto
	{
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Image { get; set; }

        public UserDto()
		{
		}
	}
}

