namespace ProjectSBS.Services.Navigation;

internal class NavigationService : INavigation
{
    public Frame? RootFrame { private get; set; }
    public Frame? NestedFrame { private get; set; }

    public void Navigate(Type page)
    {
        if (RootFrame is not { } frame)
        {
            return;
        }

        frame.Navigate(page);
    }

    public void NavigateNested(Type page)
    {
        if (NestedFrame is not { } frame)
        {
            return;
        }

        frame.Navigate(page);
    }
}
