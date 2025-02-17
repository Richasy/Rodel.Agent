// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Draw;

/// <summary>
/// 翻译服务页面控件基类.
/// </summary>
public sealed partial class DrawServiceHeader : DrawServicePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawServiceHeader"/> class.
    /// </summary>
    public DrawServiceHeader() => InitializeComponent();

    private void OnServiceSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var service = (sender as ComboBox)?.SelectedItem as DrawServiceItemViewModel;
        if (service == null || ViewModel.Session?.DrawService == service)
        {
            return;
        }

        ViewModel.SetSelectedDrawServiceCommand.Execute(service);
    }
}
