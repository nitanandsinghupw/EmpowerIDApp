using MediatR;

namespace App.CommentService.Commands
{
    public class GetCommentByIdCommand : IRequest<Entity.Database.Comment>
    {
        public int CommentID { get; set; }
        public GetCommentByIdCommand(int ID)
        {
            CommentID = ID;
        }
    }    
}
