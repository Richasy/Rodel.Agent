// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class ChatAgentOptionPanel : ChatAgentConfigControlBase
{
    public ChatAgentOptionPanel() => InitializeComponent();

    protected override void OnControlLoaded()
    {
        ViewModel.RequestReloadOptionsUI += OnRequestReloadOptionsUI;
        ViewModel.InjectFunc(OptionsPanel.GetOptions, OptionsPanel.GetStreamOutput, OptionsPanel.GetMaxRounds);
    }

    private void OnRequestReloadOptionsUI(object? sender, EventArgs e)
        => OptionsPanel.ReloadOptionsUI(
            ViewModel.Agent?.UseStreamOutput ?? true,
            ViewModel.Agent?.MaxRounds ?? 0,
            ViewModel.CurrentOptions);

    protected override void OnControlUnloaded()
        => ViewModel.RequestReloadOptionsUI -= OnRequestReloadOptionsUI;
}
