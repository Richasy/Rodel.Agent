// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Pages.Startup;

/// <summary>
/// 聊天会话配置页面.
/// </summary>
public sealed partial class ChatConfigurationPage : ChatConfigurationPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatConfigurationPage"/> class.
    /// </summary>
    public ChatConfigurationPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
        => ViewModel = e.Parameter as ChatServiceItemViewModel;

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        ViewModel.CheckCurrentConfig();
        ViewModel = default;
    }
}

/// <summary>
/// 聊天配置页面基类.
/// </summary>
public abstract class ChatConfigurationPageBase : LayoutPageBase<ChatServiceItemViewModel>
{
}
