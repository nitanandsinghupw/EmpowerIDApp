using App.DataAccess.BlogDbContext;
using App.Entity;
using App.Entity.Interface;
using App.Entity.Service;
using AutoMapper;
using AzureRedisCacheDemo.Repositories.AzureRedisCache;
using Microsoft.AspNetCore.Mvc;
using BlogPost = App.Entity.Database.BlogPost;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class BlogPostController : ControllerBase
    {
        private readonly BlogDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IRedisCache _redisCache;
        private readonly ICommentService _commentService;

        public BlogPostController(BlogDbContext dbContext, IMapper mapper, IRedisCache redisCache, ICommentService  commentService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _redisCache = redisCache;
            _commentService = commentService;
        }

        private IActionResult ApiResponse<T>(T data)
        {
            return Ok(new ApiResponse<T> { Success = true, Data = data });
        }

        [HttpGet]
        public  IActionResult GetPosts()
        {             
            List<BlogPost> blogPosts = new List<BlogPost>();
            var cacheData = _redisCache.GetCacheData<List<BlogPost>>("posts");
            if (cacheData != null)
            {
                blogPosts = cacheData;
            }
            if (blogPosts != null)
            {
                var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                _redisCache.SetCacheData("posts", blogPosts, expirationTime);
                return Ok(blogPosts);
            }
            else
            {
                return NoContent();
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetPostById(int id)
        {
            BlogPost blogPost = new BlogPost();
            var cacheData = _redisCache.GetCacheData<BlogPost>("blogPost-"+ id);
            if (cacheData != null)
            {
                blogPost = cacheData;
            }

            var comment = _commentService.GetPostComments(id);

            if (blogPost == null)
            {
                return NotFound(new ApiResponse<string> { Success = false, Message = "Post not found" });
            }
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            _redisCache.SetCacheData<BlogPost>("blogPost-" + id, blogPost, expirationTime);
            return ApiResponse(blogPost);
        }

        [HttpPost]
        public IActionResult CreatePost(BlogPost post)
        {
            var entityBlogPost = _mapper.Map<BlogPost>(post);

            _dbContext.Posts.Add(entityBlogPost);
            _dbContext.SaveChanges();

            _redisCache.RemoveData("posts");
            return CreatedAtAction(nameof(GetPostById), new { id = entityBlogPost.Id }, ApiResponse(entityBlogPost));
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePost(int id, UpdateBlogPost updatedPost)
        {
            var existingPost = _dbContext.Posts.FirstOrDefault(p => p.Id == id);
            if (existingPost == null)
            {
                return NotFound(new ApiResponse<string> { Success = false, Message = "Post not found" });
            }

            existingPost.Title = updatedPost.Title;
            existingPost.Content = updatedPost.Content;

            _dbContext.SaveChanges();
            _redisCache.RemoveData("posts");
            return Ok(new ApiResponse<string> { Success = true, Message = "Post updated successfully" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePost(int id)
        {
            var post = _dbContext.Posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound(new ApiResponse<string> { Success = false, Message = "Post not found" });
            }

            _dbContext.Posts.Remove(post);

            var comments = _dbContext.Comments.FirstOrDefault(p => p.PostId == id);
            if (comments != null)
            {
                _dbContext.Comments.Remove(comments);
            }
            _dbContext.SaveChanges();
            _redisCache.RemoveData("posts");
            return Ok(new ApiResponse<string> { Success = true, Message = "Post deleted successfully" });
        }
    }
}
