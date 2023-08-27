using App.BlogService.Commands;
using App.CommentService.Repositories;
using MediatR;

namespace App.CommentService.Handlers
{
    public class GetAllPostsHandler : IRequestHandler<GetAllPostCommand, List<App.Entity.Database.BlogPost>>
    {
        private readonly ICommentRepository _postRepository;
        public GetAllPostsHandler(ICommentRepository postRepository)
        {
            _postRepository = postRepository;
        }       

        public Task<List<Entity.Database.BlogPost>> Handle(GetAllPostCommand command, CancellationToken cancellationToken)
        {
            return _postRepository.GetPostsAsync();
        }
    }
}
