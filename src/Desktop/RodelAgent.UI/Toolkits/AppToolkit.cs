// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using System.Net.NetworkInformation;
using RodelAgent.UI.Models.Constants;
using Windows.ApplicationModel;
using Windows.Graphics;

namespace RodelAgent.UI.Toolkits;

/// <summary>
/// 应用工具箱.
/// </summary>
public static class AppToolkit
{
    private static readonly string[] _supportImageExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".ico" };

    /// <summary>
    /// 获取应用包版本.
    /// </summary>
    /// <returns>包版本.</returns>
    public static string GetPackageVersion()
    {
        var appVersion = Package.Current.Id.Version;
        return $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}.{appVersion.Revision}";
    }

    /// <summary>
    /// 重置控件主题.
    /// </summary>
    /// <param name="element">控件.</param>
    public static void ResetControlTheme(FrameworkElement element)
    {
        var localTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        element.RequestedTheme = localTheme;
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
    /// 杀掉占用当前端口的进程.
    /// </summary>
    /// <param name="port">端口号.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task KillProcessIfUsingPortAsync(int port)
    {
        var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        var listeners = ipGlobalProperties.GetActiveTcpListeners();
        var hasProcess = listeners.Any(p => p.Port == port);
        if (hasProcess)
        {
            await Task.Run(() =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-Command \"Get-NetTCPConnection -LocalPort {port} | Select-Object -ExpandProperty OwningProcess | ForEach-Object {{ Stop-Process -Id $_ -Force }}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    },
                };

                _ = process.Start();
                process.WaitForExit();
            });
        }
    }

    /// <summary>
    /// 获取预设头像路径.
    /// </summary>
    /// <param name="id">预设 Id.</param>
    /// <returns>路径.</returns>
    public static string GetPresetAvatarPath(string id)
    {
        var workDir = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
        var avatarPath = Path.Combine(workDir, "Avatars", $"{id}.png");
        return avatarPath;
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
    public static string GetPluginAvatarPath(string id)
    {
        var actualId = id.Split("<|>").First();
        var pluginFolder = Path.Combine(GetChatPluginFolder(), actualId);
        if (!Directory.Exists(pluginFolder))
        {
            return string.Empty;
        }

        var files = Directory.GetFiles(pluginFolder);
        var logoFile = files.FirstOrDefault(p => Path.GetFileName(p).StartsWith("favicon", StringComparison.InvariantCultureIgnoreCase) && _supportImageExtensions.Contains(Path.GetExtension(p)));
        return logoFile;
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
    public static string GetSpeechFolderPath()
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
    /// <returns>音频路径.</returns>
    public static string GetSpeechPath(string id)
    {
        var drawFolder = GetSpeechFolderPath();
        return Path.Combine(drawFolder, $"{id}.wav");
    }
}
