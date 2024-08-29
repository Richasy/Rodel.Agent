// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Items;

/// <summary>
/// 聊天预设项控件.
/// </summary>
public sealed partial class ChatSessionPresetItemControl : LayoutControlBase<ChatPresetItemViewModel>
{
    /// <summary>
    /// <see cref="DefaultSymbol"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty DefaultSymbolProperty =
        DependencyProperty.Register(nameof(DefaultSymbol), typeof(FluentIcons.Common.Symbol), typeof(ChatSessionPresetItemControl), new PropertyMetadata(FluentIcons.Common.Symbol.Bot));

    private CardPanel _rootCard;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionPresetItemControl"/> class.
    /// </summary>
    public ChatSessionPresetItemControl()
    {
        DefaultStyleKey = typeof(ChatSessionPresetItemControl);
    }

    /// <summary>
    /// 条目被点击.
    /// </summary>
    public event EventHandler<ChatPresetItemViewModel> Click;

    /// <summary>
    /// 默认图标.
    /// </summary>
    public FluentIcons.Common.Symbol DefaultSymbol
    {
        get => (FluentIcons.Common.Symbol)GetValue(DefaultSymbolProperty);
        set => SetValue(DefaultSymbolProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        var rootCard = GetTemplateChild("RootCard") as CardPanel;
        if (rootCard != null)
        {
            _rootCard = rootCard;
            _rootCard.Click += OnClick;
        }
    }

    private void OnClick(object sender, RoutedEventArgs e)
        => Click?.Invoke(this, ViewModel);
}
