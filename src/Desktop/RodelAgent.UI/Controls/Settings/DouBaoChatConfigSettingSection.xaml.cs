// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Client;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// 字节豆包配置设置部分.
/// </summary>
public sealed partial class DouBaoChatConfigSettingSection : ChatServiceConfigControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DouBaoChatConfigSettingSection"/> class.
    /// </summary>
    public DouBaoChatConfigSettingSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ChatServiceItemViewModel? oldValue, ChatServiceItemViewModel? newValue)
    {
        if (newValue is not ChatServiceItemViewModel newVM)
        {
            return;
        }

        Logo.Provider = newVM.ProviderType.ToString();
        KeyCard.Description = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AccessKeyDescription), newVM.Name);
        KeyBox.PlaceholderText = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AccessKeyPlaceholder), newVM.Name);

        newVM.Config ??= new DouBaoClientConfig();
        ViewModel.CheckCurrentConfig();
    }

    private void OnKeyBoxLoaded(object sender, RoutedEventArgs e)
    {
        KeyBox.Password = ViewModel.Config?.Key ?? string.Empty;
        KeyBox.Focus(FocusState.Programmatic);
    }

    private void OnKeyBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Config.Key = KeyBox.Password;
        ViewModel.CheckCurrentConfig();
    }
}
