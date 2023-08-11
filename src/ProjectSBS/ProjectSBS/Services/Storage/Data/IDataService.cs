namespace ProjectSBS.Services.FileManagement.Data;

public interface IDataService
{
    Task<(List<Item>, List<ItemLog>)> InitializeDatabaseAsync();
    Task<List<Item>> LoadDataAsync();

    Task<bool> SaveDataAsync(List<Item> data);
    Task<bool> SaveLogsAsync(List<ItemLog> logs);

    Task<bool> AddLogAsync(ItemLog log);
}