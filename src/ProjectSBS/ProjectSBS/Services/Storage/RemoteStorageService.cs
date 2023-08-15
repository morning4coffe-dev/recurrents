namespace ProjectSBS.Services.Storage;

//TODO: Auth - Implement remote storage
public class RemoteStorageService : IStorageService
{
    private RemoteStorageService()
    {

    }

    public bool DoesFileExist(string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<string> ReadFileAsync(string relativeLocalPath, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> WriteFileAsync(string content, string relativeLocalPath)
    {
        throw new NotImplementedException();
    }
    
    public Task<bool> DeleteFileAsync(string absolutePathInLocalStorage)
    {
        throw new NotImplementedException();
    }
}
