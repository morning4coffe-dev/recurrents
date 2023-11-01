using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Text;
using UserModel = ProjectSBS.Business.Models;

namespace ProjectSBS.Services.User;

public class MsalUser : IUserService
{
    private readonly ITokenCache _token;

    private GraphServiceClient? _client;

    private UserModel.User? _currentUser;

    public MsalUser(ITokenCache token)
    {
        _token = token;
    }

    private async void Initialize()
    {
        var token = await _token.AccessTokenAsync();

        if (token is not null)
        {
            var authenticationProvider = new BaseBearerTokenAuthenticationProvider(new TokenProvider(token));

            _client = new GraphServiceClient(authenticationProvider);//.WithIosKeychainSecurityGroup("com.microsoft.adalcache
        }
    }

    public async Task<UserModel.User?> GetUser()
    {
        if (_currentUser is { })
        {
            return _currentUser;
        }

        if (_client is null)
        {
            Initialize();

            if (_client is null)
            {
                return null;
            }
        }

        var user = await _client.Me
            .GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select =
                    new string[] { "displayName", "mail" };
            });

        var fullName = user?.DisplayName;
        var mail = user?.Mail;

        // Retrieve the user's profile picture
        BitmapImage? photoBitmap = null;
        try
        {
            var photoStream = await _client.Me
                .Photo
                .Content
                .GetAsync();

            photoBitmap = new BitmapImage();
            var stream = new MemoryStream();
            await photoStream.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            await photoBitmap.SetSourceAsync(stream.AsRandomAccessStream());
        }
        catch
        {
            // TODO There is no profile picture
        }

        return new UserModel.User(fullName, mail, photoBitmap);
    }

    public async Task<bool> UploadData(string content, string relativeLocalPath, CancellationToken token = default)
    {
        //https://learn.microsoft.com/en-us/onedrive/developer/rest-api/concepts/special-folders-appfolder?view=odsp-graph-online
        var user = await _client.Me.GetAsync(cancellationToken: token);

        var userDrive = await _client.Users[user.Id].Drive.GetAsync(cancellationToken: token);
        var driveItems = await _client.Drives[userDrive.Id].Special["approot"].GetAsync(cancellationToken: token);

        byte[] byteArray = Encoding.UTF8.GetBytes(content);
        var newItem = new DriveItem { Name = relativeLocalPath, Content = byteArray };

        var uploadedItem = await _client.Drives[userDrive.Id]
            .Items[driveItems.Id]
            .ItemWithPath(newItem.Name).Content
            .PutAsync(new MemoryStream(byteArray));

        return uploadedItem != null;
    }

    public async Task<Stream?> RetrieveData(string relativeLocalPath, CancellationToken token)
    {
        if (_client is null)
        {
            return null;
        }

        var user = await _client.Me.GetAsync(cancellationToken: token);

        var userDrive = await _client.Users[user.Id].Drive.GetAsync(cancellationToken: token);
        var driveItems = await _client.Drives[userDrive.Id].Special["approot"].GetAsync(cancellationToken: token);

        return await _client.Drives[userDrive.Id]
            .Items[driveItems.Id]
            .ItemWithPath(relativeLocalPath)
            .Content
            .GetAsync(cancellationToken: token);
    }

    public void Logout()
    {
        _currentUser = null;
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
