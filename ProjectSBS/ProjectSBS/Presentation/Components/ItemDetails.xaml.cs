namespace ProjectSBS.Presentation.Components;

public sealed partial class ItemDetails : Page
{
    public ItemDetailsViewModel ViewModel => (ItemDetailsViewModel)DataContext;
    public ItemDetails()
    {
        InitializeComponent();

        DataContext = App.Services?.GetRequiredService<ItemDetailsViewModel>()!;
        Loaded += Page_Loaded;
        Unloaded += Page_Unloaded;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Load();
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Unload();
    }

    private void PeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.FirstOrDefault() as KeyValuePair<Period, string>? is { } period)
        {
            //ViewModel.SelectedPeriod = period.Key;
        }
    }
}
