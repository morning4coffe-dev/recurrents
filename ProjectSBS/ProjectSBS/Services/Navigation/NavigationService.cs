namespace ProjectSBS.Services.Navigation;

internal class NavigationService : INavigation
{
    public List<NavigationCategory> Categories { get; private init; }

    public NavigationCategory SelectedCategory { get; set; }

    public Frame? RootFrame { private get; set; }
    public Frame? NestedFrame { get; set; }

    public NavigationService(IStringLocalizer localizer)
    {
        Categories =
        [
            new(0, localizer["Home"], "\uE80F", typeof(HomePage)),
            new(1, localizer["Items"], "\uF0B2", typeof(HomePage)),
            new(2, localizer["Archive"], "\uE7B8", typeof(ArchivePage)),
            //new(3, localizer["Stats"], "\uEAFC", typeof(HomePage)),
            //new(4, localizer["Dev"], "\uE98F", typeof(HomePage)/*, CategoryVisibility.Desktop*/),
            new(5, localizer["Settings"], "\uE713", typeof(SettingsPage)/*, CategoryVisibility.Mobile*/),
        ];

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
