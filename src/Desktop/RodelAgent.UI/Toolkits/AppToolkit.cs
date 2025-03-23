// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.UI.Models.Constants;
using Windows.Globalization;
using Windows.Graphics;

namespace RodelAgent.UI.Toolkits;

/// <summary>
/// 应用工具箱.
/// </summary>
internal sealed class AppToolkit : SharedAppToolkit
{
    private static readonly string[] _supportImageExtensions = [".png", ".jpg", ".jpeg", ".bmp", ".gif", ".ico"];

    /// <summary>
    /// Initializes a new instance of the <see cref="AppToolkit"/> class.
    /// </summary>
    public AppToolkit(ISettingsToolkit settings)
        : base(settings)
    {
    }

    /// <summary>
    /// 将 <see cref="Rect"/> 转换为 <see cref="RectInt32"/>.
    /// </summary>
    /// <param name="rect">矩形对象.</param>
    /// <param name="scaleFactor">缩放比例.</param>
    /// <returns><see cref="RectInt32"/>.</returns>
    public static RectInt32 GetRectInt32(Rect rect, double scaleFactor)
        => new(
              Convert.ToInt32(rect.X * scaleFactor),
              Convert.ToInt32(rect.Y * scaleFactor),
              Convert.ToInt32(rect.Width * scaleFactor),
              Convert.ToInt32(rect.Height * scaleFactor));

    /// <summary>
    /// 获取预设头像路径.
    /// </summary>
    /// <param name="id">预设 Id.</param>
    /// <returns>路径.</returns>
    public static string GetPresetAvatarPath(string id)
    {
        var workDir = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
        return Path.Combine(workDir, "Avatars", $"{id}.png");
    }

    /// <summary>
    /// 获取插件文件夹.
    /// </summary>
    /// <returns>路径.</returns>
    public static string GetChatPluginFolder()
    {
        var workDir = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
        return Path.Combine(workDir, "Plugins");
    }

    /// <summary>
    /// 获取插件头像路径.
    /// </summary>
    /// <param name="id">插件 ID.</param>
    /// <returns>插件路径.</returns>
    public static string? GetPluginAvatarPath(string id)
    {
        var actualId = id.Split("<|>").First();
        var pluginFolder = Path.Combine(GetChatPluginFolder(), actualId);
        if (!Directory.Exists(pluginFolder))
        {
            return string.Empty;
        }

        var files = Directory.GetFiles(pluginFolder);
        return Array.Find(files, p => Path.GetFileName(p).StartsWith("favicon", StringComparison.InvariantCultureIgnoreCase) && _supportImageExtensions.Contains(Path.GetExtension(p)));
    }

    /// <summary>
    /// 获取绘图文件夹.
    /// </summary>
    /// <returns>文件夹路径.</returns>
    public static string GetDrawFolderPath()
    {
        var workDir = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
        return Path.Combine(workDir, "Draw");
    }

    /// <summary>
    /// 获取语音文件夹.
    /// </summary>
    /// <returns>文件夹路径.</returns>
    public static string GetAudioFolderPath()
    {
        var workDir = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
        return Path.Combine(workDir, "Speech");
    }

    /// <summary>
    /// 获取绘图图片路径.
    /// </summary>
    /// <param name="id">图片 Id.</param>
    /// <returns>图片路径.</returns>
    public static string GetDrawPicturePath(string id)
    {
        var drawFolder = GetDrawFolderPath();
        return Path.Combine(drawFolder, $"{id}.png");
    }

    /// <summary>
    /// 获取生成音频的路径.
    /// </summary>
    /// <param name="id">音频 Id.</param>
    /// <param name="forceWav">是否强制为 wav 格式.</param>
    /// <returns>音频路径.</returns>
    public static string GetAudioPath(string id, bool forceWav = false)
    {
        var folder = GetAudioFolderPath();

        if (forceWav)
        {
            return Path.Combine(folder, $"{id}.wav");
        }

        // 查找文件夹下包含此id的文件.
        var files = Directory.GetFiles(folder);
        var file = Array.Find(files, p => Path.GetFileName(p).StartsWith(id, StringComparison.InvariantCultureIgnoreCase));
        return file ?? string.Empty;
    }

    public static string GetDocumentLink(string path)
    {
        var isEn = ApplicationLanguages.Languages[0].StartsWith("en", StringComparison.OrdinalIgnoreCase);
        var baseUrl = isEn ? "https://agent.richasy.net/en" : "https://agent.richasy.net";
        return $"{baseUrl}/{path.TrimStart('/')}";
    }
}
