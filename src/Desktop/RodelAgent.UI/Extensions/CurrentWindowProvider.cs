// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.Share;
using RodelAgent.UI.ViewModels.Core;

namespace RodelAgent.UI.Extensions;

internal sealed class CurrentWindowProvider : ICurrentWindowProvider
{
    public Window CurrentWindow
        => this.Get<AppViewModel>().ActivatedWindow;
}
