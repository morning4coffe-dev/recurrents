namespace ProjectSBS.Services.Navigation;

public interface INavigation
{
    public Frame? RootFrame { set; }
    public Frame? NestedFrame { set; }

    public void Navigate(Type page);
    public void NavigateNested(Type page);
}
