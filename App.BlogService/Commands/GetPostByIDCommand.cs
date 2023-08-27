using MediatR;

namespace App.BlogService.Commands
{
    public class GetPostByIDCommand : IRequest<Entity.Database.BlogPost>
    {
        public int Id { get; set; }   
    }
}
