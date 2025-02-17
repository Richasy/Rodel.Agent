// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using System.Diagnostics;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 混元配置部分.
/// </summary>
public sealed partial class HunYuanChatConfigSection : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HunYuanChatConfigSection"/> class.
    /// </summary>
    public HunYuanChatConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ChatServiceItemViewModel? oldValue, ChatServiceItemViewModel? newValue)
    {
        if (newValue is not ChatServiceItemViewModel newVM)
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
