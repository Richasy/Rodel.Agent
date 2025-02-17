// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Client;
using System.Diagnostics;
using System.Reflection;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 客户端终结点配置部分.
/// </summary>
public sealed partial class DrawClientEndpointConfigSection : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawClientEndpointConfigSection"/> class.
    /// </summary>
    public DrawClientEndpointConfigSection() => InitializeComponent();

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
        => EndpointBox.Text = (ViewModel.Config as ClientEndpointConfigBase)?.Endpoint ?? string.Empty;

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
                return config;
            }
        }

        return null;
    }
}
