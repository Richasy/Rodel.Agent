// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Translation;

/// <summary>
/// 翻译服务页面控件基类.
/// </summary>
public sealed partial class TranslationServiceHeader : TranslateServicePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationServiceHeader"/> class.
    /// </summary>
    public TranslationServiceHeader()
    {
        InitializeComponent();
    }

    private void OnServiceSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var service = (sender as ComboBox)?.SelectedItem as TranslateServiceItemViewModel;
        if (service == null || ViewModel.Session?.TranslateService == service)
        {
            return;
        }

        ViewModel.SetSelectedTranslateServiceCommand.Execute(service);
    }

    private void OnHistoryButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
}
