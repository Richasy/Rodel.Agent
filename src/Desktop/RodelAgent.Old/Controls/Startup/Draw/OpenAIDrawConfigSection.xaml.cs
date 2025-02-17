// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Client;
using System.Diagnostics;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// OpenAI 配置部分.
/// </summary>
public sealed partial class OpenAIDrawConfigSection : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIDrawConfigSection"/> class.
    /// </summary>
    public OpenAIDrawConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DrawServiceItemViewModel? oldValue, DrawServiceItemViewModel? newValue)
    {
        if (newValue is not DrawServiceItemViewModel newVM)
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
