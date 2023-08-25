namespace App.Entity
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }

    public static class ApiResponseHelper
    {
        public static ApiResponse<T> Success<T>(T data, string message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> Error<T>(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default(T),
                Message = message
            };
        }
    }
}
