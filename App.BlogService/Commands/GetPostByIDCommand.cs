using MediatR;

namespace App.BlogService.Commands
{
    public class GetPostByIDCommand : IRequest<App.Entity.Database.BlogPost>
    {
        public int Id { get; set; }   
    }
}
