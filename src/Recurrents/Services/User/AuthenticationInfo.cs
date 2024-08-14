namespace Recurrents.Services.User;

public class AuthenticationInfo
{
	public required DateTimeOffset ExpiresOn { get; init; }

	public required string Token { get; init; }
}
