// Copyright (c) Rodel. All rights reserved.

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
    public SystemInstructionPanel()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
        => UpdateInstruction();

    private void OnLoaded(object sender, RoutedEventArgs e)
        => UpdateInstruction();

    private void UpdateInstruction()
    {
        SystemBox.Text = ViewModel?.Data?.SystemInstruction ?? string.Empty;
        _textChanged = false;
    }

    private void OnSystemBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ViewModel.Data.SystemInstruction = SystemBox.Text;
        _textChanged = true;
    }

    private void OnSystemBoxLostFocus(object sender, RoutedEventArgs e)
    {
        if (_textChanged)
        {
            ViewModel.SaveSessionToDatabaseCommand.Execute(default);
        }

        _textChanged = false;
    }
}
