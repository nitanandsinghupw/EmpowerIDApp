using App.Entity.Service;

namespace App.Entity.Interface
{
    public interface ICommentService
    {
        public Task<Comment> GetPostComments(int postId);
    }
}