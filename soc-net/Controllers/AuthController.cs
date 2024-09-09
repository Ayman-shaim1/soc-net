using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using soc_net.Models;



namespace soc_net.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SocNetContext _context;

        private readonly JwtSettings _jwtSettings;


        public AuthController(SocNetContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto user)
        {
            User findUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if(findUser == null)
            {
                string passwordHach = BCrypt.Net.BCrypt.HashPassword(user.Password);
                User userToAdd = new()
                {
                    Name = user.Name,
                    Email = user.Email,
                    Password = passwordHach
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userToAdd.Id.ToString()) }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                _context.Users.Add(userToAdd);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    name = userToAdd.Name,
                    email = userToAdd.Email,
                    image = userToAdd.Image,
                    token = tokenHandler.WriteToken(token),
                });
            }
            else
            {
                return BadRequest(new { message = "this email already exist !" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto user)
        {

            User findUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);

            if (findUser != null)
            {
                if (BCrypt.Net.BCrypt.Verify(user.Password, findUser.Password))
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, findUser.Id.ToString()) }),
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    return Ok(new {
                        name = findUser.Name,
                        email = findUser.Email,
                        image = findUser.Image,
                        token = tokenHandler.WriteToken(token),
                    });
                }
                else
                    return BadRequest(new { message ="Wrong password !"});
            }
            else
                return BadRequest(new { message = "Wrong email !" });
        }


    }
}

