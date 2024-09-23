// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelTranslate.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 腾讯云的客户端配置部分.
/// </summary>
public sealed partial class TencentTranslateConfigSection : TranslateServiceConfigControlBase
{
    /// <summary>
    /// InitiTencentTranslatezes a new instance of the <see cref="TencentTranslateConfigSection"/> class.
    /// </summary>
    public TencentTranslateConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(TranslateServiceItemViewModel? oldValue, TranslateServiceItemViewModel? newValue)
    {
        if (newValue is not TranslateServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= new TencentClientConfig();
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => SecretBox.Password = (ViewModel.Config as TencentClientConfig)?.SecretId ?? string.Empty;

    private void OnSecretBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ((TencentClientConfig)ViewModel.Config).SecretId = SecretBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
