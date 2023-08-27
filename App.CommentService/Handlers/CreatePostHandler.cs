using App.BlogService.Commands;
using App.CommentService.Repositories;
using App.Entity.Database;
using MediatR;

namespace App.CommentService.Handlers
{
    public class CreatePostHandler : IRequestHandler<CreatePostCommand, App.Entity.Database.BlogPost>
    {
        private readonly ICommentRepository _postRepository;
        public CreatePostHandler(ICommentRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<App.Entity.Database.BlogPost> Handle(CreatePostCommand command, CancellationToken cancellationToken)
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
