namespace ProjectSBS.Services.Navigation;

internal class NavigationService : INavigation
{
    public List<NavigationCategory> Categories { get; private init; }

    public NavigationCategory SelectedCategory { get; set; }

    public Frame? RootFrame { private get; set; }
    public Frame? NestedFrame { get; set; }

    public NavigationService()
    {
        Categories = new()
        {
            new("Home", "\uE80F", typeof(HomePage)),
            new("Items", "\uF0B2", typeof(HomePage)),
            new("Stats", "\uEAFC", typeof(HomePage)),
            new("Dev", "\uE98F", typeof(HomePage)),
        };

        SelectedCategory = Categories[0];
    }

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
