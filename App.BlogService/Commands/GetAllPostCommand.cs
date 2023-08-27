using MediatR;

namespace App.BlogService.Commands
{
    public class GetAllPostCommand : IRequest<List<App.Entity.Database.BlogPost>>
    {     
        
    }    
}
