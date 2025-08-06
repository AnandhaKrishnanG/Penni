namespace Penni.WebAPI.Models.Responses
{
    public class ErrorResponseDto
    {
        public int StatusCode { get; set; }
        public string MessageCode { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}
