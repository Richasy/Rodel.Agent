// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelTranslate.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// Azure翻译的客户端配置部分.
/// </summary>
public sealed partial class AzureTranslateConfigSection : TranslateServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureTranslateConfigSection"/> class.
    /// </summary>
    public AzureTranslateConfigSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        var newVM = e.NewValue as TranslateServiceItemViewModel;
        if (newVM == null)
        {
            return;
        }

        newVM.Config ??= new AzureClientConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RegionBox.Text = (ViewModel.Config as AzureClientConfig)?.Region ?? string.Empty;
    }

    private void OnRegionBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((AzureClientConfig)ViewModel.Config).Region = RegionBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
