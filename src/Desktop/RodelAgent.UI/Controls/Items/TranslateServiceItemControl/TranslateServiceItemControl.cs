// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Items;

/// <summary>
/// 翻译服务条目控件.
/// </summary>
public sealed class TranslateServiceItemControl : LayoutControlBase<TranslateServiceItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateServiceItemControl"/> class.
    /// </summary>
    public TranslateServiceItemControl() => DefaultStyleKey = typeof(TranslateServiceItemControl);

    /// <summary>
    /// 点击事件.
    /// </summary>
    public event EventHandler<TranslateServiceItemViewModel> Click;

    /// <summary>
    /// 修改 LOGO 的高度.
    /// </summary>
    /// <param name="height">高度.</param>
    public void ChangeLogoHeight(double height)
    {
        var logo = GetTemplateChild("Logo") as ProviderLogo;
        if (logo != null)
        {
            logo.Height = height;
        }
    }

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
