using App.CommentService.Commands;
using App.CommentService.Repositories;
using App.Entity.Database;
using MediatR;

namespace App.CommentService.Handlers
{
    public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, Comment>
    {
        private readonly ICommentRepository _commentRepository;
        public CreateCommentHandler(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public async Task<Comment> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
        {
            var comment = new Comment()
            {
                Id = command.Id,
                Text = command.Text,
                PostId = command.PostId,
                Post = command.Post
            };

            return await _commentRepository.CreateComment(comment);
        }
    }
}
