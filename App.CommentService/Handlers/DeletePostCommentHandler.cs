using App.CommentService.Commands;
using App.CommentService.Repositories;
using App.Entity;
using MediatR;

namespace App.CommentService.Handlers
{
    public class DeletePostCommentHandler : IRequestHandler<DeletePostCommentCommand, Entity.ApiResponse<string>>
    {
        private readonly ICommentRepository _commentRepository;
        public DeletePostCommentHandler(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }       

        public async Task<Entity.ApiResponse<string>> Handle(DeletePostCommentCommand command, CancellationToken cancellationToken)
        {
            var result =await _commentRepository.DeletePostComment(command.PostId);
            if (result)
            {
                return new ApiResponse<string> { Success = true, Message = "Comments for the post deleted successfully" };
            }
            return new ApiResponse<string> { Success = false, Message = "comment not found" };
        }
    }
}
