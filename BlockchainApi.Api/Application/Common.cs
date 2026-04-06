namespace BlockchainApi.Api.Application.Common;

public class Constants
{
    public static readonly HashSet<string> ValidCoins = new(StringComparer.OrdinalIgnoreCase)
    {
        "btc", "eth", "ltc", "dash"
    };
}

public class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }
    public int StatusCode { get; init; }

    public static Result<T> Success(T value) => new()
    {
        IsSuccess = true,
        Value = value,
        StatusCode = 200
    };

    public static Result<T> Failure(string error, int statusCode) => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = statusCode
    };
}
