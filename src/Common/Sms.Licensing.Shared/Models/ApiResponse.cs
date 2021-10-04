namespace Sms.Licensing.Shared.Models
{
    public class ApiResponse : ApiResponse<object>
    {
    }

    public class ApiResponse<T>
    {
        public ApiResponseStatus Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public enum ApiResponseStatus
    {
        Success,
        Error
    }
}