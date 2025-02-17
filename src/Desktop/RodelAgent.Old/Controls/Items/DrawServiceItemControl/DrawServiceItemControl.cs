// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Items;

/// <summary>
/// 绘图服务项控件.
/// </summary>
public sealed class DrawServiceItemControl : LayoutControlBase<DrawServiceItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawServiceItemControl"/> class.
    /// </summary>
    public DrawServiceItemControl() => DefaultStyleKey = typeof(DrawServiceItemControl);

    /// <summary>
    /// 点击事件.
    /// </summary>
    public event EventHandler<DrawServiceItemViewModel> Click;

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
