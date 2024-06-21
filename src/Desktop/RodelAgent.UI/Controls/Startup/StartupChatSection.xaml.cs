// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 启动页面聊天部分.
/// </summary>
public sealed partial class StartupChatSection : StartupPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupChatSection"/> class.
    /// </summary>
    public StartupChatSection() => InitializeComponent();

    private void OnItemClick(object sender, ViewModels.Items.ChatServiceItemViewModel e)
        => ViewModel.SideNavigate(typeof(Pages.Startup.ChatConfigurationPage), e);
}
