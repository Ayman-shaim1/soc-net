using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using soc_net.Models;


namespace soc_net.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly SocNetContext _context;

        public PostController(SocNetContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "hello world" });
        }

        [HttpPost]
        public async Task<IActionResult> Add(PostDto post)
        {
            // Récupérer l'ID à partir des claims du token JWT
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            Post postToAdd = new()
            {
                Textcontent = post.Textcontent,
                date = DateTime.Now,
                userId = int.Parse(userId),
            };

            _context.Posts.Add(postToAdd);
            await _context.SaveChangesAsync();

            return Ok(postToAdd);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            // Récupérer l'ID à partir des claims du token JWT
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;

            Post findPost = _context.Posts.FirstOrDefault(p => p.Id == id);

            if (findPost == null)
                return NotFound(new { message = "post not found !" });


            if (findPost.userId == int.Parse(userId))
            {
                _context.Posts.Remove(findPost);
                await _context.SaveChangesAsync();

                return Ok(new { message = "post deleted successfully !" });
            }
            else

                return BadRequest(new { message = "You are not the user who posted this post!" });
        }


    }
}

