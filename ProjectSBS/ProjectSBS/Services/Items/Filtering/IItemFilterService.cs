namespace ProjectSBS.Services.Items.Filtering;

public interface IItemFilterService
{
    List<FilterCategory> Categories { get; }
    FilterCategory SelectedCategory { get; set; }
}