// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// Chat session main body.
/// </summary>
public sealed partial class ChatSessionMainBody : ChatSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionMainBody"/> class.
    /// </summary>
    public ChatSessionMainBody() => InitializeComponent();

    private async void OnLoaded(object? sender, RoutedEventArgs e)
        => await ViewModel.InitializeAsync(MainView);
}
