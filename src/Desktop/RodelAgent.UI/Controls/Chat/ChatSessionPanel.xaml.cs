// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// Chat session panel.
/// </summary>
public sealed partial class ChatSessionPanel : ChatSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionPanel"/> class.
    /// </summary>
    public ChatSessionPanel() => InitializeComponent();

    private void OnSideGridSizeChanged(object sender, SizeChangedEventArgs e)
        => ExtraSizer.Maximum = e.NewSize.Height - 100;
}
