using System.Text.Json.Serialization;

namespace MyFinance.Core.Response;

public class Response<TData>
{
    private readonly int code;
    public Response(TData? data, 
                    int code = Configuration.DefaultStatusCode, 
                    string? message = null)
    {
        Data = data;
        this.code = code;
        Message = message;
    }
    // Json Constructor and JsonIgnore
    // are provided by System.Text.Json.Serialization

    [JsonConstructor]
    public Response() => code = Configuration.DefaultStatusCode;
    
    public TData? Data { get; set; }
    public string? Message { get; set; }
    [JsonIgnore]
    public bool IsSuccess 
        => code is >= 200 and <= 299;
}
