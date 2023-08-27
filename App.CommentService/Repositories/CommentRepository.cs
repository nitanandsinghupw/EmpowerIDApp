using App.DataAccess.BlogDbContext;
using App.Utility;
using AutoMapper;
using App.Entity.Database;

namespace App.CommentService.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly BlogDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IRedisCache _redisCache;        
        public CommentRepository(BlogDbContext blogDbContext, IMapper mapper, IRedisCache redisCache)
        {
            _dbContext = blogDbContext;
            _mapper = mapper;
            _redisCache = redisCache;            
       }

        public async Task<Comment> CreateComment(Comment post)
        {
            var entityBlogComments = _mapper.Map<Comment>(post);

            _dbContext.Comments.Add(entityBlogComments);
            await _dbContext.SaveChangesAsync();

            return entityBlogComments;
        }

        public async Task<bool> DeleteComment(int id)
        {
            var comment = _dbContext.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return false;
            }

            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();
            _redisCache.RemoveData("Comment-" + id);

            return true;
        }

        public async Task<bool> DeletePostComment(int postId)
        {
            var comments = _dbContext.Comments.Where(c => c.PostId == postId).ToList();
            if (comments.Count == 0)
            {
                return false;
            }

            _dbContext.Comments.RemoveRange(comments);
            await _dbContext.SaveChangesAsync();
           _redisCache.RemoveData("Comments-Post-" + postId);

            return true;
        }

        public async Task<Comment> GetCommentById(int id)
        {
            var cacheData = _redisCache.GetCacheData<Comment>("Comment-" + id);
            if (cacheData != null)
            {
                return cacheData;
            }

            var comment = _dbContext.Comments.FirstOrDefault(c => c.Id == id);

            if (comment == null)
            {
                return new Comment();
            }

            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            _redisCache.SetCacheData("Comment-" + id, comment, expirationTime);
            return comment;
        }

        public async Task<List<Comment>> GetCommentByPostId(int id)
        {
            var cacheData = _redisCache.GetCacheData<List<Comment>>("Comments-Post-" + id);
            if (cacheData != null)
            {
                return cacheData;
            }

            var comments = _dbContext.Comments.Where(c => c.PostId == id).ToList();

            if (comments.Count == 0)
            {
                return new List<Comment>();
            }

            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            _redisCache.SetCacheData("Comments-Post-" + id, comments, expirationTime);
            return comments;
        }

        public async Task<bool> UpdateComment(int id, Comment updatedComment)
        {
            var existingPost = _dbContext.Comments.FirstOrDefault(p => p.PostId == id);
            if (existingPost == null)
            {
                return false;
            }


            existingPost.Text = updatedComment.Text;

            await _dbContext.SaveChangesAsync();
            _redisCache.RemoveData("Comment-" + id);
            return true;
        }
    }
}
