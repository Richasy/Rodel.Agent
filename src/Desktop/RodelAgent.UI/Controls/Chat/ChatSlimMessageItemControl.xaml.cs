// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Models.Feature;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 简化的聊天消息项.
/// </summary>
public sealed partial class ChatSlimMessageItemControl : LayoutUserControlBase
{
    /// <summary>
    /// <see cref="ViewModel"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(ChatInteropMessage), typeof(ChatSlimMessageItemControl), new PropertyMetadata(default, new PropertyChangedCallback(OnViewModelChanged)));

    public ChatSlimMessageItemControl() => InitializeComponent();

    /// <summary>
    /// 视图模型.
    /// </summary>
    public ChatInteropMessage ViewModel
    {
        get => (ChatInteropMessage)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    protected override void OnControlLoaded()
        => CheckState();

    private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as ChatSlimMessageItemControl;
        instance?.CheckState();
    }

    private void CheckState()
    {
        var stateName = ViewModel.Role.Equals("system", StringComparison.OrdinalIgnoreCase)
            ? nameof(SystemState)
            : ViewModel.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase)
                ? nameof(AssistantState)
                : nameof(UserState);

        VisualStateManager.GoToState(this, stateName, false);
    }
}