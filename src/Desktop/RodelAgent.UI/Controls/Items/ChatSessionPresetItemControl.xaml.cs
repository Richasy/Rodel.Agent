// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Items;

/// <summary>
/// 聊天预设项控件.
/// </summary>
public sealed partial class ChatSessionPresetItemControl : ChatSessionPresetItemControlBase
{
    /// <summary>
    /// <see cref="DefaultSymbol"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty DefaultSymbolProperty =
        DependencyProperty.Register(nameof(DefaultSymbol), typeof(FluentIcons.Common.Symbol), typeof(ChatSessionPresetItemControl), new PropertyMetadata(FluentIcons.Common.Symbol.Bot));

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionPresetItemControl"/> class.
    /// </summary>
    public ChatSessionPresetItemControl() => InitializeComponent();

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

    private void OnClick(object sender, RoutedEventArgs e)
        => Click?.Invoke(this, ViewModel);
}

/// <summary>
/// 聊天预设项控件基类.
/// </summary>
public abstract class ChatSessionPresetItemControlBase : ReactiveUserControl<ChatPresetItemViewModel>
{
}
