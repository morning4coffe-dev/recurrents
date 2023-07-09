namespace ProjectSBS.Services.Storage;

public interface IFileWriter
{
    Task<bool> DeleteFileAsync(string absolutePathInLocalStorage);
    Task<string> ReadLocalAsync(string relativeLocalPath, CancellationToken ct = default);
    Task<string> ReadAsync(string relativeLocalPath, CancellationToken ct = default);
    Task<string> WriteBitmapAsync(Stream stream, string nameWithExt);
    Task<string> WriteFileAsync(Stream stream, string nameWithExt, string? localDirName = null);
    Task<string> WriteImageAsync(Stream stream, string nameWithExt);
    Task WriteStringAsync(string content, string relativeLocalPath);
}