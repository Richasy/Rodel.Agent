// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 聊天服务页头部.
/// </summary>
public sealed partial class ChatServiceHeader : ChatServicePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServiceHeader"/> class.
    /// </summary>
    public ChatServiceHeader() => InitializeComponent();

    private FluentIcons.Common.IconVariant GetIconVariant(bool isSelected)
    {
        _ = this;
        return isSelected ? FluentIcons.Common.IconVariant.Filled : FluentIcons.Common.IconVariant.Regular;
    }

    private void OnServiceButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.IsServiceSectionVisible = true;

    private void OnPresetButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.IsPluginSectionVisible = true;
}
