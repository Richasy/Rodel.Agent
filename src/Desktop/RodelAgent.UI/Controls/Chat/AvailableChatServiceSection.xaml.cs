// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 可用聊天服务部分.
/// </summary>
public sealed partial class AvailableChatServiceSection : ChatServicePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AvailableChatServiceSection"/> class.
    /// </summary>
    public AvailableChatServiceSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel.AvailableServices.Count == 0)
        {
            ViewModel.ResetAvailableChatServicesCommand.Execute(default);
        }
    }

    private void OnServiceItemClick(object sender, ViewModels.Items.ChatServiceItemViewModel e)
        => ViewModel.SetSelectedChatServiceCommand.Execute(e);
}
