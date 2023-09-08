using ProjectSBS.Services.User;
using System.Text.Json;

namespace ProjectSBS.Services.Storage.Data;

public class DataService : IDataService
{
    private readonly IStorageService _storage;
    private readonly IStorageService _remoteStorage;
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authentication;

    //TODO Rename
    private const string itemsPath = "appItems.json";
    private const string logsPath = "itemLogs.json";

    public DataService(IEnumerable<IStorageService> storages, IUserService userService, IAuthenticationService authentication)
    {
        _storage = storages.First();
        _remoteStorage = storages.Last();
        _userService = userService;
        _authentication = authentication;
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

        return await SaveAsync(stringData, itemsPath);
    }

    public async Task<bool> AddLogAsync(ItemLog log)
    {
        var logs = await LoadLogsAsync() ?? new List<ItemLog>();

        logs.Add(log);

        var stringData = JsonSerializer.Serialize(logs);

        return await SaveAsync(stringData, logsPath);
    }

    public async Task<bool> SaveLogsAsync(List<ItemLog> logs)
    {
        var stringData = JsonSerializer.Serialize(logs);

        return await SaveAsync(stringData, logsPath);
    }

    private async Task<bool> SaveAsync(string content, string path)
    {
        await _storage.WriteFileAsync(content, path);

        var isSigned = await _authentication.IsAuthenticated();

        if (isSigned)
        {
            await _userService.UploadData(content, path);
        }

        //TODO return based on result of both operations
        return true;
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
        var isSigned = await _authentication.IsAuthenticated();
        string remoteContent = "";

        if (isSigned)
        {
            //TODO Use CancellationToken
            var data = await _userService.RetrieveData(path, CancellationToken.None);
            remoteContent = data.ReadToEnd();
        }

        if (!_storage.DoesFileExist(path))
        {
            return null;
        }

        var fileContent = await _storage.ReadFileAsync(path);

        if (string.IsNullOrEmpty(fileContent))
        {
            return null;
        }

        //TODO Compare remote and local content

        return JsonSerializer.Deserialize(remoteContent, type);
    }
}
