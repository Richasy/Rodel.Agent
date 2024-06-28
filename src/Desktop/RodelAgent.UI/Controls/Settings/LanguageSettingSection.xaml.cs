// Copyright (c) Rodel. All rights reserved.

using System.ComponentModel;
using Microsoft.Windows.AppLifecycle;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// 应用语言设置.
/// </summary>
public sealed partial class LanguageSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageSettingSection"/> class.
    /// </summary>
    public LanguageSettingSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => ViewModel.PropertyChanged -= OnViewModelPropertyChanged;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        CheckLanguage();
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.AppLanguage))
        {
            CheckLanguage();
        }
    }

    private void CheckLanguage()
    {
        if (string.IsNullOrEmpty(ViewModel.AppLanguage))
        {
            return;
        }

        var selectIndex = ViewModel.AppLanguage switch
        {
            "zh-Hans-CN" => 1,
            "en-US" => 2,
            _ => 0,
        };

        if (selectIndex == LanguageComboBox.SelectedIndex)
        {
            return;
        }

        LanguageComboBox.SelectedIndex = selectIndex;
    }

    private async void OnLanguageChangedAsync(object sender, SelectionChangedEventArgs e)
    {
        var item = e.AddedItems.First() as ComboBoxItem;
        var lanCode = item?.Tag as string;
        if (string.IsNullOrEmpty(lanCode)
            || (!string.IsNullOrEmpty(ViewModel.AppLanguage) && ViewModel.AppLanguage == lanCode))
        {
            return;
        }

        ViewModel.AppLanguage = lanCode;
        var dialog = new TipDialog(ResourceToolkit.GetLocalizedString(StringNames.ChangeLanguageTip));
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            AppInstance.GetCurrent().UnregisterKey();
            _ = AppInstance.Restart(default);
        }
    }
}
