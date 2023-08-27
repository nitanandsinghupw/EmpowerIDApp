using MediatR;

namespace App.CommentService.Commands
{
    public class GetCommentByPostIdCommand : IRequest<List<Entity.Database.Comment>>
    {
        public int PostID { get; set; }

        public GetCommentByPostIdCommand(int ID )
        {
            PostID = ID;
        }
    }
}
