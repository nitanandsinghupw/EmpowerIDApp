using MediatR;

namespace App.BlogService.Commands
{
    public class GetAllPostCommand : IRequest<List<Entity.Database.BlogPost>>
    {     
        
    }    
}
