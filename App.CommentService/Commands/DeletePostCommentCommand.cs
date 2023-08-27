using MediatR;

namespace App.CommentService.Commands
{
    public class DeletePostCommentCommand : IRequest<Entity.ApiResponse<string>>
    {
        public int PostId { get; set; }

        public DeletePostCommentCommand(int postId)
        {
            PostId = postId;
        }
    }    
}
