namespace ProjectSBS.Services.User;

public interface IUserService
{
    bool IsLoggedIn { get; }
    bool NeedsRefresh { get; }

    event EventHandler<Business.Models.User?>? OnLoggedInChanged;

    Task<bool> AuthenticateAsync(bool silentOnly = false);
    Task<Business.Models.User?> RetrieveUser();

    Task<bool> UploadData(string content, string relativeLocalPath, CancellationToken token = default);
    Task<Stream?> RetrieveData(string relativeLocalPath, CancellationToken token);

    void Logout();
}
