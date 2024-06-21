// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Draw;

/// <summary>
/// 绘图提供者选项面板.
/// </summary>
public sealed partial class DrawProviderOptionPanel : DrawSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawProviderOptionPanel"/> class.
    /// </summary>
    public DrawProviderOptionPanel()
    {
        InitializeComponent();
    }

    private void OnModelClick(object sender, DrawModelItemViewModel e)
    {
        ViewModel.ChangeModelCommand.Execute(e);
    }
}
