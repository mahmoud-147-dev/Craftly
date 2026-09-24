using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Results
{
    public class Result<T> 
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? Error { get; private set; }

        private Result(bool isSuccess, T? data, string? error)
        {
            IsSuccess = isSuccess;
            Data = data;
            Error = error;
        }

        public static Result<T> Success(T data)
        {
            return new Result<T>(
                isSuccess: true,
                data: data,
                error: null);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(
                isSuccess: false,
                data: default,
                error: error);
        }
    }
}
