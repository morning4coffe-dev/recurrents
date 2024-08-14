namespace Recurrents.Services.Navigation;

public interface INavigation
{
    List<NavigationCategory> Categories { get; }

    event EventHandler<NavigationCategory>? CategoryChanged;
    NavigationCategory SelectedCategory { get; }

    Frame? RootFrame { set; }
    Frame? NestedFrame { get; set; }

    void Navigate(Type page);
    void NavigateCategory(NavigationCategory category);

    Task<ContentDialogResult> ShowDialogAsync(ContentDialog dialog);
}
