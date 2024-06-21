// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 混元配置部分.
/// </summary>
public sealed partial class HunYuanDrawConfigSection : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HunYuanDrawConfigSection"/> class.
    /// </summary>
    public HunYuanDrawConfigSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        var newVM = e.NewValue as DrawServiceItemViewModel;
        if (newVM == null)
        {
            return;
        }

        newVM.Config ??= new HunYuanClientConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => SecretBox.Password = ((HunYuanClientConfig)ViewModel.Config).SecretId;

    private void OnSecretBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ((HunYuanClientConfig)ViewModel.Config).SecretId = SecretBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
