// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using System.Diagnostics;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// OpenAI 配置部分.
/// </summary>
public sealed partial class OpenAIChatConfigSection : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIChatConfigSection"/> class.
    /// </summary>
    public OpenAIChatConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ChatServiceItemViewModel? oldValue, ChatServiceItemViewModel? newValue)
    {
        if (newValue is not ChatServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= CreateCurrentConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => OrganizationBox.Text = ((OpenAIClientConfig)ViewModel.Config).OrganizationId;

    private static OpenAIClientConfig CreateCurrentConfig()
    {
        var config = new OpenAIClientConfig
        {
            Endpoint = ProviderConstants.OpenAIApi,
            OrganizationId = string.Empty,
        };
        return config;
    }

    private void OnOrganizationBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((OpenAIClientConfig)ViewModel.Config).OrganizationId = OrganizationBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
