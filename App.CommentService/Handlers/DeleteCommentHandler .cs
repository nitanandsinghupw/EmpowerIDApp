using App.CommentService.Commands;
using App.CommentService.Repositories;
using App.Entity;
using MediatR;

namespace App.CommentService.Handlers
{
    public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, ApiResponse<string>>
    {
        private readonly ICommentRepository _commentRepository;
        public DeleteCommentHandler(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }     

        public async Task<ApiResponse<string>> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
        {      
            var result = await _commentRepository.DeleteComment(command.CommentID);

            if (result)
            {
                return new ApiResponse<string> { Success = true, Message = "comment deleted successfully" };
            }
            return new ApiResponse<string> { Success = false, Message = "comment not found" };
        }
    }
}
