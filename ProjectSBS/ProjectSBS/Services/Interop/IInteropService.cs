namespace ProjectSBS.Services.Interop
{
    public interface IInteropService
    {
        Version GetAppVersion();
        Task OpenStoreReviewUrlAsync();
        void SetTheme(ElementTheme theme);
        void UpdateAppTitle(string title);
    }
}