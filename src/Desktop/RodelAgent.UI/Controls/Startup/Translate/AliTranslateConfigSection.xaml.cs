// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelTranslate.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 阿里云的客户端配置部分.
/// </summary>
public sealed partial class AliTranslateConfigSection : TranslateServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AliTranslateConfigSection"/> class.
    /// </summary>
    public AliTranslateConfigSection()
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

        newVM.Config ??= new AliClientConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        SecretBox.Password = (ViewModel.Config as AliClientConfig)?.Secret ?? string.Empty;
    }

    private void OnSecretBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ((AliClientConfig)ViewModel.Config).Secret = SecretBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
