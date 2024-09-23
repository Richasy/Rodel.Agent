// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using System.Text.Json;
using RodelAgent.Models.Constants;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// Azure OpenAI 配置部分.
/// </summary>
public sealed partial class AzureOpenAIChatConfigSection : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIChatConfigSection"/> class.
    /// </summary>
    public AzureOpenAIChatConfigSection()
    {
        InitializeComponent();
        InitializeApiVersion();
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ChatServiceItemViewModel? oldValue, ChatServiceItemViewModel? newValue)
    {
        if (newValue is not ChatServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= new AzureOpenAIClientConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => VersionComboBox.SelectedIndex = (int)((AzureOpenAIClientConfig)ViewModel.Config).Version;

    private void InitializeApiVersion()
    {
        var versions = Enum.GetValues<AzureOpenAIVersion>();
        var json = JsonSerializer.Serialize(versions);
        var stringVersions = JsonSerializer.Deserialize<List<string>>(json);
        VersionComboBox.ItemsSource = stringVersions;
    }

    private void OnVersionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = VersionComboBox.SelectedIndex;
        if (index < 0)
        {
            return;
        }

        var version = (AzureOpenAIVersion)index;
        ((AzureOpenAIClientConfig)ViewModel.Config).Version = version;
    }
}
