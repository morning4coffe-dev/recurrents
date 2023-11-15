using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Text;
using Uno.UI.MSAL;
using UserModel = ProjectSBS.Business.Models;

namespace ProjectSBS.Services.User;

public class MsalUser : IUserService
{
    private GraphServiceClient? _client;

    private UserModel.User? _currentUser;

    public MsalUser()
    {
        //_token = token;
    }

    private string token = "";

    public bool IsLoggedIn => _currentUser is { };

    public event EventHandler<UserModel.User?>? OnLoggedInChanged;

    public async Task<bool> LoginUser()
    {
        IPublicClientApplication _app = PublicClientApplicationBuilder.Create("")
        .WithRedirectUri("")
        .WithAuthority("")
        .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
        .WithUnoHelpers()
        .Build();


        string[] scopes = new string[] {

            };


        var accounts = await _app.GetAccountsAsync();

        AuthenticationResult? result;
        try
        {
            if (Enumerable.Any(accounts))
            {
                result = await _app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                       .ExecuteAsync();
            }
            else
            {
                result = await _app.AcquireTokenInteractive(scopes)
                    .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount)
                    .WithUnoHelpers() // Add this line on interactive token acquisition flow
                    .ExecuteAsync();
            }
        }
        catch (Exception ex)
        {
            //TODO Log Login failed
            return false;
        }

        if (result != null)
        {
            token = result.AccessToken;
            // Use the token

            return true;
        }

        return false;
    }

    private async void Initialize()
    {
        //var token = await _token. .AccessTokenAsync();

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
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

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

        OnLoggedInChanged?.Invoke(this, _currentUser);

        _currentUser = new UserModel.User(fullName, mail, photoBitmap);
        return _currentUser;
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

        OnLoggedInChanged.Invoke(this, _currentUser);
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
