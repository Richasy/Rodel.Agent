﻿// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 星火配置部分.
/// </summary>
public sealed partial class SparkDeskDrawConfigSection : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkDeskDrawConfigSection"/> class.
    /// </summary>
    public SparkDeskDrawConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not DrawServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= new SparkDeskClientConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => AppIdBox.Text = ((SparkDeskClientConfig)ViewModel.Config).AppId;

    private void OnAppIdBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((SparkDeskClientConfig)ViewModel.Config).AppId = AppIdBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
