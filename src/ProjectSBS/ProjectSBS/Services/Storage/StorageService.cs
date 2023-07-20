using System.Collections.Concurrent;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace ProjectSBS.Services.Storage
{
    /// <summary>
    /// Writes data to either the local or remote directory.
    /// </summary>
    public class StorageService : IStorageService
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();
        private const string _imagesDirName = "images";

        private readonly IPropertySet SettingsStorage = ApplicationData.Current.LocalSettings.Values;

        public async Task<string> ReadLocalAsync(string relativeLocalPath, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(relativeLocalPath))
            {
                return string.Empty;
            }

            IStorageItem targetLocation = await ApplicationData.Current.LocalFolder.TryGetItemAsync(relativeLocalPath);

            ct.ThrowIfCancellationRequested();

            if (targetLocation is StorageFile file)
            {
                return await FileIO.ReadTextAsync(file);
            }

            return string.Empty;
        }

        public async Task<string> ReadRemoteAsync(string relativeLocalPath, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteFileAsync(string absolutePathInLocalStorage)
        {
            if (string.IsNullOrEmpty(absolutePathInLocalStorage))
            {
                return false;
            }

            try
            {
                var file = await StorageFile.GetFileFromPathAsync(absolutePathInLocalStorage);
                await file.DeleteAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task WriteStringAsync(string content, string relativeLocalPath)
        {
            if (string.IsNullOrEmpty(relativeLocalPath))
            {
                return;
            }

            var semaphore = _semaphores.GetOrAdd(relativeLocalPath, new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();
            try
            {
                StorageFile targetLocation = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    relativeLocalPath,
                    CreationCollisionOption.OpenIfExists);

                if (targetLocation != null)
                {
                    await FileIO.WriteTextAsync(targetLocation, content);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        public Task<string> WriteImageAsync(Stream stream, string nameWithExt)
        {
            return WriteFileAsync(stream, nameWithExt, _imagesDirName);
        }

        public async Task<string> WriteFileAsync(Stream stream, string nameWithExt, string? localDirName = null)
        {
            StorageFolder dir = string.IsNullOrWhiteSpace(localDirName)
                ? ApplicationData.Current.LocalFolder
                : await ApplicationData.Current.LocalFolder.CreateFolderAsync(
                    localDirName,
                    CreationCollisionOption.OpenIfExists);

            StorageFile storageFile = await dir.CreateFileAsync(
                nameWithExt,
                CreationCollisionOption.ReplaceExisting);

            using IRandomAccessStream fileStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite);
            await stream.CopyToAsync(fileStream.AsStreamForWrite());
            await fileStream.FlushAsync();
            return storageFile.Path;
        }

        public async Task<string> WriteBitmapAsync(Stream stream, string nameWithExt)
        {
            StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                nameWithExt,
                CreationCollisionOption.ReplaceExisting);

            using (IRandomAccessStream s = stream.AsRandomAccessStream())
            {
                // Create the decoder from the stream
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(s);

                // Get the SoftwareBitmap representation of the file
                var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, await storageFile.OpenAsync(FileAccessMode.ReadWrite));

                encoder.SetSoftwareBitmap(softwareBitmap);

                await encoder.FlushAsync();

                return storageFile.Path;
            }
        }

        public bool DoesFileExist(string fileName)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var filePath = Path.Combine(localFolder.Path, fileName);
            return File.Exists(filePath);
        }

        public void SetValue<T>(string key, T value)
        {
            if (!SettingsStorage.ContainsKey(key))
                SettingsStorage.Add(key, value);
            else
                SettingsStorage[key] = value;
        }

        public T GetValue<T>(string key, T defaultValue = default)
        {
            if (SettingsStorage.TryGetValue(key, out object value))
            {
                try
                {
                    return (T)value;
                }
                catch
                {
                    // Corrupted storage, return default
                }
            }

            return defaultValue;
        }
    }
}
