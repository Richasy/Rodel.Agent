// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 启动页面音频部分.
/// </summary>
public sealed partial class StartupAudioSection : StartupPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupAudioSection"/> class.
    /// </summary>
    public StartupAudioSection() => InitializeComponent();

    private void OnItemClick(object sender, ViewModels.Items.AudioServiceItemViewModel e)
        => ViewModel.SideNavigate(typeof(Pages.Startup.AudioConfigurationPage), e);
}
