using System.Security.Claims;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;
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
        public async Task<IActionResult> Get()
        {
            var posts = await _context.Posts
                        .Include(p => p.user)
                            .Include(p => p.Comments)
                            .Include(p => p.Likes)
                       
                            .Select(p => new
                            {
                                Id = p.Id,
                                Textcontent = p.Textcontent,
                                Date = p.date,
                                User = new
                                {
                                    Id = p.user.Id,
                                    Name = p.user.Name,
                                    Image = p.user.Image
                                },
                                Comments = p.Comments.Select(c => new
                                {
                                    Id = c.Id,
                                    Textcontent = c.textcontent,
                                    Date = c.date,
                                }).ToArray(),
                                Likes = p.Likes.Select(l => new
                                {
                                    userId = l.UserId,
                                }).ToArray()
                            })
                            .ToListAsync();

            return Ok(posts);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _context.Posts
                  .Include(p => p.user)
                      .Include(p => p.Comments)
                      .Include(p => p.Likes)
                      .Where(p => p.Id == id)
                      .Select(p => new
                      {
                          Id = p.Id,
                          Textcontent = p.Textcontent,
                          Date = p.date,
                          User = new
                          {
                              Id = p.user.Id,
                              Name = p.user.Name,
                              Image = p.user.Image
                          },
                          Comments = p.Comments.Select(c => new
                          {
                              Id = c.Id,
                              Textcontent = c.textcontent,
                              Date = c.date,
                          }).ToArray(),
                          Likes = p.Likes.Select(l => new
                          {
                              userId = l.UserId,
                          }).ToArray()
                      })
                      .FirstOrDefaultAsync();

            return Ok(post);
        }

        [HttpPut("{id}/toggle/like")]
        public async Task<IActionResult> ToggleLike(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var findPost = await _context.Posts.Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id == id);

            if (findPost == null)
                return NotFound(new { message = "Post not found!" });

            var findPostLike = findPost.Likes.FirstOrDefault(l => l.PostId == id && l.UserId == userId);

            if (findPostLike == null)
            {
                var postLikeToAdd = new PostLike
                {
                    UserId = userId,
                    PostId = findPost.Id
                };

                findPost.Likes.Add(postLikeToAdd);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Post liked successfully!" });
            }
            else
            {
                findPost.Likes.Remove(findPostLike);
                _context.PostLikes.Remove(findPostLike);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Post unliked successfully!" });
            }
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

        [HttpPost("comments/{id}")]
        public async Task<IActionResult> comment(int id, [FromBody] CommentDto comment)
        {
            // Récupérer l'ID à partir des claims du token JWT
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;

            Post findPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

            if (findPost == null)
                return NotFound(new { message = "post not found !" });

            else
            {
                Comment commentToAdd = new()
                {
                    textcontent = comment.textContent,
                    date = DateTime.Now,
                    postId = findPost.Id,
                    userId = int.Parse(userId),
                };

 
                _context.Comments.Add(commentToAdd);
                await _context.SaveChangesAsync();

                return Ok(commentToAdd);

            }
        }



        [HttpDelete("{idPost}/comments/{idComment}")]
        public async Task<IActionResult> deletecomment(int idPost, int idComment)
        {
            // Récupérer l'ID à partir des claims du token JWT
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;

            Post findPost = await _context.Posts.FirstOrDefaultAsync(p => p.Id == idPost);

            if (findPost == null)
                return NotFound(new { message = "post not found !" });

            else
            {
                Comment findComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == idComment);

                if (findComment == null)
                    return NotFound(new { message = "comment not found !" });

                if(findComment.userId != int.Parse(userId))
                    return NotFound(new { message = "you are not the user how add this comment !" });


                _context.Comments.Remove(findComment);
                await _context.SaveChangesAsync();

                return Ok(new { message = "comment delete successfully !" });


            }
        }


    }
}

