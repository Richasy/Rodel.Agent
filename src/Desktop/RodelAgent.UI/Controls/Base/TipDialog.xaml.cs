// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls;

/// <summary>
/// 提示对话框.
/// </summary>
public sealed partial class TipDialog : AppContentDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TipDialog"/> class.
    /// </summary>
    public TipDialog(string text)
    {
        InitializeComponent();
        TipBlock.Text = text;
    }
}
