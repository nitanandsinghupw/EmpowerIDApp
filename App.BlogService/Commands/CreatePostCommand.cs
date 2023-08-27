using MediatR;

namespace App.BlogService.Commands
{
    public class CreatePostCommand : IRequest<Entity.Database.BlogPost>
    {
        public string? Title { get; set; }
        public string? Content { get; set; }

        public CreatePostCommand(string? title, string? content)
        {
            Title = title;
            Content = content;
        }
    }    
}
