// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using Richasy.AgentKernel.Audio;
using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.Interfaces;
using RodelAgent.Models.Common;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Pages;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.Items;
using System.Globalization;
using Windows.Storage;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 语音页面视图模型.
/// </summary>
public sealed partial class AudioPageViewModel(ILogger<AudioPageViewModel> logger) : LayoutPageViewModelBase
{
    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(AudioPage);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Services == null)
        {
            this.Get<AppViewModel>().RequestReloadAudioServices += (_, _) => ReloadAvailableServicesCommand.Execute(default);
            IsEnterSend = SettingsToolkit.ReadLocalSetting(SettingNames.AudioServicePageIsEnterSend, true);
            await ReloadAvailableServicesAsync();
        }
    }

    [RelayCommand]
    private async Task ReloadAvailableServicesAsync()
    {
        var providers = Enum.GetValues<AudioProviderType>();
        var services = new List<AudioServiceItemViewModel>();
        var drawConfigManager = this.Get<IAudioConfigManager>();
        foreach (var p in providers)
        {
            if (p == AudioProviderType.Edge || p == AudioProviderType.Windows)
            {
                services.Add(new AudioServiceItemViewModel(p));
            }
            else
            {
                var config = await drawConfigManager.GetAudioConfigAsync(p);
                if (config?.IsValid() == true)
                {
                    services.Add(new AudioServiceItemViewModel(p));
                }
            }
        }

        Services = services;
        var lastSelectedService = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedAudioService, AudioProviderType.Edge);
        var service = Services.Find(p => p.ProviderType == lastSelectedService) ?? Services[0];
        SelectServiceCommand.Execute(service);
    }

    [RelayCommand]
    private async Task SelectServiceAsync(AudioServiceItemViewModel service)
    {
        Models.Clear();
        SelectedModel = null;
        foreach (var item in Services)
        {
            item.IsSelected = item.ProviderType == service.ProviderType;
        }

        SelectedService = service;
        SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedAudioService, service.ProviderType);

        _audioService = this.Get<IAudioService>(service.ProviderType.ToString());
        var sourceConfig = await this.Get<IAudioConfigManager>().GetAudioConfigAsync(service.ProviderType);
        var models = _audioService.GetPredefinedModels();
        models.ToList().ForEach(p => Models.Add(new AudioModelItemViewModel(p)));
        var lastSelectedModel = this.Get<ISettingsToolkit>().ReadLocalSetting($"{service.ProviderType}LastSelectedAudioModel", string.Empty);
        var model = Models.FirstOrDefault(p => p.Id == lastSelectedModel) ?? Models.FirstOrDefault();
        SelectModelCommand.Execute(model);
        ReloadHistoryCommand.Execute(default);
    }

    [RelayCommand]
    private void SelectModel(AudioModelItemViewModel model)
    {
        if (SelectedModel == model)
        {
            return;
        }

        Languages.Clear();
        SelectedLanguage = null;
        foreach (var item in Models)
        {
            item.IsSelected = item.Id == model.Id;
        }

        SelectedModel = model;
        this.Get<ISettingsToolkit>().WriteLocalSetting($"{SelectedService!.ProviderType}LastSelectedAudioModel", model.Id);
        var languages = model.Data.Voices.SelectMany(p => p.Languages).Distinct().ToList().ConvertAll(p => new LanguageItemViewModel(p, new(p)));
        languages.ForEach(Languages.Add);
        var currentUILang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        var lastSelectedLanguage = this.Get<ISettingsToolkit>().ReadLocalSetting($"{SelectedService.ProviderType}_{model.Id}_LastSelectedLanguage", string.Empty);
        var lan = string.IsNullOrEmpty(lastSelectedLanguage)
            ? languages.FirstOrDefault(p => p.Code?.StartsWith(currentUILang, StringComparison.InvariantCultureIgnoreCase) ?? false) ?? languages[0]
            : languages.FirstOrDefault(p => p.Code == lastSelectedLanguage) ?? languages.FirstOrDefault();

        SelectLanguageCommand.Execute(lan);
    }

    [RelayCommand]
    private async Task SelectLanguageAsync(LanguageItemViewModel language)
    {
        if (SelectedLanguage == language)
        {
            return;
        }

        Voices.Clear();
        SelectedVoice = null;
        SelectedLanguage = language;
        this.Get<ISettingsToolkit>().WriteLocalSetting($"{SelectedService!.ProviderType}_{SelectedModel!.Id}_LastSelectedLanguage", language.Code);
        var voices = SelectedModel?.Data.Voices.Where(p => p.Languages.Contains(language.Code)).ToList().ConvertAll(p => new AudioVoiceItemViewModel(p)) ?? [];
        voices.ForEach(Voices.Add);
        var lastSelectedVoice = this.Get<ISettingsToolkit>().ReadLocalSetting($"{SelectedService.ProviderType}_{SelectedModel!.Id}_LastSelectedVoice", string.Empty);
        SelectedVoice = voices.FirstOrDefault(p => p.Data.Id == lastSelectedVoice) ?? voices.FirstOrDefault();
        var config = await this.Get<IAudioConfigManager>().GetServiceConfigAsync(SelectedService.ProviderType, SelectedModel.Data);
        _audioService.Initialize(config);
    }

    [RelayCommand]
    private static async Task OpenAudioFolderAsync()
    {
        if (!Directory.Exists(AppToolkit.GetAudioFolderPath()))
        {
            Directory.CreateDirectory(AppToolkit.GetAudioFolderPath());
        }

        var folder = await StorageFolder.GetFolderFromPathAsync(AppToolkit.GetAudioFolderPath());
        await Windows.System.Launcher.LaunchFolderAsync(folder);
    }

    [RelayCommand]
    private async Task ReloadHistoryAsync()
    {
        if (SelectedService is null)
        {
            return;
        }

        var history = await this.Get<IStorageService>().GetAudioSessionsAsync();
        SyncAudioHistory(history ?? []);
    }

    [RelayCommand]
    private async Task SaveAudioAsync()
    {
        if (string.IsNullOrEmpty(AudioPath))
        {
            return;
        }

        var file = await this.Get<IFileToolkit>().SaveFileAsync(".wav", this.Get<AppViewModel>().ActivatedWindow);
        if (file is null)
        {
            return;
        }

        var sourceFile = await StorageFile.GetFileFromPathAsync(AudioPath);
        await sourceFile.CopyAndReplaceAsync(file);
    }

    [RelayCommand]
    private void OpenAudio()
    {
        if (AudioPath is null)
        {
            return;
        }

        var currentId = GetCurrentPresentAudioId();
        var recordVM = History.FirstOrDefault(p => p.Data.Id == currentId);
        recordVM?.OpenCommand.Execute(default);
    }

    private void SyncAudioHistory(List<AudioRecord> list)
    {
        // 边界情况处理
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(History);

        list = [.. list.OrderByDescending(item => item.Time)];
        var listDict = list.ToDictionary(item => item.Id);

        for (var i = History.Count - 1; i >= 0; i--)
        {
            var item = History[i];
            if (!listDict.ContainsKey(item.Data.Id))
            {
                History.RemoveAt(i);
            }
        }

        for (var i = 0; i < list.Count; i++)
        {
            var listItem = list[i];
            if (i < History.Count)
            {
                var collectionItem = History[i];
                if (!Equals(listItem.Id, collectionItem.Data.Id))
                {
                    History.Insert(i, new(listItem));
                }
            }
            else
            {
                History.Add(new(listItem));
            }
        }

        CheckHistoryCount();
    }

    private void CheckHistoryCount()
    {
        HistoryCount = History.Count;
        IsHistoryEmpty = History.Count == 0;
    }

    private string GetCurrentPresentAudioId()
        => AudioPath is null ? string.Empty : Path.GetFileNameWithoutExtension(AudioPath);

    partial void OnSelectedVoiceChanged(AudioVoiceItemViewModel? value)
    {
        if (SelectedModel is null || value is null)
        {
            return;
        }

        this.Get<ISettingsToolkit>().WriteLocalSetting($"{SelectedService!.ProviderType}_{SelectedModel.Id}_LastSelectedVoice", value.Data.Id);
    }

    partial void OnIsEnterSendChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.AudioServicePageIsEnterSend, value);
}
