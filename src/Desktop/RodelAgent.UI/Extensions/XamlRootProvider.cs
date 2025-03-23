// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.Share;
using RodelAgent.UI.ViewModels.Core;

namespace RodelAgent.UI.Extensions;

internal sealed class XamlRootProvider : IXamlRootProvider
{
    public XamlRoot? XamlRoot => GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().ActivatedWindow.Content.XamlRoot;
}
