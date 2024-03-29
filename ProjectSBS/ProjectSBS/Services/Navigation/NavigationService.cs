namespace ProjectSBS.Services.Navigation;

internal class NavigationService(IStringLocalizer localizer) : INavigation
{
    private readonly Queue<QueuedDialog> _dialogQueue = new();
    private bool _isProcessing;

    public List<NavigationCategory> Categories { get; private init; } =
    [
        new(0, localizer["Home"], "\uE80F", typeof(HomePage)),
        new(1, localizer["Items"], "\uF0B2", typeof(HomePage)),
        new(2, localizer["Archive"], "\uE7B8", typeof(ArchivePage)),
        //new(3, localizer["Stats"], "\uEAFC", typeof(HomePage)),
        //new(4, localizer["Dev"], "\uE98F", typeof(HomePage)/*, CategoryVisibility.Desktop*/),
        new(5, localizer["Settings"], "\uE713", typeof(SettingsPage)/*, CategoryVisibility.Mobile*/),
    ];

    public event EventHandler<NavigationCategory>? CategoryChanged;
    public NavigationCategory SelectedCategory { get; private set; }

    public Frame? RootFrame { private get; set; }
    public Frame? NestedFrame { get; set; }

    public void Navigate(Type page)
    {
        if (RootFrame is not { } frame)
        {
            return;
        }

        frame.Navigate(page);
    }

    public void NavigateCategory(NavigationCategory category)
    {
        if (NestedFrame is not { } frame)
        {
            return;
        }

        frame.Navigate(category.Page);
        SelectedCategory = category;
        CategoryChanged?.Invoke(this, category);
    }

    public async Task<ContentDialogResult> ShowDialogAsync(ContentDialog dialog)
    {
        var queuedDialog = EnqueueDialog(dialog);
        await ProcessQueueAsync();
        return await queuedDialog.CompletionSource.Task;
    }

    private QueuedDialog EnqueueDialog(ContentDialog dialog)
    {
        var queuedDialog = new QueuedDialog(dialog);
        _dialogQueue.Enqueue(queuedDialog);
        return queuedDialog;
    }

    private async Task ProcessQueueAsync()
    {
        if (!_isProcessing)
        {
            try
            {
                _isProcessing = true;
                while (_dialogQueue.Count > 0)
                {
                    var queuedDialog = _dialogQueue.Dequeue();
                    queuedDialog.Dialog.XamlRoot = RootFrame?.XamlRoot;
                    var result = await queuedDialog.Dialog.ShowAsync();
                    queuedDialog.CompletionSource.SetResult(result);
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }
}
