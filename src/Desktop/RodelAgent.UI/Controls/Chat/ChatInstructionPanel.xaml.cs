// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// Chat instruction panel.
/// </summary>
public sealed partial class ChatInstructionPanel : ChatSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatInstructionPanel"/> class.
    /// </summary>
    public ChatInstructionPanel() => InitializeComponent();

    private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        => ViewModel.SaveSystemInstructionCommand.Execute(default);
}
