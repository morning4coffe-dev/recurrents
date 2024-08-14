namespace Recurrents.Services.Dialogs;

public interface IDialogService
{
    Task<ContentDialogResult> ShowAsync(
        string title,
        string message,
        string submitText = "",
        ICommand? submit = default);
}
