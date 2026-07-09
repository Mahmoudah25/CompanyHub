namespace CompanyHub.API.Middleware
{
    public class ErrorResponse
    {
        public int Status {  get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
