// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Client;
using System.Diagnostics;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 混元配置部分.
/// </summary>
public sealed partial class HunYuanDrawConfigSection : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HunYuanDrawConfigSection"/> class.
    /// </summary>
    public HunYuanDrawConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DrawServiceItemViewModel? oldValue, DrawServiceItemViewModel? newValue)
    {
        if (newValue is not DrawServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= new HunYuanClientConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => SecretBox.Password = ((HunYuanClientConfig)ViewModel.Config).SecretId;

    private void OnSecretBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ((HunYuanClientConfig)ViewModel.Config).SecretId = SecretBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
