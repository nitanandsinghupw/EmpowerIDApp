using App.Entity.Database;
using MediatR;

namespace App.BlogService.Commands
{
    public class UpdatePostCommand : IRequest<Entity.ApiResponse<string>>
    {
        public int Id { get; set; }
        public BlogPost Post { get; set; }

        public UpdatePostCommand(int ID, BlogPost post)
        {
            Id= ID;
            Post= post;
        }
    }    
}
