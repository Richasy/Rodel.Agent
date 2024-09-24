// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using System.Reflection;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 客户端终结点配置部分.
/// </summary>
public sealed partial class ChatClientEndpointConfigSection : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatClientEndpointConfigSection"/> class.
    /// </summary>
    public ChatClientEndpointConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ChatServiceItemViewModel? oldValue, ChatServiceItemViewModel? newValue)
    {
        if (newValue is not ChatServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= CreateCurrentConfig();
        if (newVM.Config is OllamaClientConfig)
        {
            BaseSection.Visibility = Visibility.Collapsed;
        }

        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        EndpointBox.Text = (ViewModel.Config as ClientEndpointConfigBase)?.Endpoint ?? string.Empty;
    }

    private void OnEndpointBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((ClientEndpointConfigBase)ViewModel.Config).Endpoint = EndpointBox.Text;
        ViewModel.CheckCurrentConfig();
    }

    private ClientEndpointConfigBase CreateCurrentConfig()
    {
        var assembly = Assembly.GetAssembly(typeof(ClientEndpointConfigBase));
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ClientEndpointConfigBase)));

        foreach (var type in types)
        {
            if (type.Name.StartsWith(ViewModel.ProviderType.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var config = (ClientEndpointConfigBase)Activator.CreateInstance(type);
                if (config is AnthropicClientConfig)
                {
                    config.Endpoint = ProviderConstants.AnthropicApi;
                }
                else if (config is GeminiClientConfig)
                {
                    config.Endpoint = ProviderConstants.GeminiApi;
                }
                else if (config is OllamaClientConfig)
                {
                    config.Endpoint = ProviderConstants.OllamaApi;
                }

                return config;
            }
        }

        return null;
    }
}
