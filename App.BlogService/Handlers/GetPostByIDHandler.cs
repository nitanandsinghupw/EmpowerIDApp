using App.BlogService.Commands;
using App.BlogService.Repositories;
using MediatR;

namespace App.BlogService.Handlers
{
    public class GetPostByIDHandler : IRequestHandler<GetPostByIDCommand, Entity.Database.BlogPost>
    {
        private readonly IPostRepository _postRepository;
        public GetPostByIDHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<Entity.Database.BlogPost> Handle(GetPostByIDCommand command, CancellationToken cancellationToken)
        {           
            return await _postRepository.GetPostByIdAsync(command.Id);
        }
    }
}
