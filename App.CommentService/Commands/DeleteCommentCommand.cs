using MediatR;

namespace App.CommentService.Commands
{
    public class DeleteCommentCommand : IRequest<Entity.ApiResponse<string>>
    {
        public int CommentID { get; set; }        

        public DeleteCommentCommand(int ID)
        {
            CommentID = ID;
        }
    }    
}
