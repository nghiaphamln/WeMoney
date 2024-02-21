namespace WeMoney.Models.Base;

[Serializable]
public class BaseResponse
{
    public string? Message { get; set; }

    public BaseResponse(string? message)
    {
        Message = message;
    }

    protected BaseResponse()
    {
        
    }
}

[Serializable]
public class BaseResponse<T> : BaseResponse
{
    public T? Data { get; set; }

    public BaseResponse(T? data)
    {
        Data = data;
    }

    public BaseResponse(T? data, string? message)
    {
        Data = data;
        Message = message;
    }
}