// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Internal;

/// <summary>
/// 提示词测试页面侧边栏.
/// </summary>
public sealed partial class PromptTestSideBody : PromptTestPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptTestSideBody"/> class.
    /// </summary>
    public PromptTestSideBody() => InitializeComponent();

    private void OnServiceSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var service = e.AddedItems.OfType<ChatServiceItemViewModel>().FirstOrDefault();
        if (service != null)
        {
            ViewModel?.ChangeServiceCommand.Execute(service);
        }
    }

    private void OnModelSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var model = e.AddedItems.OfType<ChatModelItemViewModel>().FirstOrDefault();
        if (model != null)
        {
            ViewModel?.ChangeModelCommand.Execute(model);
        }
    }
}
