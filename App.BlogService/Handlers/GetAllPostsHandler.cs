using App.BlogService.Commands;
using App.BlogService.Repositories;
using MediatR;

namespace App.BlogService.Handlers
{
    public class GetAllPostsHandler : IRequestHandler<GetAllPostCommand, List<App.Entity.Database.BlogPost>>
    {
        private readonly IPostRepository _postRepository;
        public GetAllPostsHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }       

        public Task<List<Entity.Database.BlogPost>> Handle(GetAllPostCommand command, CancellationToken cancellationToken)
        {
            return _postRepository.GetPostsAsync();
        }
    }
}
