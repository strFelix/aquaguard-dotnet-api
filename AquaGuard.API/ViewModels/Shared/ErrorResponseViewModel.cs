namespace AquaGuard.API.ViewModels.Shared;

public class ErrorResponseViewModel
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}