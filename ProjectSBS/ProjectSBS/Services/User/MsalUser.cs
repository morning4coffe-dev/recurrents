using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.UI.Xaml.Media.Imaging;
using Uno.UI.MSAL;
using UserModel = ProjectSBS.Business.Models;

namespace ProjectSBS.Services.User;

public class MsalUser : IUserService
{
    private IPublicClientApplication? _app;
    private AuthenticationInfo? _authenticationInfo;
    private GraphServiceClient? _client;
    private UserModel.User? _currentUser;

    public bool IsLoggedIn => _currentUser is { };
    public bool NeedsRefresh => _authenticationInfo?.ExpiresOn < DateTimeOffset.UtcNow.AddMinutes(-5);

    public event EventHandler<UserModel.User?>? OnLoggedInChanged;


    public async Task<bool> AuthenticateAsync(bool silentOnly = false)
    {
        if (IsLoggedIn && !NeedsRefresh)
        {
            return true;
        }

        await EnsureIdentityClientAsync();

        var accounts = await _app.GetAccountsAsync();
        AuthenticationResult? result = null;
        bool tryInteractiveLogin = false;

        try
        {
            result = await _app
                .AcquireTokenSilent(AppConfig.Scopes, accounts.FirstOrDefault())
                .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            tryInteractiveLogin = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MSAL Silent Error: {ex.Message}");
            return false;
        }

        if (tryInteractiveLogin && !silentOnly)
        {
            try
            {
                result = await _app
                    .AcquireTokenInteractive(AppConfig.Scopes)
                    .ExecuteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MSAL Interactive Error: {ex.Message}");
                return false;
            }
        }

        _authenticationInfo = new AuthenticationInfo
        {
            ExpiresOn = result?.ExpiresOn ?? DateTimeOffset.MinValue,
            Token = result?.AccessToken ?? "",
        };

        return !string.IsNullOrEmpty(_authenticationInfo.Token);
    }

    private void Initialize()
    {
        if (_authenticationInfo?.Token is not null)
        {
            var authenticationProvider = new BaseBearerTokenAuthenticationProvider(new TokenProvider(_authenticationInfo.Token));

            _client = new GraphServiceClient(authenticationProvider);
        }
    }

    public async Task<UserModel.User?> RetrieveUser()
    {
        if (_currentUser is { })
        {
            return _currentUser;
        }

        if (_client is null)
        {
            if (string.IsNullOrEmpty(_authenticationInfo?.Token))
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
                    ["displayName", "mail"];
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
            Debug.WriteLine("User: No profile picture");
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
        _app?.RemoveAsync(_app.GetAccountsAsync().Result.FirstOrDefault());

        App.Services!.GetRequiredService<ISettingsService>().ContinueWithoutLogin = false;

        OnLoggedInChanged?.Invoke(this, _currentUser);
    }

    [MemberNotNull(nameof(_app))]
    private async Task EnsureIdentityClientAsync()
    {
        if (_app == null)
        {
#if __ANDROID__
            _app = PublicClientApplicationBuilder
                .Create(AppConfig.ApplicationId)
                .WithAuthority(AzureCloudInstance.AzurePublic, AppConfig.TenantId)
                .WithRedirectUri($"msal{AppConfig.ApplicationId}://auth")
                .WithParentActivityOrWindow(() => ContextHelper.Current)
                .Build();

            await Task.CompletedTask;
#elif __IOS__
			_app = PublicClientApplicationBuilder
				.Create(AppConfig.ApplicationId)
				.WithAuthority(AzureCloudInstance.AzurePublic, AppConfig.TenantId)
				.WithIosKeychainSecurityGroup("com.microsoft.adalcache")
				.WithRedirectUri($"msal{AppConfig.ApplicationId}://auth")
				.Build();

			await Task.CompletedTask;
#else
            _app = PublicClientApplicationBuilder
                .Create(AppConfig.ApplicationId)
                .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                .WithUnoHelpers()
                .Build();

            await AttachTokenCacheAsync();
#endif
        }
    }

#if !__ANDROID__ && !__IOS__
    private async Task AttachTokenCacheAsync()
    {
#if !HAS_UNO
        // Cache configuration and hook-up to public application. Refer to https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet/wiki/Cross-platform-Token-Cache#configuring-the-token-cache
        var storageProperties = new StorageCreationPropertiesBuilder("msal.cache", ApplicationData.Current.LocalFolder.Path)
                .Build();

        var msalcachehelper = await MsalCacheHelper.CreateAsync(storageProperties);
        msalcachehelper.RegisterCache(_app!.UserTokenCache);
#else
		await Task.CompletedTask;
#endif
    }
#endif
}

class TokenProvider(string token) : IAccessTokenProvider
{
    private readonly string _token = token ?? throw new ArgumentNullException(nameof(token));

    public Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_token);
    }

    public AllowedHostsValidator? AllowedHostsValidator { get; }
}
