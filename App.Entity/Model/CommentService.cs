using App.Entity.Interface;
using App.Entity.Service;
using Newtonsoft.Json;

namespace App.Entity
{
    public class CommentService : ICommentService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CommentService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Comment> GetPostComments(int postId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var responseString = await httpClient.GetStringAsync("");

            var comments = JsonConvert.DeserializeObject<Comment>(responseString);
            return comments;
        }
    }
}