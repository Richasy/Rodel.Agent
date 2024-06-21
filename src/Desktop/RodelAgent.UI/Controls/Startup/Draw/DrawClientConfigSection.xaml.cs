// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using System.Reflection;
using RodelAgent.UI.ViewModels.Items;
using RodelDraw.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 仅包含密钥的配置部分.
/// </summary>
public sealed partial class DrawClientConfigSection : DrawServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawClientConfigSection"/> class.
    /// </summary>
    public DrawClientConfigSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        var newVM = e.NewValue as DrawServiceItemViewModel;
        if (newVM == null)
        {
            return;
        }

        newVM.Config ??= CreateCurrentConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }

    private ClientConfigBase CreateCurrentConfig()
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
