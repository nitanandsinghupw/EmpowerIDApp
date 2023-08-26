using App.DataAccess.BlogDbContext;
using App.Entity;
using App.Entity.Interface;
using App.Entity.Service;
using App.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


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
            List<App.Entity.Database.BlogPost> blogPosts = new List<App.Entity.Database.BlogPost>();
            var cacheData = _redisCache.GetCacheData<List<App.Entity.Database.BlogPost>>("posts");
            if (cacheData != null)
            {
                blogPosts = cacheData;
            }

            blogPosts = _dbContext.Posts.ToList();
            // Fetch comments for each blog post and map them
            foreach (var post in blogPosts)
            {
                post.Comments = _commentService.GetPostComments(post.Id).Result;
            }

            if (blogPosts != null)
            {
                var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                _redisCache.SetCacheData("posts", blogPosts, expirationTime);
                return ApiResponse(blogPosts);
            }
            else
            {
                return NoContent();
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetPostById(int id)
        {
            App.Entity.Database.BlogPost blogPost = new App.Entity.Database.BlogPost();
            var cacheData = _redisCache.GetCacheData<App.Entity.Database.BlogPost>("blogPost-"+ id);
            if (cacheData != null)
            {
                blogPost = cacheData;
            }

            blogPost = _dbContext.Posts.FirstOrDefault(p => p.Id == id);            

            if (blogPost == null)
            {
                return NotFound(new ApiResponse<string> { Success = false, Message = "Post not found" });
            }

            var comment = _commentService.GetPostComments(id);


            blogPost.Comments = comment.Result.ToList();
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            _redisCache.SetCacheData<App.Entity.Database.BlogPost>("blogPost-" + id, blogPost, expirationTime);
            return ApiResponse(blogPost);
        }

        [HttpPost]
        public IActionResult CreatePost(App.Entity.Service.BlogPost post)
        {
            var entityBlogPost = _mapper.Map<App.Entity.Database.BlogPost>(post);

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
            _dbContext.SaveChanges();

            //remove comments with post

            var comment = _commentService.DeletePostComment(id);

            _redisCache.RemoveData("posts");
            return Ok(new ApiResponse<string> { Success = true, Message = "Post deleted successfully" });
        }
    }
}
