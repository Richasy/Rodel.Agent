// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Pages.Startup;

/// <summary>
/// 音频会话配置页面.
/// </summary>
public sealed partial class AudioConfigurationPage : AudioConfigurationPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioConfigurationPage"/> class.
    /// </summary>
    public AudioConfigurationPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
        => ViewModel = e.Parameter as AudioServiceItemViewModel;

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        ViewModel.CheckCurrentConfig();
        ViewModel = default;
    }
}

/// <summary>
/// 音频配置页面基类.
/// </summary>
public abstract class AudioConfigurationPageBase : PageBase<AudioServiceItemViewModel>
{
}
