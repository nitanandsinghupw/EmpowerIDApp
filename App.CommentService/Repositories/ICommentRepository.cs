using App.Entity.Database;

namespace App.CommentService.Repositories
{
    public interface ICommentRepository
    {      
        public Task<Comment> GetCommentById(int id);
        public Task<List<Comment>> GetCommentByPostId(int id);
        public Task<Comment> CreateComment(Comment post);
        public Task<bool> UpdateComment(int id, Comment updatedComment);
        public Task<bool> DeleteComment(int id);
        public Task<bool> DeletePostComment(int postId);
    }
}
