namespace App.BlogService.Repositories
{
    public interface IPostRepository
    {
        public Task<List<Entity.Database.BlogPost>> GetPostsAsync();
        public Task<Entity.Database.BlogPost> GetPostByIdAsync(int id);
        public Task<Entity.Database.BlogPost> CreatePostAsync(Entity.Database.BlogPost post);
        public Task<bool> UpdatePostAsync(int id, Entity.Database.BlogPost blogPost);
        public Task<bool> DeletePostAsync(int id);        
    }
}
