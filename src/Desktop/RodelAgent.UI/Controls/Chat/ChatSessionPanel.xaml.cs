﻿// Copyright (c) Richasy. All rights reserved.


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

    protected override void OnControlLoaded()
    {
        ViewModel.RequestReloadOptionsUI += OnRequestReloadOptionsUI;
        ViewModel.InjectFunc(OptionsPanel.GetOptions, OptionsPanel.GetStreamOutput, OptionsPanel.GetMaxRounds);
    }

    private void OnRequestReloadOptionsUI(object? sender, EventArgs e)
        => OptionsPanel.ReloadOptionsUI(
            ViewModel.GetCurrentConversation()?.UseStreamOutput ?? true,
            ViewModel.GetCurrentConversation()?.MaxRounds ?? 0,
            ViewModel.CurrentOptions);

    protected override void OnControlUnloaded()
        => ViewModel.RequestReloadOptionsUI -= OnRequestReloadOptionsUI;

    private void OnSideGridSizeChanged(object sender, SizeChangedEventArgs e)
        => ExtraSizer.Maximum = e.NewSize.Height - 100;
}
