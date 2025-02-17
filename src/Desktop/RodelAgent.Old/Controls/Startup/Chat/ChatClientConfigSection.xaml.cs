// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using System.Diagnostics;
using System.Reflection;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 仅包含密钥的配置部分.
/// </summary>
public sealed partial class ChatClientConfigSection : ChatServiceConfigControlBase
{
    /// <summary>
    /// <see cref="CustomHeaderText"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty CustomHeaderTextProperty =
        DependencyProperty.Register(nameof(CustomHeaderText), typeof(string), typeof(ChatClientConfigSection), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatClientConfigSection"/> class.
    /// </summary>
    public ChatClientConfigSection() => InitializeComponent();

    /// <summary>
    /// 自定义密钥标题文本.
    /// </summary>
    public string CustomHeaderText
    {
        get => (string)GetValue(CustomHeaderTextProperty);
        set => SetValue(CustomHeaderTextProperty, value);
    }

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
    {
        if (string.IsNullOrEmpty(CustomHeaderText))
        {
            CustomHeaderText = ResourceToolkit.GetLocalizedString(StringNames.AccessKey);
        }

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
