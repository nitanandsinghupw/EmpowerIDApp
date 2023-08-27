using MediatR;

namespace App.BlogService.Commands
{
    public class DeletePostCommand : IRequest<Entity.ApiResponse<string>>
    {
        public int Id { get; set; }        

        public DeletePostCommand(int ID)
        {
            Id = ID;
        }
    }    
}
