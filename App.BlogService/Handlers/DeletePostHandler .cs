using App.BlogService.Commands;
using App.BlogService.Repositories;
using App.Entity;
using MediatR;

namespace App.BlogService.Handlers
{
    public class DeletePostHandler : IRequestHandler<DeletePostCommand, ApiResponse<string>>
    {
        private readonly IPostRepository _postRepository;
        public DeletePostHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }     

        public async Task<ApiResponse<string>> Handle(DeletePostCommand command, CancellationToken cancellationToken)
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
