// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using Windows.ApplicationModel.Resources.Core;

namespace RodelAgent.UI.Toolkits;

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
        => ResourceManager.Current.MainResourceMap.GetValueOrDefault($"Resources/{stringName}")?.Candidates?.FirstOrDefault()?.ValueAsString ?? string.Empty;
}

/// <summary>
/// 字符串资源工具箱.
/// </summary>
public class StringResourceToolkit : IStringResourceToolkit
{
    /// <inheritdoc/>
    public string GetString(string key)
        => ResourceManager.Current.MainResourceMap.GetValueOrDefault($"Resources/{key}")?.Candidates?.FirstOrDefault()?.ValueAsString ?? string.Empty;
}
