using App.CommentService.Commands;
using App.CommentService.Repositories;
using MediatR;

namespace App.CommentService.Handlers
{
    public class GetCommentByPostIdHandler : IRequestHandler<GetCommentByPostIdCommand, List<Entity.Database.Comment>>
    {
        private readonly ICommentRepository _commentRepository;
        public GetCommentByPostIdHandler(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public async Task<List<Entity.Database.Comment>> Handle(GetCommentByPostIdCommand command, CancellationToken cancellationToken)
        {
            return await _commentRepository.GetCommentByPostId(command.PostID);
        }
    }
}
