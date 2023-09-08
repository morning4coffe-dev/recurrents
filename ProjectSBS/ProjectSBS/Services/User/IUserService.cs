namespace ProjectSBS.Services.User;

public interface IUserService
{
    Task<Business.Models.User?> GetUser();
    Task<bool> UploadData(string content, string relativeLocalPath, CancellationToken token = default);
    Task<Stream?> RetrieveData(string relativeLocalPath, CancellationToken token);
}