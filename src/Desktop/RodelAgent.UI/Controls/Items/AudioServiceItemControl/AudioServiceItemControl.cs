// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Items;

/// <summary>
/// 音频服务项控件.
/// </summary>
public sealed class AudioServiceItemControl : LayoutControlBase<AudioServiceItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioServiceItemControl"/> class.
    /// </summary>
    public AudioServiceItemControl() => DefaultStyleKey = typeof(AudioServiceItemControl);

    /// <summary>
    /// 点击事件.
    /// </summary>
    public event EventHandler<AudioServiceItemViewModel> Click;

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        var rootCard = GetTemplateChild("RootCard") as CardPanel;
        if (rootCard != null)
        {
            rootCard.Click += OnClick;
        }
    }

    private void OnClick(object sender, RoutedEventArgs e)
        => Click?.Invoke(this, ViewModel);
}
