namespace App.Entity.Service
{
    public class BlogPost
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
    }

    public class Comment
    {
        public int PostId { get; set; }
        public string? Text { get; set; }

    }

    public class UpdateBlogPost
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
    }

}
