namespace Recurrents.Services.Dialogs;

public class DialogService(IStringLocalizer localizer, INavigation navigation) : IDialogService
{
	private readonly IStringLocalizer _localizer = localizer;
	private readonly INavigation _navigation = navigation;

    private readonly Dictionary<string, Type> _dialogs = [];

	public async Task<ContentDialogResult> ShowAsync(
        string title, 
        string message,
        string submitText = "",
        ICommand? submit = default)
	{
		var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = submitText,
            PrimaryButtonCommand = submit,
            CloseButtonText = _localizer["Cancel"]
        };

		return await _navigation.ShowDialogAsync(dialog);
	}
}
