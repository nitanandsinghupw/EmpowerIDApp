using App.DataAccess.BlogDbContext;
using App.Entity;
using App.Entity.Database;
using App.Utility;
using AutoMapper;
using BlogAPI;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.CommentService.Controllers
{
    [ApiController]
    [Route("api/comment")]
    public class CommentController : ControllerBase
    {
        private readonly BlogDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IRedisCache _redisCache;
        private readonly IMediator _mediator;

        public CommentController(BlogDbContext dbContext, IMapper mapper, IRedisCache redisCache, IMediator mediator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _redisCache = redisCache;
            _mediator = mediator;
        }

        private IActionResult ApiResponse<T>(T data)
        {
            return Ok(new ApiResponse<T> { Success = true, Data = data });
        }

        [HttpGet("comment/{id}")]
        public IActionResult GetCommentById(int id)
        {
            var cacheData = _redisCache.GetCacheData<Comment>("Comment-" + id);
            if (cacheData != null)
            {
                return ApiResponse(cacheData);
            }

            var comment = _dbContext.Comments.FirstOrDefault(c => c.Id == id);

            if (comment == null)
            {
                return NotFound(new ApiResponse<string> { Success = false, Message = "Comment not found" });
            }

            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            _redisCache.SetCacheData("Comment-" + id, comment, expirationTime);
            return ApiResponse(comment);
        }

        [HttpGet("post/{id}/comments")]
        public IActionResult GetCommentByPostId(int id)
        {
            var cacheData = _redisCache.GetCacheData<List<Comment>>("Comments-Post-" + id);
            if (cacheData != null)
            {
                return ApiResponse(cacheData);
            }

            var comments = _dbContext.Comments.Where(c => c.PostId == id).ToList();

            if (comments.Count == 0)
            {
                return ApiResponse(new List<Comment>());
            }

            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            _redisCache.SetCacheData("Comments-Post-" + id, comments, expirationTime);
            return ApiResponse(comments);
        }


        [HttpPost]
        public IActionResult CreateComment(CommentModel post)
        {
            var entityBlogComments = _mapper.Map<Comment>(post);

            _dbContext.Comments.Add(entityBlogComments);
            _dbContext.SaveChanges();

            return  ApiResponse(entityBlogComments);
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

        [HttpDelete("comment/{id}")]
        public IActionResult DeleteComment(int id)
        {
            var comment = _dbContext.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return NotFound(new ApiResponse<string> { Success = false, Message = "Comment not found" });
            }

            _dbContext.Comments.Remove(comment);
            _dbContext.SaveChanges();
            _redisCache.RemoveData("Comment-" + id);

            return Ok(new ApiResponse<string> { Success = true, Message = "Comment deleted successfully" });
        }

        [HttpDelete("post/{postId}/comment")]
        public IActionResult DeletePostComment(int postId)
        {
            var comments = _dbContext.Comments.Where(c => c.PostId == postId).ToList();
            if (comments.Count == 0)
            {
                return NotFound(new ApiResponse<string> { Success = false, Message = "No comments found for the post" });
            }

            _dbContext.Comments.RemoveRange(comments);
            _dbContext.SaveChanges();
            _redisCache.RemoveData("Comments-Post-" + postId);

            return Ok(new ApiResponse<string> { Success = true, Message = "Comments for the post deleted successfully" });
        }

    }
}