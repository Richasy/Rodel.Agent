// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 启动页面绘图部分.
/// </summary>
public sealed partial class StartupDrawSection : StartupPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupDrawSection"/> class.
    /// </summary>
    public StartupDrawSection() => InitializeComponent();

    private void OnItemClick(object sender, ViewModels.Items.DrawServiceItemViewModel e)
        => ViewModel.SideNavigate(typeof(Pages.Startup.DrawConfigurationPage), e);
}
