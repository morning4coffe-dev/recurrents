namespace ProjectSBS.Services.Storage;

/// <summary>
/// Interface for a service that writes data to either local or remote directory.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Reads data from the local storage.
    /// </summary>
    /// <param name="relativeLocalPath">The relative path of the file to read.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The content of the file as a string.</returns>
    Task<string> ReadLocalAsync(string relativeLocalPath, CancellationToken ct = default);

    /// <summary>
    /// Reads data from a remote storage.
    /// </summary>
    /// <param name="relativeLocalPath">The relative path of the file to read.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The content of the file as a string.</returns>
    Task<string> ReadRemoteAsync(string relativeLocalPath, CancellationToken ct = default);

    /// <summary>
    /// Deletes a file from the local storage.
    /// </summary>
    /// <param name="absolutePathInLocalStorage">The absolute path of the file in the local storage.</param>
    /// <returns>True if the file was successfully deleted; otherwise, false.</returns>
    Task<bool> DeleteFileAsync(string absolutePathInLocalStorage);

    /// <summary>
    /// Writes a string to the local storage.
    /// </summary>
    /// <param name="content">The content to write as a string.</param>
    /// <param name="relativeLocalPath">The relative path to the file.</param>
    Task WriteStringAsync(string content, string relativeLocalPath);

    /// <summary>
    /// Writes an image to the local storage.
    /// </summary>
    /// <param name="stream">The stream containing the image data.</param>
    /// <param name="nameWithExt">The name of the image file with extension.</param>
    /// <returns>The path of the saved image file.</returns>
    Task<string> WriteImageAsync(Stream stream, string nameWithExt);

    /// <summary>
    /// Writes a file to the local storage.
    /// </summary>
    /// <param name="stream">The stream containing the file data.</param>
    /// <param name="nameWithExt">The name of the file with extension.</param>
    /// <param name="localDirName">The name of the local directory (optional).</param>
    /// <returns>The path of the saved file.</returns>
    Task<string> WriteFileAsync(Stream stream, string nameWithExt, string? localDirName = null);

    /// <summary>
    /// Writes a bitmap image to the local storage.
    /// </summary>
    /// <param name="stream">The stream containing the bitmap image data.</param>
    /// <param name="nameWithExt">The name of the bitmap image file with extension.</param>
    /// <returns>The path of the saved bitmap image file.</returns>
    Task<string> WriteBitmapAsync(Stream stream, string nameWithExt);

    /// <summary>
    /// Checks if a file exists in the local storage.
    /// </summary>
    /// <param name="fileName">The name of the file to check.</param>
    /// <returns>True if the file exists; otherwise, false.</returns>
    bool DoesFileExist(string fileName);

    /// <summary>
    /// Sets a value in the storage.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key to identify the value.</param>
    /// <param name="value">The value to be stored.</param>
    void SetValue<T>(string key, T value);

    /// <summary>
    /// Gets a value from the storage.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key to identify the value.</param>
    /// <param name="defaultValue">The default value to return if the key is not found.</param>
    /// <returns>The retrieved value or the default value if the key is not found.</returns>
    T GetValue<T>(string key, T defaultValue = default);
}