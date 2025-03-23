// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;

namespace RodelAgent.UI.Toolkits;

/// <summary>
/// 资源管理工具.
/// </summary>
public sealed class ResourceToolkit : SharedResourceToolkit, IStringResourceToolkit
{
    /// <summary>
    /// Get localized text.
    /// </summary>
    /// <param name="stringName">Resource name corresponding to localized text.</param>
    /// <returns>Localized text.</returns>
    public static string GetLocalizedString(StringNames stringName)
        => GlobalDependencies.Kernel.GetRequiredService<IResourceToolkit>().GetLocalizedString(stringName.ToString());
}
