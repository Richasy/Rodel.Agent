// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 聊天群选项面板.
/// </summary>
public sealed partial class GroupOptionsPanel : ChatGroupControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupOptionsPanel"/> class.
    /// </summary>
    public GroupOptionsPanel()
    {
        InitializeComponent();
    }

    private void OnMaxRoundsChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (e.NewValue < 1 || ViewModel is null)
        {
            return;
        }

        ViewModel.MaxRounds = (int)e.NewValue;
        ViewModel.CheckMaxRoundsCommand.Execute(default);
    }

    private void OnTerminateTextSubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        => ViewModel.CheckMaxRoundsCommand.Execute(default);
}
