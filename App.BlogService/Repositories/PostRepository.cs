using App.Entity.Service;
using Microsoft.EntityFrameworkCore;
using App.DataAccess.BlogDbContext;
using App.Utility;
using AutoMapper;
using App.Entity.Interface;

namespace App.BlogService.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly BlogDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IRedisCache _redisCache;
        private readonly ICommentService _commentService;
        public PostRepository(BlogDbContext blogDbContext, IMapper mapper, IRedisCache redisCache, ICommentService commentService)
        {
            _dbContext = blogDbContext;
            _mapper = mapper;
            _redisCache = redisCache;
            _commentService = commentService;
        } 
        public async Task<List<Entity.Database.BlogPost>> GetPostsAsync()
        {
            List<App.Entity.Database.BlogPost> blogPosts = new List<App.Entity.Database.BlogPost>();
            var cacheData = _redisCache.GetCacheData<List<App.Entity.Database.BlogPost>>("posts");
            if (cacheData != null)
            {
                blogPosts = cacheData;
            }

            blogPosts =await _dbContext.Posts.ToListAsync();
            // Fetch comments for each blog post and map them
            foreach (var post in blogPosts)
            {
                post.Comments = _commentService.GetPostComments(post.Id).Result;
            }

            if (blogPosts != null)
            {
                var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                _redisCache.SetCacheData("posts", blogPosts, expirationTime);
                
            }

            return blogPosts;
        }

        public async Task<Entity.Database.BlogPost> GetPostByIdAsync(int id)
        {
            App.Entity.Database.BlogPost blogPost = new App.Entity.Database.BlogPost();
            var cacheData = _redisCache.GetCacheData<App.Entity.Database.BlogPost>("blogPost-" + id);
            if (cacheData != null)
            {
                blogPost = cacheData;
            }

            blogPost = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);

            if (blogPost != null)
            {
                var comment = _commentService.GetPostComments(id);
                blogPost.Comments = comment.Result.ToList();
                var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                _redisCache.SetCacheData<App.Entity.Database.BlogPost>("blogPost-" + id, blogPost, expirationTime);                
            }

            return blogPost;
        }

        public async Task<Entity.Database.BlogPost> CreatePostAsync(Entity.Service.BlogPost post)
        {
            var entityBlogPost = _mapper.Map<App.Entity.Database.BlogPost>(post);

            await _dbContext.Posts.AddAsync(entityBlogPost);
            await _dbContext.SaveChangesAsync();

            _redisCache.RemoveData("posts");
            return entityBlogPost;
        }

        public async Task<bool> UpdatePostAsync(int id, Entity.Database.BlogPost  blogPost)
        {
            var existingPost = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (existingPost == null)
            {
                return false;
            }

            existingPost.Title = blogPost.Title;
            existingPost.Content = blogPost.Content;

            await _dbContext.SaveChangesAsync();
            _redisCache.RemoveData("posts");
            return true;
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return false;
            }

            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();

            //remove comments with post

            var comment = _commentService.DeletePostComment(id);

            _redisCache.RemoveData("posts");
            return true;
        }
    }
}
