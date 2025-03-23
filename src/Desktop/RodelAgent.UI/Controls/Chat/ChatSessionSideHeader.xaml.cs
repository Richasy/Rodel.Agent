// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class ChatSessionSideHeader : ChatSessionControlBase
{
    public ChatSessionSideHeader() => InitializeComponent();

    private void OnInstructionButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.IsInstructionVisible = true;

    private void OnOptionsButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.IsSessionOptionsVisible = true;
}
