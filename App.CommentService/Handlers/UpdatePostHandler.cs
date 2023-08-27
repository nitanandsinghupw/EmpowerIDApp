using App.BlogService.Commands;
using App.CommentService.Repositories;
using App.Entity;
using MediatR;

namespace App.CommentService.Handlers
{
    public class UpdatePostHandler : IRequestHandler<UpdatePostCommand, Entity.ApiResponse<string>>
    {
        private readonly ICommentRepository _postRepository;
        public UpdatePostHandler(ICommentRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<Entity.ApiResponse<string>> Handle(UpdatePostCommand command, CancellationToken cancellationToken)
        {
            var result = await _postRepository.UpdatePostAsync(command.Id, command.Post);
            if (result == null)
            {
                return new ApiResponse<string> { Success = true, Message = "Post updated successfully" };
            }
            return new ApiResponse<string> { Success = false, Message = "Post not found" };          
        }
    }
}
