using App.BlogService.Commands;
using App.CommentService.Repositories;
using MediatR;

namespace App.CommentService.Handlers
{
    public class GetPostByIDHandler : IRequestHandler<GetPostByIDCommand, App.Entity.Database.BlogPost>
    {
        private readonly ICommentRepository _postRepository;
        public GetPostByIDHandler(ICommentRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<App.Entity.Database.BlogPost> Handle(GetPostByIDCommand command, CancellationToken cancellationToken)
        {           
            return await _postRepository.GetPostByIdAsync(command.Id);
        }
    }
}
