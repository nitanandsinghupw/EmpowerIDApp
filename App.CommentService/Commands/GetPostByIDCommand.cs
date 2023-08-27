using MediatR;

namespace App.CommentService.Commands
{
    public class GetPostByIDCommand : IRequest<App.Entity.Database.BlogPost>
    {
        public int Id { get; set; }   
    }
}
