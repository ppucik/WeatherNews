namespace WeatherNews.Application.Common;

/// <summary>
/// Represents the outcome of an operation, containing either a successful result value or an error message.
/// </summary>
/// <remarks>
/// A <see cref="Result{T}"/> instance indicates whether an operation succeeded or failed. If the operation succeeds,
/// the IsSuccess property is <see langword="true"/> and the Value property contains the result. If the operation fails,
/// IsSuccess is <see langword="false"/> and the Error property contains a descriptive error message. This type is
/// commonly used to avoid exceptions for expected error conditions and to make error handling explicit.
/// </remarks>
/// <typeparam name="T">The type of the value returned when the operation is successful.</typeparam>
public sealed class Result<T>
{
    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }


    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
