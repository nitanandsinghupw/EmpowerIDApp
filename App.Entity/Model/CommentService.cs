using App.Entity.Interface;
using Newtonsoft.Json;

namespace App.Entity.Database
{
    public class CommentService : ICommentService
    {
        private readonly HttpClient _httpClient;
        public CommentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Comment>> GetPostComments(int postId)
        {
            var apiUrl = $"comment/post/{postId}/comments";

            try
            {
                var responseString = await _httpClient.GetStringAsync(apiUrl);
                
                ApiResponse<List<Comment>> response = JsonConvert.DeserializeObject<ApiResponse<List<Comment>>>(responseString);

                if (!response.Success)
                {
                    // Handle other error scenarios if needed
                    throw new Exception("Error while retrieving comments: " + response.Message);
                }

                List<Comment> comments = response.Data;
                return comments;
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                // Handle the "Not Found" scenario by returning an empty list
                return new List<Comment>();
            }
        } 

        public async Task<Comment> CreateComment(Comment comment)
        {

            var apiUrl = "comment";

            var content = new StringContent(JsonConvert.SerializeObject(comment), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, content);

            response.EnsureSuccessStatusCode(); // Throw an exception if the status code indicates failure

            var responseString = await response.Content.ReadAsStringAsync();
            var createdComment = JsonConvert.DeserializeObject<Comment>(responseString);
            return createdComment;
        }

        public async Task UpdateComment(int id, Comment updatedComment)
        {
            var apiUrl = $"comment/{id}";

            var content = new StringContent(JsonConvert.SerializeObject(updatedComment), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(apiUrl, content);

            response.EnsureSuccessStatusCode(); // Throw an exception if the status code indicates failure
        }

        public async Task DeletePostComment(int postId)
        {
            var apiUrl = $"post/{{postId}}/comment";

            var response = await _httpClient.DeleteAsync(apiUrl);

            response.EnsureSuccessStatusCode(); // Throw an exception if the status code indicates failure
        }
    }
}
