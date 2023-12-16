namespace secrestapi.Models;

public class UserToken
{
    public string Token { set; get; } = "";
    public string ExpiredAt { set; get; } = "";
    public string Message { set; get; } = "";
}

