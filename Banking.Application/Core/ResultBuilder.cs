namespace Banking.Application.Core
{
    public static class ResultBuilder
    {
        public static Result<T> Success<T>(T value) => new Result<T>(value, null);
        public static Result<T> Failure<T>(Exception error) => new Result<T>(default, error);
    }

    public class Result<T>
    {
        public T? Value { get; }
        public Exception? Error { get; }
        public bool IsSuccess => Error == null;

        internal Result(T? value, Exception? error)
        {
            Value = value;
            Error = error;
        }
    }
}
