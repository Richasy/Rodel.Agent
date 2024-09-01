// Copyright (c) Rodel. All rights reserved.

using Microsoft.Windows.ApplicationModel.Resources;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;

namespace RodelAgent.UI.Toolkits;

/// <summary>
/// 资源管理工具.
/// </summary>
public static class ResourceToolkit
{
    private static ResourceLoader _loader;

    /// <summary>
    /// Get localized text.
    /// </summary>
    /// <param name="stringName">Resource name corresponding to localized text.</param>
    /// <returns>Localized text.</returns>
    public static string GetLocalizedString(StringNames stringName)
    {
        _loader ??= new ResourceLoader(ResourceLoader.GetDefaultResourceFilePath(), "Resources");
        return _loader.GetString(stringName.ToString());
    }
}

/// <summary>
/// 字符串资源工具箱.
/// </summary>
public class StringResourceToolkit : IStringResourceToolkit
{
    private ResourceLoader _loader;

    /// <inheritdoc/>
    public string GetString(string key)
    {
        _loader ??= new ResourceLoader(ResourceLoader.GetDefaultResourceFilePath(), "Resources");
        return _loader.GetString(key);
    }
}
