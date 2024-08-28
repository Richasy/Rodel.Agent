// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Pages;

/// <summary>
/// 音频生成页面.
/// </summary>
public sealed partial class AudioServicePage : AudioServicePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioServicePage"/> class.
    /// </summary>
    public AudioServicePage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        if (ViewModel.IsAvailableServicesEmpty)
        {
            ViewModel.ResetAvailableAudioServicesCommand.Execute(default);
        }
    }

    private void OnHistoryVisibilityButtonClick(object sender, EventArgs e)
        => ViewModel.IsHistoryColumnManualHide = !ViewModel.IsHistoryColumnManualHide;
}

/// <summary>
/// 音频服务页面基类.
/// </summary>
public abstract class AudioServicePageBase : LayoutPageBase<AudioServicePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioServicePageBase"/> class.
    /// </summary>
    protected AudioServicePageBase() => ViewModel = this.Get<AudioServicePageViewModel>();
}
