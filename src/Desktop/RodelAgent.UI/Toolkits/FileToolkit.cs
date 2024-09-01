// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace RodelAgent.UI.Toolkits;

/// <summary>
/// 文件工具箱.
/// </summary>
public static class FileToolkit
{
    /// <summary>
    /// 选择文件.
    /// </summary>
    /// <param name="extension">扩展名.</param>
    /// <param name="windowInstance">窗口对象.</param>
    /// <returns>文件对象（可能为空）.</returns>
    public static async Task<StorageFile> PickFileAsync(string extension, object windowInstance)
    {
        try
        {
            var picker = new FileOpenPicker();
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(windowInstance));
            var exts = extension.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var ext in exts)
            {
                picker.FileTypeFilter.Add(ext);
            }

            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            var file = await picker.PickSingleFileAsync().AsTask();
            return file;
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// 保存文件.
    /// </summary>
    /// <param name="extension">文件扩展名.</param>
    /// <param name="windowInstance">窗口实例.</param>
    /// <returns>文件.</returns>
    public static async Task<StorageFile> SaveFileAsync(string extension, object windowInstance)
    {
        try
        {
            var picker = new FileSavePicker();
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(windowInstance));
            var exts = extension.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var ext in exts)
            {
                picker.FileTypeChoices.Add(ext, new List<string> { ext });
            }

            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.DefaultFileExtension = exts[0];
            var file = await picker.PickSaveFileAsync().AsTask();
            return file;
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// 选择文件夹.
    /// </summary>
    /// <param name="windowInstance">窗口实例.</param>
    /// <returns>文件夹.</returns>
    public static async Task<StorageFolder> PickFolderAsync(object windowInstance)
    {
        try
        {
            var picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(windowInstance));
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            var folder = await picker.PickSingleFolderAsync().AsTask();
            return folder;
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// Get local data and convert.
    /// </summary>
    /// <typeparam name="T">Conversion target type.</typeparam>
    /// <param name="fileName">File name.</param>
    /// <param name="defaultValue">The default value when the file does not exist or has no content.</param>
    /// <param name="folderName">The folder to which the file belongs.</param>
    /// <returns>Converted result.</returns>
    public static Task<T?> ReadLocalDataAsync<T>(string fileName, string defaultValue = "{}", string folderName = "") => Task.Run(async () =>
    {
        var path = string.IsNullOrEmpty(folderName) ?
                        $"ms-appdata:///local/{fileName}" :
                        $"ms-appdata:///local/{folderName}/{fileName}";
        var content = defaultValue;
        try
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path))
                    .AsTask();
            var fileContent = await FileIO.ReadTextAsync(file)
                           .AsTask();

            if (!string.IsNullOrEmpty(fileContent))
            {
                content = fileContent;
            }
        }
        catch (FileNotFoundException)
        {
        }

        return typeof(T) == typeof(string) ? (T)content.Clone() : JsonSerializer.Deserialize<T>(content);
    });

    /// <summary>
    /// Write data to local file.
    /// </summary>
    /// <typeparam name="T">Type of data.</typeparam>
    /// <param name="fileName">File name.</param>
    /// <param name="data">Data to be written.</param>
    /// <param name="folderName">The folder to which the file belongs.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static Task WriteLocalDataAsync<T>(string fileName, T data, string folderName = "") => Task.Run(async () =>
    {
        var folder = ApplicationData.Current.LocalFolder;

        if (!string.IsNullOrEmpty(folderName))
        {
            folder = await folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists)
                        .AsTask();
        }

        var writeContent = string.Empty;
        writeContent = data is string ? data.ToString() : JsonSerializer.Serialize(data);

        var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists)
                    .AsTask();

        await FileIO.WriteTextAsync(file, writeContent)
          .AsTask();
    });

    /// <summary>
    /// Delete local data file.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <param name="folderName">The folder to which the file belongs.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static Task DeleteLocalDataAsync(string fileName, string folderName = "") => Task.Run(async () =>
    {
        var folder = ApplicationData.Current.LocalFolder;

        if (!string.IsNullOrEmpty(folderName))
        {
            folder = await folder.CreateFolderAsync(folderName)
                        .AsTask();
        }

        var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists)
                    .AsTask();
        await file.DeleteAsync()
            .AsTask();
    });
}
