// Copyright (c) Rodel. All rights reserved.

using Windows.ApplicationModel.Resources.Core;

namespace RodelChat.UI.Toolkits;

/// <summary>
/// 资源管理工具.
/// </summary>
public static class ResourceToolkit
{
    /// <summary>
    /// Get localized text.
    /// </summary>
    /// <param name="stringName">Resource name corresponding to localized text.</param>
    /// <returns>Localized text.</returns>
    public static string GetLocalizedString(StringNames stringName)
        => ResourceManager.Current.MainResourceMap[$"Resources/{stringName}"].Candidates[0].ValueAsString;

    /// <summary>
    /// 获取助理头像地址（可能不存在）.
    /// </summary>
    /// <param name="assistantId">助理 Id.</param>
    /// <returns>路径.</returns>
    public static string GetAssistantAvatarPath(string assistantId)
    {
        var libPath = SettingsToolkit.ReadLocalSetting(SettingNames.LibraryFolderPath, string.Empty);
        var avatarPath = Path.Combine(libPath, "Assistants", assistantId + ".png");
        return avatarPath;
    }
}
