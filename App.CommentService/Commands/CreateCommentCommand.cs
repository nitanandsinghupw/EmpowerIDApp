using App.Entity.Database;
using MediatR;

namespace App.CommentService.Commands
{
    public class CreateCommentCommand : IRequest<Comment>
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public int PostId { get; set; }
        public BlogPost? Post { get; set; }

        public CreateCommentCommand(int id, string? text, int postId, BlogPost? post)
        {
            Id = id;
            Text = text;
            PostId = postId;
            Post = post;
        }
    }    
}
