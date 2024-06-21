// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelTranslate.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 火山的客户端配置部分.
/// </summary>
public sealed partial class VolcanoTranslateConfigSection : TranslateServiceConfigControlBase
{
    /// <summary>
    /// InitiVolcanozes a new instance of the <see cref="VolcanoTranslateConfigSection"/> class.
    /// </summary>
    public VolcanoTranslateConfigSection()
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

        newVM.Config ??= new VolcanoClientConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        IdBox.Text = (ViewModel.Config as VolcanoClientConfig)?.KeyId ?? string.Empty;
    }

    private void OnIdBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((VolcanoClientConfig)ViewModel.Config).KeyId = IdBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
