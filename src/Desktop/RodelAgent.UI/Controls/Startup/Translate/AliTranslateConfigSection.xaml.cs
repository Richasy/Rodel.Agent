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
    public AliTranslateConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(TranslateServiceItemViewModel? oldValue, TranslateServiceItemViewModel? newValue)
    {
        if (newValue is not TranslateServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= new AliClientConfig();
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => SecretBox.Password = (ViewModel.Config as AliClientConfig)?.Secret ?? string.Empty;

    private void OnSecretBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ((AliClientConfig)ViewModel.Config).Secret = SecretBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
