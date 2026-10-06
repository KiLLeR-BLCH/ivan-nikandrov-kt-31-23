namespace IvanNikandrovKt_31_23.Dto
{
    public enum ServiceStatus
    {
        Ok,
        NotFound,
        Invalid
    }

    /// <summary>Результат работы сервиса: данные либо ошибка (не найдено / некорректные данные).</summary>
    public class ServiceResult<T>
    {
        public ServiceStatus Status { get; private init; }
        public T? Data { get; private init; }
        public string? Message { get; private init; }

        public static ServiceResult<T> Ok(T data) => new() { Status = ServiceStatus.Ok, Data = data };
        public static ServiceResult<T> NotFound(string message) => new() { Status = ServiceStatus.NotFound, Message = message };
        public static ServiceResult<T> Invalid(string message) => new() { Status = ServiceStatus.Invalid, Message = message };
    }
}
