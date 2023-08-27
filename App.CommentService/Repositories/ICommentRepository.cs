namespace App.CommentService.Repositories
{
    public interface ICommentRepository
    {
        public Task<List<App.Entity.Database.BlogPost>> GetPostsAsync();
        public Task<App.Entity.Database.BlogPost> GetPostByIdAsync(int id);
        public Task<App.Entity.Database.BlogPost> CreatePostAsync(App.Entity.Database.BlogPost post);
        public Task<bool> UpdatePostAsync(int id, App.Entity.Database.BlogPost blogPost);
        public Task<bool> DeletePostAsync(int id);        
    }
}
