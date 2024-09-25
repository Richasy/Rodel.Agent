// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 系统指令面板.
/// </summary>
public sealed partial class SystemInstructionPanel : ChatSessionControlBase
{
    private bool _textChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemInstructionPanel"/> class.
    /// </summary>
    public SystemInstructionPanel() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ChatSessionViewModel? oldValue, ChatSessionViewModel? newValue)
        => UpdateInstruction();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => UpdateInstruction();

    private void UpdateInstruction()
    {
        SystemBox.Text = ViewModel?.Data?.SystemInstruction ?? string.Empty;
        _textChanged = false;
    }

    private void OnSystemBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        ViewModel.Data.SystemInstruction = SystemBox.Text;
        ViewModel.ResetLastInputTimeCommand.Execute(default);
        _textChanged = true;
    }

    private void OnSystemBoxLostFocus(object sender, RoutedEventArgs e)
    {
        if (_textChanged && ViewModel is not null)
        {
            ViewModel.SaveSessionToDatabaseCommand.Execute(default);
        }

        _textChanged = false;
    }
}
