// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using RodelAudio.Models.Client;
using System.Diagnostics;
using System.Reflection;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 仅包含密钥的配置部分.
/// </summary>
public sealed partial class AudioClientConfigSection : AudioServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioClientConfigSection"/> class.
    /// </summary>
    public AudioClientConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(AudioServiceItemViewModel? oldValue, AudioServiceItemViewModel? newValue)
    {
        if (newValue is not AudioServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= CreateCurrentConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private ClientConfigBase? CreateCurrentConfig()
    {
        var assembly = Assembly.GetAssembly(typeof(ClientConfigBase));
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ClientConfigBase)));

        foreach (var type in types)
        {
            if (type.Name.StartsWith(ViewModel.ProviderType.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return (ClientConfigBase)Activator.CreateInstance(type);
            }
        }

        return null;
    }
}
