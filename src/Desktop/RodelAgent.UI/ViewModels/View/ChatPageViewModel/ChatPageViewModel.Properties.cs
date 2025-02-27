// Copyright (c) Richasy. All rights reserved.

using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.UI.ViewModels.Core;

namespace RodelAgent.UI.ViewModels.View;

public sealed partial class ChatPageViewModel
{
    private readonly ChatSessionViewModel _sessionViewModel;

    [ObservableProperty]
    public partial List<ChatServiceItemViewModel>? Services { get; set; }

    [ObservableProperty]
    public partial ChatServiceItemViewModel? SelectedService { get; set; }

    [ObservableProperty]
    public partial bool IsAgentSectionVisible { get; set; }

    [ObservableProperty]
    public partial bool IsToolSectionVisible { get; set; }

    [ObservableProperty]
    public partial bool IsNoService { get; private set; }

    [ObservableProperty]
    public partial bool IsInitializing { get; set; }
}
