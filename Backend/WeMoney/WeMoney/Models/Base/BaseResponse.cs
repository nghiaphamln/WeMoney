namespace WeMoney.Models.Base;

public class BaseResponse
{
    public string? Message { get; set; }

    public BaseResponse(string message)
    {
        Message = message;
    }
}