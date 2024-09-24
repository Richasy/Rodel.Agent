// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls.Internal;

/// <summary>
/// 提示词测试页面主体.
/// </summary>
public sealed partial class PromptTestMainBody : PromptTestPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptTestMainBody"/> class.
    /// </summary>
    public PromptTestMainBody() => InitializeComponent();

    private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
    {
        ViewModel?.UpdatePromptVariablesCommand.Execute(default);
    }
}
