using MediatR;

namespace App.CommentService.Commands
{
    public class GetAllPostCommand : IRequest<List<App.Entity.Database.BlogPost>>
    {     
        
    }    
}
