// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// Chat side header.
/// </summary>
public sealed partial class ChatSideHeader : ChatPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSideHeader"/> class.
    /// </summary>
    public ChatSideHeader() => InitializeComponent();

    private void OnAgentButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.IsAgentSectionVisible = true;

    private void OnToolButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.IsToolSectionVisible = true;
}
