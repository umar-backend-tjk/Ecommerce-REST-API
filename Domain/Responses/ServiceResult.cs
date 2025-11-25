using System.Net;

namespace Domain.Responses;

public class ServiceResult<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }

    public ServiceResult(T data)
    {
        Data = data;
        Message = "Success";
        StatusCode = (int)HttpStatusCode.OK;
    }

    public ServiceResult(HttpStatusCode statusCode, string message)
    {   
        Data = default;
        Message = message;
        StatusCode = (int)statusCode;
    }
    
    public static ServiceResult<T> Ok(T data)
        => new ServiceResult<T>(data);

    public static ServiceResult<T> Fail(string message, HttpStatusCode code = HttpStatusCode.BadRequest)
        => new ServiceResult<T>(code, message);
}

public class ServiceResult
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public object? Data { get; set; }
    

    public ServiceResult(HttpStatusCode statusCode, string message)
    {   
        Data = default;
        Message = message;
        StatusCode = (int)statusCode;
    }
    
    public static ServiceResult Ok(string message = "")
        => new ServiceResult(HttpStatusCode.OK, message);

    public static ServiceResult Fail(string message, HttpStatusCode code = HttpStatusCode.BadRequest)
        => new ServiceResult(code, message);

}