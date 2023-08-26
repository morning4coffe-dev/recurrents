using System.Text;
using UserModel = ProjectSBS.Business.Models;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;

namespace ProjectSBS.Services.User;

public class MsalUser : IUserService
{
    private readonly ITokenCache _token;

    private GraphServiceClient? _client;

    public MsalUser(ITokenCache token)
    {
        _token = token;

        Initialize();
    }

    private async void Initialize()
    {
        var token = await _token.AccessTokenAsync();

        if (token is not null)
        {
            var authenticationProvider = new BaseBearerTokenAuthenticationProvider(new TokenProvider(token));

            _client = new GraphServiceClient(authenticationProvider);
        }
    }

    public async Task<UserModel.User> GetUser()
    {
        var user = await _client.Me
            .GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select =
                    new string[] { "displayName", "mail" };
            });

        var fullName = user.DisplayName;
        var firstName = user.GivenName;
        var mail = user.Mail;
        var c = user.Photo;

        return new UserModel.User(fullName, mail);
    }

    public async Task<bool> UploadData(string content, string relativeLocalPath, CancellationToken token = default)
    {
        //TODO cancellation token not used
        //https://learn.microsoft.com/en-us/onedrive/developer/rest-api/concepts/special-folders-appfolder?view=odsp-graph-online
        var user = await _client.Me.GetAsync();

        var userDrive = await _client.Users[user.Id].Drive.GetAsync();
        var driveItems = await _client.Drives[userDrive.Id].Special["approot"].GetAsync();

        byte[] byteArray = Encoding.UTF8.GetBytes(content);
        var newItem = new DriveItem { Name = relativeLocalPath, Content = byteArray };

        // Upload the file content
        var uploadedItem = await _client.Drives[userDrive.Id]
            .Items[driveItems.Id]
            .ItemWithPath(newItem.Name).Content
            .PutAsync(new MemoryStream(byteArray));

        return uploadedItem != null;
    }

    public async Task<bool> RetrieveData(CancellationToken token)
    {
        return false;
    }
}

class TokenProvider : IAccessTokenProvider
{
    private readonly string _token;

    public TokenProvider(string token)     
    {
        _token = token;
    }

    public Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object> additionalAuthenticationContext = default,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_token);
    }

    public AllowedHostsValidator AllowedHostsValidator { get; }
}
