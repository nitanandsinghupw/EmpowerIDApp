using App.CommentService.Commands;
using App.CommentService.Repositories;
using MediatR;

namespace App.CommentService.Handlers
{
    public class GetCommentByIdHandler : IRequestHandler<GetCommentByIdCommand, Entity.Database.Comment>
    {
        private readonly ICommentRepository _commentRepository;
        public GetCommentByIdHandler(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public async Task<Entity.Database.Comment> Handle(GetCommentByIdCommand command, CancellationToken cancellationToken)
        {           
            return await _commentRepository.GetCommentById(command.CommentID);
        }
    }
}
