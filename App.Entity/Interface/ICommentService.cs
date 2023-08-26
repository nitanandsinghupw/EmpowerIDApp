using App.Entity.Database;

namespace App.Entity.Interface
{
    public interface ICommentService
    {
        public Task<List<Comment>> GetPostComments(int postId);
        Task<Comment> CreateComment(Comment comment);
        Task UpdateComment(int id, Comment updatedComment);
        Task DeletePostComment(int postId);
    }
}