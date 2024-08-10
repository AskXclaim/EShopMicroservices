using System.Net;

namespace BuildingBlocks.Models;

public class ApiResponse
{
    public HttpStatusCode Status { get; set; }
    public bool IsSuccess { get; set; }
    public ICollection<string> Error { get; set; } = new List<string>();
    public object? Result { get; set; }
}