namespace ProjectSBS.Services.Navigation;

public interface INavigation
{
    public List<NavigationCategory> Categories { get; }
    public NavigationCategory SelectedCategory { get; set; }

    public Frame? RootFrame { set; }
    public Frame? NestedFrame { get; set; }

    public void Navigate(Type page);
    public void NavigateNested(Type page);
}
