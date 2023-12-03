namespace ProjectSBS.Services.Navigation;

public interface INavigation
{
    List<NavigationCategory> Categories { get; }
    NavigationCategory SelectedCategory { get; set; }

    Frame? RootFrame { set; }
    Frame? NestedFrame { get; set; }

    void Navigate(Type page);
    void NavigateNested(Type page);
}
