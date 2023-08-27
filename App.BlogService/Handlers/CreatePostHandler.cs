using App.BlogService.Commands;
using App.BlogService.Repositories;
using App.Entity.Database;
using MediatR;

namespace App.BlogService.Handlers
{
    public class CreatePostHandler : IRequestHandler<CreatePostCommand, BlogPost>
    {
        private readonly IPostRepository _postRepository;
        public CreatePostHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<BlogPost> Handle(CreatePostCommand command, CancellationToken cancellationToken)
        {
            var blogPost = new BlogPost()
            {
                Title = command.Title,
                Content = command.Content                
            };

            return await _postRepository.CreatePostAsync(blogPost);
        }
    }
}
