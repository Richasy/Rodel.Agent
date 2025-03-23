// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using Richasy.AgentKernel.Translation;
using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.Items;
using System.Globalization;
using Windows.ApplicationModel.DataTransfer;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 翻译页面视图模型.
/// </summary>
public sealed partial class TranslatePageViewModel(ILogger<TranslatePageViewModel> logger) : ViewModelBase
{
    private static int GetMaxLength(TranslateProviderType providerType)
        => providerType switch
        {
            TranslateProviderType.Ali
            or TranslateProviderType.Tencent
            or TranslateProviderType.Volcano
            or TranslateProviderType.Youdao => 5000,
            TranslateProviderType.Azure => 50_000,
            TranslateProviderType.Baidu => 6000,
            TranslateProviderType.Google => 2000,
            _ => 5000,
        };

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Services == null)
        {
            this.Get<AppViewModel>().RequestReloadTranslateServices += (_, _) => ReloadAvailableServicesCommand.Execute(default);
            await ReloadAvailableServicesAsync();
        }
    }

    [RelayCommand]
    private async Task ReloadAvailableServicesAsync()
    {
        var providers = Enum.GetValues<TranslateProviderType>();
        var services = new List<TranslateServiceItemViewModel>();
        var translateConfigManager = this.Get<ITranslateConfigManager>();
        foreach (var p in providers)
        {
            if (p == TranslateProviderType.Google)
            {
                services.Add(new TranslateServiceItemViewModel(p));
            }
            else
            {
                var config = await translateConfigManager.GetTranslateConfigAsync(p);
                if (config?.IsValid() == true)
                {
                    services.Add(new TranslateServiceItemViewModel(p));
                }
            }
        }

        Services = services;
        var lastSelectedService = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.LastSelectedTranslateService, TranslateProviderType.Google);
        var service = Services.Find(p => p.ProviderType == lastSelectedService) ?? Services[0];
        SelectServiceCommand.Execute(service);
    }

    [RelayCommand]
    private async Task SelectServiceAsync(TranslateServiceItemViewModel service)
    {
        SourceLanguages.Clear();
        TargetLanguages.Clear();
        SelectedSourceLanguage = null;
        SelectedTargetLanguage = null;
        foreach (var item in Services)
        {
            item.IsSelected = item.ProviderType == service.ProviderType;
        }

        SelectedService = service;
        SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.LastSelectedTranslateService, service.ProviderType);

        var config = await this.Get<ITranslateConfigManager>().GetServiceConfigAsync(service.ProviderType);
        _translateService = this.Get<ITranslateService>(service.ProviderType.ToString());
        _translateService.Initialize(config);
        var lanPack = _translateService.GetSupportedLanguages();
        var sourceLanguages = lanPack.SourceLanguages.Select(p => new Items.LanguageItemViewModel(p.Key, p.Value)).OrderBy(p => p.Name).ToList();
        var targetLanguages = lanPack.TargetLanguages.Select(p => new Items.LanguageItemViewModel(p.Key, p.Value)).OrderBy(p => p.Name).ToList();
        // Move auto to first in sourceLanguages.
        var auto = sourceLanguages.Find(p => p.Code == "auto");
        if (auto != null)
        {
            sourceLanguages.Remove(auto);
            sourceLanguages.Insert(0, auto);
        }

        sourceLanguages.ForEach(p => SourceLanguages.Add(p));
        targetLanguages.ForEach(p => TargetLanguages.Add(p));
        var lastSelectedSourceLanguage = this.Get<ISettingsToolkit>().ReadLocalSetting($"{service.ProviderType}LastSelectedSourceLanguage", string.Empty);
        var lastSelectedTargetLanguage = this.Get<ISettingsToolkit>().ReadLocalSetting($"{service.ProviderType}LastSelectedTargetLanguage", string.Empty);
        var sourceLanguage = string.IsNullOrEmpty(lastSelectedSourceLanguage) ? SourceLanguages[0] : SourceLanguages.FirstOrDefault(p => p.Code == lastSelectedSourceLanguage) ?? SourceLanguages[0];
        var currentUILang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        var targetLanguage = string.IsNullOrEmpty(lastSelectedTargetLanguage)
            ? TargetLanguages.FirstOrDefault(p => p.Code?.StartsWith(currentUILang, StringComparison.InvariantCultureIgnoreCase) ?? false) ?? TargetLanguages[0]
            : TargetLanguages.FirstOrDefault(p => p.Code == lastSelectedTargetLanguage) ?? TargetLanguages[0];
        SelectedSourceLanguage = sourceLanguage;
        SelectedTargetLanguage = targetLanguage;
        SourceTextLength = SourceText?.Length ?? 0;
        MaxTextLength = GetMaxLength(service.ProviderType);
        CheckTextLimit();
    }

    [RelayCommand]
    private void CopyResultText()
    {
        if (!string.IsNullOrEmpty(ResultText))
        {
            var dp = new DataPackage();
            dp.SetText(ResultText);
            Clipboard.SetContent(dp);
            this.Get<Core.AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
        }
    }

    private void CheckTextLimit()
        => IsExceedLimit = SourceTextLength > MaxTextLength;

    partial void OnMaxTextLengthChanged(int value)
        => CheckTextLimit();

    partial void OnSourceTextChanged(string value)
        => CheckTextLimit();

    partial void OnSelectedSourceLanguageChanged(LanguageItemViewModel? value)
    {
        if (value != null && SelectedService != null)
        {
            this.Get<ISettingsToolkit>().WriteLocalSetting($"{SelectedService.ProviderType}LastSelectedSourceLanguage", value.Code);
        }
    }

    partial void OnSelectedTargetLanguageChanged(LanguageItemViewModel? value)
    {
        if (value != null && SelectedService != null)
        {
            this.Get<ISettingsToolkit>().WriteLocalSetting($"{SelectedService.ProviderType}LastSelectedTargetLanguage", value.Code);
        }
    }
}
