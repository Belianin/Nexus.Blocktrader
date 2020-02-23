namespace Blocktrader.Utils
{
    public class Result<T>
    {
        public T Value { get; private set; }
        
        public string Error { get; private set; }

        public bool IsSuccess { get; private set; }

        public bool IsFail => !IsSuccess;

        public static Result<T> Ok(T value)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Value = value
            };
        }

        public static Result<T> Fail(string error)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Error = error
            };
        }

        public static implicit operator Result<T>(T value)
        {
            return Ok(value);
        }

        public static implicit operator Result<T>(string error)
        {
            return Fail(error);
        }
        
        private Result()
        {
            
        }
    }

    public class Result
    {
        public string Error { get; private set; }

        public bool IsSuccess { get; private set; }

        public bool IsFail => !IsSuccess;

        public static Result Ok()
        {
            return new Result
            {
                IsSuccess = true
            };
        }

        public static Result Fail(string error)
        {
            return new Result
            {
                IsSuccess = false,
                Error = error
            };
        }

        public static Result<T> Ok<T>(T value)
        {
            return Result<T>.Ok(value);
        }

        public static Result<T> Fail<T>(string error)
        {
            return Result<T>.Fail(error);
        }
        
        private Result()
        {
            
        }
    }
}