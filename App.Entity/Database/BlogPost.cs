using System.ComponentModel.DataAnnotations;

namespace App.Entity.Database
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
     
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string? Text { get; set; }
        public int PostId { get; set; }
        public BlogPost? Post { get; set; }
    }

}
