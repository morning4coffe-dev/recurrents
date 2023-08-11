using ProjectSBS.Services.Storage;
using System.Text.Json;

namespace ProjectSBS.Services.FileManagement.Data;

public class DataService : IDataService
{
    private readonly IStorageService _storage;

    //TODO Rename
    private const string itemsPath = "appItems.json";
    private const string logsPath = "appItems.json";

    public DataService(IStorageService storage)
    {
        _storage = storage;
    }

    public async Task<(List<Item>, List<ItemLog>)> InitializeDatabaseAsync()
    {
        //TODO Check if user is logged in
        //Load from remote database
        //Check if the remote database is newer than the local one (or vice-versa)

        var data = await LoadDataAsync();
        var logs = await LoadLogsAsync();

        return (data, logs);
    }

    public async Task<bool> SaveDataAsync(List<Item> data)
    {
        var stringData = JsonSerializer.Serialize(data);

        await _storage.WriteStringAsync(stringData, itemsPath);

        //TODO return depending on success
        return false;
    }

    public async Task<bool> SaveLogsAsync(List<ItemLog> logs)
    {
        var stringData = JsonSerializer.Serialize(logs);

        await _storage.WriteStringAsync(stringData, logsPath);

        //TODO return depending on success
        return false;
    }

    public async Task<bool> AddLogAsync(ItemLog log)
    {
        //TODO This seem inefficient
        var logs = await LoadLogsAsync();

        logs.Add(log);

        var stringData = JsonSerializer.Serialize(logs);

        await _storage.WriteStringAsync(stringData, logsPath);

        //TODO return depending on success
        return false;
    }

    public async Task<List<Item>> LoadDataAsync()
    {
        var data = await LoadAsync(itemsPath);

        if (data is null)
        {
            return new List<Item>();
        }

        return (List<Item>)data;
    }

    private async Task<List<ItemLog>> LoadLogsAsync()
    {
        var logs = await LoadAsync(logsPath);

        if (logs is null)
        {
            return new List<ItemLog>();
        }

        return (List<ItemLog>)logs;
    }

    private async Task<object?> LoadAsync(string path)
    {
        //TODO Check if user is connected and is logged in
        //Load from remote database

        if (!_storage.DoesFileExist(path))
        {
            return null;
        }

        var fileContent = await _storage.ReadLocalAsync(path);

        if (string.IsNullOrEmpty(fileContent))
        {
            return null;
        }

        //TODO Try catch

        var appData = JsonSerializer.Deserialize<List<Item>>(fileContent);
        return appData;
    }
}
