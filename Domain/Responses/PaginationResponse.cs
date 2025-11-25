using System.Net;

namespace Domain.Responses;

public class PaginationResponse<T> : ServiceResult<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }

    public PaginationResponse(T data, int totalRecords, int pageNumber, int pageSize) : base(data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
    }
    
    public PaginationResponse(T data) : base(data) {}

    public PaginationResponse(HttpStatusCode statusCode, string message) : base(statusCode, message) { }
}