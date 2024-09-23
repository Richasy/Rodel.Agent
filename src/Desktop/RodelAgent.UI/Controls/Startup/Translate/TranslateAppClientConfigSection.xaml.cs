// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using System.Reflection;
using RodelAgent.UI.ViewModels.Items;
using RodelTranslate.Models.Client;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 包含应用 ID 的客户端配置部分.
/// </summary>
public sealed partial class TranslateAppClientConfigSection : TranslateServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateAppClientConfigSection"/> class.
    /// </summary>
    public TranslateAppClientConfigSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(TranslateServiceItemViewModel? oldValue, TranslateServiceItemViewModel? newValue)
    {
        if (newValue is not TranslateServiceItemViewModel newVM)
        {
            return;
        }

        newVM.Config ??= CreateCurrentConfig();
        Debug.Assert(ViewModel.Config != null, "ViewModel.Config should not be null.");
        ViewModel.CheckCurrentConfig();
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => AppIdBox.Text = (ViewModel.Config as AppClientConfigBase)?.AppId ?? string.Empty;

    private AppClientConfigBase CreateCurrentConfig()
    {
        var assembly = Assembly.GetAssembly(typeof(AppClientConfigBase));
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(AppClientConfigBase)));

        foreach (var type in types)
        {
            if (type.Name.StartsWith(ViewModel.ProviderType.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var config = (AppClientConfigBase)Activator.CreateInstance(type);
                return config;
            }
        }

        return null;
    }

    private void OnAppIdBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        ((AppClientConfigBase)ViewModel.Config).AppId = AppIdBox.Text;
        ViewModel.CheckCurrentConfig();
    }
}
