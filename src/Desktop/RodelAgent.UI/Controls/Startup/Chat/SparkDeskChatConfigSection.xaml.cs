// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 星火配置部分.
/// </summary>
public sealed partial class SparkDeskChatConfigSection : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkDeskChatConfigSection"/> class.
    /// </summary>
    public SparkDeskChatConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not ChatServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= new SparkDeskClientConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        SecretBox.Password = ((SparkDeskClientConfig)ViewModel.Config).Secret;
        AppIdBox.Text = ((SparkDeskClientConfig)ViewModel.Config).AppId;
    }

    private void OnAppIdBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((SparkDeskClientConfig)ViewModel.Config).AppId = AppIdBox.Text;
        ViewModel.CheckCurrentConfig();
    }

    private void OnSecretBoxTextChanged(object sender, RoutedEventArgs e)
    {
        ((SparkDeskClientConfig)ViewModel.Config).Secret = SecretBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
