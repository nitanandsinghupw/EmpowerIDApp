using App.BlogService.Commands;
using App.DataAccess.BlogDbContext;
using App.Entity;
using App.Entity.Interface;
using App.Entity.Database;
using App.Utility;
using AutoMapper;
using MediatR;
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
        private readonly IMediator _mediator;

        public BlogPostController(BlogDbContext dbContext, IMapper mapper, IRedisCache redisCache, ICommentService  commentService, IMediator mediator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _redisCache = redisCache;
            _commentService = commentService;
            _mediator = mediator;
        }

        private IActionResult ApiResponse<T>(T data)
        {
            return Ok(new ApiResponse<T> { Success = true, Data = data });
        }

        [HttpGet]
        public  async  Task<IActionResult> GetPosts()
        {             
            List<App.Entity.Database.BlogPost> blogPosts = new List<App.Entity.Database.BlogPost>();
            blogPosts = await _mediator.Send(new GetAllPostCommand());

            if (blogPosts != null)
            {              
                return ApiResponse(blogPosts);
            }
            else
            {
                return NoContent();
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            App.Entity.Database.BlogPost blogPost = new App.Entity.Database.BlogPost();
            blogPost =await _mediator.Send(new GetPostByIDCommand() { Id = id }) ;
            return ApiResponse(blogPost);
        }

        [HttpPost]
        public IActionResult CreatePost(App.Entity.Database.BlogPost post)
        {
            var entityBlogPost = _mapper.Map<App.Entity.Database.BlogPost>(post);

            _dbContext.Posts.Add(entityBlogPost);
            _dbContext.SaveChanges();

            _redisCache.RemoveData("posts");
            return CreatedAtAction(nameof(GetPostById), new { id = entityBlogPost.Id }, ApiResponse(entityBlogPost));
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePost(int id, BlogPost BlogPost)
        {
            var existingPost = _dbContext.Posts.FirstOrDefault(p => p.Id == id);
            if (existingPost == null)
            {
                return NotFound(new ApiResponse<string> { Success = false, Message = "Post not found" });
            }

            existingPost.Title = BlogPost.Title;
            existingPost.Content = BlogPost.Content;

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
