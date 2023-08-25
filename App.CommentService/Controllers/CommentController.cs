using App.DataAccess.BlogDbContext;
using App.Entity;
using App.Entity.Service;
using AutoMapper;
using AzureRedisCacheDemo.Repositories.AzureRedisCache;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace App.CommentService.Controllers
{
    [ApiController]
    [Route("api/comment")]
    public class CommentController : ControllerBase
    {
        private readonly BlogDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IRedisCache _redisCache;

        public CommentController(BlogDbContext dbContext, IMapper mapper, IRedisCache redisCache)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _redisCache = redisCache;
        }

        private IActionResult ApiResponse<T>(T data)
        {
            return Ok(new ApiResponse<T> { Success = true, Data = data });
        }

        [HttpGet("{id}")]
        public IActionResult GetCommentById(int id)
        {
            Comment Comments = new Comment();
            var cacheData = _redisCache.GetCacheData<Comment>("Comment-" + id);
            if (cacheData != null)
            {
                Comments = cacheData;
            }
            Comments = _dbContext.Comments.FirstOrDefault(p => p.PostId == id);

            if (Comments == null)
            {
                return NotFound(new ApiResponse<string> { Success = false, Message = "Post not found" });
            }
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            _redisCache.SetCacheData<Comment>("Comment-" + id, Comments, expirationTime);
            return ApiResponse(Comments);
        }

        [HttpPost]
        public IActionResult CreateComment(Comment post)
        {
            var entityBlogComments = _mapper.Map<Comment>(post);

            _dbContext.Comments.Add(entityBlogComments);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetCommentById), new { id = entityBlogComments.PostId }, ApiResponse(entityBlogComments));
        }

        [HttpPut("{id}")]
        public IActionResult UpdateComment(int id, Comment updatedComment)
        {
            var existingPost = _dbContext.Comments.FirstOrDefault(p => p.PostId == id);
            if (existingPost == null)
            {
                return NotFound(new ApiResponse<string> { Success = false, Message = "Post not found" });
            }


            existingPost.Text = updatedComment.Text;

            _dbContext.SaveChanges();
            _redisCache.RemoveData("Comment-" + id);
            return Ok(new ApiResponse<string> { Success = true, Message = "Comment updated successfully" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteComment(int id)
        {
            var post = _dbContext.Comments.FirstOrDefault(p => p.PostId == id);
            if (post == null)
            {
                return NotFound(new ApiResponse<string> { Success = false, Message = "Post not found" });
            }

            _dbContext.Comments.Remove(post);
            _dbContext.SaveChanges();
            _redisCache.RemoveData("Comment-" + id);
            return Ok(new ApiResponse<string> { Success = true, Message = "Comment deleted successfully" });
        }
    }
}