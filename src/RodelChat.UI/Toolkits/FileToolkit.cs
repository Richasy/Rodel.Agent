// Copyright (c) Rodel. All rights reserved.

using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace RodelChat.UI.Toolkits;

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
    /// 选择文件.
    /// </summary>
    /// <param name="extension">扩展名.</param>
    /// <param name="windowInstance">窗口对象.</param>
    /// <returns>文件对象（可能为空）.</returns>
    public static async Task<List<StorageFile>> PickFilesAsync(string extension, object windowInstance)
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
            var files = await picker.PickMultipleFilesAsync().AsTask();
            return files.ToList();
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// 保存文件.
    /// </summary>
    /// <param name="extension">扩展名.</param>
    /// <param name="suggestName">建议名称.</param>
    /// <param name="windowInstance">窗口实例.</param>
    /// <returns>文件.</returns>
    public static async Task<StorageFile> SaveFileAsync(string extension, string suggestName, object windowInstance)
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

            if (!string.IsNullOrEmpty(suggestName))
            {
                picker.SuggestedFileName = suggestName;
            }

            picker.SuggestedStartLocation = PickerLocationId.Desktop;
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
}
