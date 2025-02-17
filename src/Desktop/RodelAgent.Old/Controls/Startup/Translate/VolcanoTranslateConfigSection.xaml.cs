// Copyright (c) Richasy. All rights reserved.

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
    public VolcanoTranslateConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(TranslateServiceItemViewModel? oldValue, TranslateServiceItemViewModel? newValue)
    {
        if (newValue is not TranslateServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= new VolcanoClientConfig();
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => IdBox.Text = (ViewModel.Config as VolcanoClientConfig)?.KeyId ?? string.Empty;

    private void OnIdBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((VolcanoClientConfig)ViewModel.Config).KeyId = IdBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
