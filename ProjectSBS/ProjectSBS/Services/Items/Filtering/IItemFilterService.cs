namespace ProjectSBS.Services.Items.Filtering;

public interface IItemFilterService
{
    List<Tag> Categories { get; }
    Tag SelectedCategory { get; set; }
}