// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Items;

/// <summary>
/// 音频模型卡片.
/// </summary>
public sealed class AudioModelCard : ReactiveControl<AudioModelItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioModelCard"/> class.
    /// </summary>
    public AudioModelCard() => DefaultStyleKey = typeof(AudioModelCard);

    /// <summary>
    /// 点击事件.
    /// </summary>
    public event EventHandler<AudioModelItemViewModel> Click;

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        var card = GetTemplateChild("RootCard") as CardPanel;
        if (card != null)
        {
            card.Click += (sender, e) => Click?.Invoke(this, ViewModel);
        }
    }
}
