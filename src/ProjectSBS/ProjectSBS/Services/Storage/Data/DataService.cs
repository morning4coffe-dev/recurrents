using ProjectSBS.Services.Storage;
using System.Text.Json;

namespace ProjectSBS.Services.FileManagement.Data;

public class DataService : IDataService
{
    private readonly IStorageService _storage;

    //TODO Rename
    private const string itemsName = "appItems.json";

    public DataService(IStorageService storage)
    {
        _storage = storage;
    }

    public async Task<List<Item>> InitializeDatabaseAsync()
    {
        //TODO Check if user is logged in
        //Load from remote database

        var data = await LoadLocalAsync();

        if (data is not null)
        {
            return data;
        }

        return new List<Item>();
    }



    public async Task SaveLocalAsync(List<Item> data)
    {
        var stringData = JsonSerializer.Serialize(data);

        await _storage.WriteStringAsync(stringData, itemsName);
    }

    public async Task SaveRemoteAsync()
    {

    }

    private async Task SaveAsync()
    {

    }

    public async Task<List<Item>?> LoadLocalAsync()
    {
        if (!_storage.DoesFileExist(itemsName))
        {
            return null;
        }

        var fileContent = await _storage.ReadLocalAsync(itemsName);

        if (string.IsNullOrEmpty(fileContent))
        {
            return null;
        }

        var appData = JsonSerializer.Deserialize<List<Item>>(fileContent);
        return appData;
    }

    public async Task LoadRemoteAsync()
    {

    }

    private async Task LoadAsync()
    {

    }
}
