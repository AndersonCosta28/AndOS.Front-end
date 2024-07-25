namespace AndOS.Application.Interfaces;

public interface IUserInfo
{
    Task<string> GetUserNameAsync();

    Task<string> GetEmailAsync();
}
