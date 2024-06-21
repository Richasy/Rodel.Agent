// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// OpenAI 配置部分.
/// </summary>
public sealed partial class OpenAIChatConfigSection : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIChatConfigSection"/> class.
    /// </summary>
    public OpenAIChatConfigSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        var newVM = e.NewValue as ChatServiceItemViewModel;
        if (newVM == null)
        {
            return;
        }

        newVM.Config ??= CreateCurrentConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    private static OpenAIClientConfig CreateCurrentConfig()
    {
        var config = new OpenAIClientConfig
        {
            Endpoint = ProviderConstants.OpenAIApi,
            OrganizationId = string.Empty,
        };
        return config;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => OrganizationBox.Text = ((OpenAIClientConfig)ViewModel.Config).OrganizationId;

    private void OnOrganizationBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((OpenAIClientConfig)ViewModel.Config).OrganizationId = OrganizationBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
