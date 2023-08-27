using App.Entity.Database;
using MediatR;

namespace App.CommentService.Commands
{
    public class UpdateCommentCommand : IRequest<Entity.ApiResponse<string>>
    {
        public int CommentID { get; set; }
        public Comment Comment { get; set; }

        public UpdateCommentCommand(int ID, Comment comment)
        {
            CommentID = ID;
            Comment= comment;
        }
    }    
}
