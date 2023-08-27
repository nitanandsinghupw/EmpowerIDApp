using App.BlogService.Commands;
using App.CommentService.Repositories;
using App.Entity;
using MediatR;

namespace App.CommentService.Handlers
{
    public class DeletePostHandler : IRequestHandler<DeletePostCommand, Entity.ApiResponse<string>>
    {
        private readonly ICommentRepository _postRepository;
        public DeletePostHandler(ICommentRepository postRepository)
        {
            _postRepository = postRepository;
        }     

        public async Task<Entity.ApiResponse<string>> Handle(DeletePostCommand command, CancellationToken cancellationToken)
        {      
            var result = await _postRepository.DeletePostAsync(command.Id);

            if (result)
            {
                return new ApiResponse<string> { Success = true, Message = "Post deleted successfully" };
            }
            return new ApiResponse<string> { Success = false, Message = "Post not found" };
        }
    }
}
