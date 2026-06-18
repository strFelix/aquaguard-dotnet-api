namespace AquaGuard.API.ViewModels.Shared;

public class ApiResponseViewModel<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponseViewModel<T> Ok(T data, string message = "Operação realizada com sucesso.") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponseViewModel<T> Fail(string message) =>
        new() { Success = false, Message = message, Data = default };
}