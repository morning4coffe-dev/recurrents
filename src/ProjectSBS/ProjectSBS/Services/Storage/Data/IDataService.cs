namespace ProjectSBS.Services.FileManagement.Data
{
    public interface IDataService
    {
        Task<List<Item>> InitializeDatabaseAsync();
        Task<List<Item>?> LoadLocalAsync();
        Task LoadRemoteAsync();
        Task SaveLocalAsync(List<Item> data);
        Task SaveRemoteAsync();
    }
}