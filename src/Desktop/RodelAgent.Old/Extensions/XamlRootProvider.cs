// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.Share;
using RodelAgent.UI.ViewModels;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// XamlRoot Provider.
/// </summary>
public sealed class XamlRootProvider : IXamlRootProvider
{
    /// <inheritdoc/>
    public XamlRoot? XamlRoot => GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().ActivatedWindow.Content.XamlRoot;
}
