// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Controls;
using RodelAgent.UI.Models.Args;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Pages;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppViewModel"/> class.
    /// </summary>
    public AppViewModel(ILogger<AppViewModel> logger)
    {
        _logger = logger;
        NavigateItems.Add(new NavigateItemViewModel(FeatureType.Chat));

        if (GlobalFeatureSwitcher.IsRagFeatureEnabled)
        {
            NavigateItems.Add(new NavigateItemViewModel(FeatureType.RAG));
        }

        if (GlobalFeatureSwitcher.IsT2IFeatureEnabled)
        {
            NavigateItems.Add(new NavigateItemViewModel(FeatureType.Draw));
        }

        if (GlobalFeatureSwitcher.IsT2SFeatureEnabled)
        {
            NavigateItems.Add(new NavigateItemViewModel(FeatureType.Audio));
        }

        NavigateItems.Add(new NavigateItemViewModel(FeatureType.Translate));
        SettingsItem = new NavigateItemViewModel(FeatureType.Settings);
    }

    /// <summary>
    /// 强制更新指定预设的头像.
    /// </summary>
    /// <param name="presetId">预设标识符.</param>
    public void ForceUpdatePresetAvatar(string presetId)
        => PresetAvatarUpdateRequested?.Invoke(this, presetId);

    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <param name="message">提示内容.</param>
    /// <param name="type">提示类型.</param>
    public void ShowTip(string message, InfoType type = InfoType.Information)
        => RequestShowTip?.Invoke(this, new AppTipNotification(message, type, ActivatedWindow));

    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <param name="messageName">提示内容.</param>
    /// <param name="type">提示类型.</param>
    public void ShowTip(StringNames messageName, InfoType type = InfoType.Information)
        => ShowTip(ResourceToolkit.GetLocalizedString(messageName), type);

    /// <summary>
    /// 显示消息通知.
    /// </summary>
    /// <param name="message">消息.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task ShowMessageDialogAsync(string message)
    {
        var tipDialog = new TipDialog(message);
        tipDialog.XamlRoot = ActivatedWindow.Content.XamlRoot;
        await tipDialog.ShowAsync();
    }

    /// <summary>
    /// 显示消息通知.
    /// </summary>
    /// <param name="messageName">消息.</param>
    /// <returns><see cref="Task"/>.</returns>
    public Task ShowMessageDialogAsync(StringNames messageName)
        => ShowMessageDialogAsync(ResourceToolkit.GetLocalizedString(messageName));

    /// <summary>
    /// 导航请求.
    /// </summary>
    public void Navigate(Type pageType, object parameter = default)
        => NavigationRequested?.Invoke(this, new AppNavigationEventArgs(pageType, parameter));

    /// <summary>
    /// 修改主题.
    /// </summary>
    /// <param name="theme">主题类型.</param>
    public void ChangeTheme(ElementTheme theme)
    {
        if (ActivatedWindow == null)
        {
            return;
        }

        (ActivatedWindow.Content as FrameworkElement).RequestedTheme = theme;
        if (theme == ElementTheme.Dark)
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
        }
        else if (theme == ElementTheme.Light)
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
        }
        else
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = default;
        }
    }

    [RelayCommand]
    private void Initialize()
    {
        var lastSelectedFeature = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedFeature, FeatureType.Chat);
        ChangeFeatureCommand.Execute(lastSelectedFeature);
    }

    [RelayCommand]
    private void ChangeFeature(FeatureType feature)
    {
        var currentFeature = NavigateItems.FirstOrDefault(x => x.IsSelected)?.FeatureType;
        if (currentFeature == feature)
        {
            return;
        }

        foreach (var item in NavigateItems)
        {
            item.IsSelected = item.FeatureType == feature;
        }

        SettingsItem.IsSelected = feature == FeatureType.Settings;

        if (feature != FeatureType.Settings)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedFeature, feature);
        }

        var pageType = feature switch
        {
            FeatureType.Chat => typeof(ChatServicePage),
            FeatureType.RAG => typeof(RAGPage),
            FeatureType.Draw => typeof(DrawServicePage),
            FeatureType.Audio => typeof(AudioServicePage),
            FeatureType.Translate => typeof(TranslateServicePage),
            FeatureType.Settings => typeof(SettingsPage),
            _ => throw new ArgumentOutOfRangeException(nameof(feature), feature, null)
        };

        Navigate(pageType);
    }
}
