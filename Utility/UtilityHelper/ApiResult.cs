using Newtonsoft.Json;
using System;

namespace UtilityHelper
{
    public class ApiResult<T>
    {
        public bool Succ { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public DateTime DataTime { get; set; }
        public T Data { get; set; }

        public ApiResult()
        {
        }

        [JsonConstructor]
        public ApiResult(T data)
        {
            Code = "0000";
            Succ = true;
            DataTime = DateTime.Now;
            Data = data;
        }
    }

    public class ApiError<T> : ApiResult<T>
    {
        [JsonConstructor]
        public ApiError(string code, string message)
        {
            Code = code;
            Succ = false;
            DataTime = DateTime.Now;
            Message = message;
        }
    }
}