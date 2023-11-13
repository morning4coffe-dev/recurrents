namespace ProjectSBS.Services.User;

public interface IUserService
{
    bool IsLoggedIn { get; }
    event EventHandler<Business.Models.User?>? OnLoggedInChanged;
    Task<bool> LoginUser();
    Task<Business.Models.User?> GetUser();
    Task<bool> UploadData(string content, string relativeLocalPath, CancellationToken token = default);
    Task<Stream?> RetrieveData(string relativeLocalPath, CancellationToken token);
    void Logout();
}