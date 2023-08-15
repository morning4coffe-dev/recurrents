using ProjectSBS.Services.Storage;
using System.Text.Json;

namespace ProjectSBS.Services.FileManagement.Data;

public class DataService : IDataService
{
    private readonly IStorageService _storage;
    private readonly IStorageService _remoteStorage;

    //TODO Rename
    private const string itemsPath = "appItems.json";
    private const string logsPath = "itemLogs.json";

    public DataService(IEnumerable<IStorageService> storages)
    {
        _storage = storages.First();
        _remoteStorage = storages.Last();
    }

    public async Task<(List<Item> items, List<ItemLog> logs)> InitializeDatabaseAsync()
    {
        //TODO Check if user is logged in
        //Load from remote database
        //Check if the remote database is newer than the local one (or vice-versa)

        var data = await LoadDataAsync();
        //var logs = await LoadLogsAsync();

        return (data, new List<ItemLog>());
    }




    public async Task<bool> SaveDataAsync(List<Item> data)
    {
        var stringData = JsonSerializer.Serialize(data);

        return await _storage.WriteFileAsync(stringData, itemsPath);
    }
    public async Task<bool> AddLogAsync(ItemLog log)
    {
        var logs = await LoadLogsAsync() ?? new List<ItemLog>();

        logs.Add(log);

        var stringData = JsonSerializer.Serialize(logs);
        
        return await _storage.WriteFileAsync(stringData, logsPath);
    }

    public async Task<bool> SaveLogsAsync(List<ItemLog> logs)
    {
        var stringData = JsonSerializer.Serialize(logs);

        return await _storage.WriteFileAsync(stringData, logsPath);
    }




    public async Task<List<Item>> LoadDataAsync()
    {
        var data = await LoadAsync(itemsPath, typeof(List<Item>));

        return (data as List<Item>) ?? new List<Item>();
    }

    public async Task<List<ItemLog>> LoadLogsAsync()
    {
        var logs = await LoadAsync(logsPath, typeof(List<ItemLog>));

        return (logs as List<ItemLog>) ?? new List<ItemLog>();
    }

    private async Task<object?> LoadAsync(string path, Type type)
    {
        //TODO Check if user is connected and is logged in
        //Load from remote database

        if (!_storage.DoesFileExist(path))
        {
            return null;
        }

        var fileContent = await _storage.ReadFileAsync(path);

        if (string.IsNullOrEmpty(fileContent))
        {
            return null;
        }

        return JsonSerializer.Deserialize(fileContent, type);
    }
}
