using App.CommentService.Commands;
using App.CommentService.Repositories;
using App.Entity;
using MediatR;

namespace App.CommentService.Handlers
{
    public class UpdateCommentHandler : IRequestHandler<UpdateCommentCommand, ApiResponse<string>>
    {
        private readonly ICommentRepository _commentRepository;
        public UpdateCommentHandler(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public async Task<ApiResponse<string>> Handle(UpdateCommentCommand command, CancellationToken cancellationToken)
        {
            var result = await _commentRepository.UpdateComment(command.CommentID, command.Comment);
            if (result)
            {
                return new ApiResponse<string> { Success = true, Message = "comment updated successfully" };
            }
            return new ApiResponse<string> { Success = false, Message = "comment not found" };          
        }
    }
}
