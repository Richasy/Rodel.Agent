// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 千帆配置部分.
/// </summary>
public sealed partial class QianFanDrawConfigSection : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QianFanDrawConfigSection"/> class.
    /// </summary>
    public QianFanDrawConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DrawServiceItemViewModel? oldValue, DrawServiceItemViewModel? newValue)
    {
        if (newValue is not DrawServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= new QianFanClientConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => SecretBox.Password = ((QianFanClientConfig)ViewModel.Config).Secret;

    private void OnSecretBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ((QianFanClientConfig)ViewModel.Config).Secret = SecretBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
