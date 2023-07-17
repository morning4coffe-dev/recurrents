namespace ProjectSBS.Services.Interop
{
    public interface IInteropService
    {
        Version GetAppVersion();
        Task OpenStoreReviewUrlAsync();
        Task SetThemeAsync(ElementTheme theme);
        void UpdateAppTitle(string title);
    }
}