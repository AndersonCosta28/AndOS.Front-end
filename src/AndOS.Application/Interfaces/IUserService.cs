namespace AndOS.Application.Interfaces;

public interface IUserService : IService
{
    Task<string> GetUserNameAsync();

    Task<string> GetEmailAsync();
}
