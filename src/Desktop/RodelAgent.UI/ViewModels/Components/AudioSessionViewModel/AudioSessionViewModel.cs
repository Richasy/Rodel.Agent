// Copyright (c) Rodel. All rights reserved.

using System.Threading;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.Pages;
using RodelAudio.Interfaces.Client;
using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;
using Windows.Storage;
using Windows.System;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 绘图会话视图模型.
/// </summary>
public sealed partial class AudioSessionViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioSessionViewModel"/> class.
    /// </summary>
    public AudioSessionViewModel(
        IAudioClient drawClient,
        ILogger<AudioSessionViewModel> logger,
        IStorageService storageService,
        AudioWaveModuleViewModel waveViewModel)
    {
        _audioClient = drawClient;
        _logger = logger;
        _storageService = storageService;
        _waveViewModel = waveViewModel;
        IsEnterSend = SettingsToolkit.ReadLocalSetting(SettingNames.AudioServicePageIsEnterSend, true);
    }

    private bool IsAudioValid(string audioPath)
    {
        if (string.IsNullOrEmpty(audioPath))
        {
            return false;
        }

        if (!File.Exists(audioPath))
        {
            var appVM = this.Get<AppViewModel>();
            appVM.ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.ImageNotFound), InfoType.Error));
            return false;
        }

        return true;
    }

    [RelayCommand]
    private async Task LoadSessionAsync(AudioSession session)
    {
        if (IsGenerating || (AudioPath?.Contains(session.Id) ?? false))
        {
            return;
        }

        if (AudioService.ProviderType != session.Provider)
        {
            Clear();
            await InitializeAsync(session.Provider);
        }

        Prompt = session.Text;
        var model = Models.FirstOrDefault(p => p.Id == session.Model);
        if (model != null)
        {
            ChangeModel(model);
        }

        AudioPath = AppToolkit.GetSpeechPath(session.Id);
        LastGenerateTime = session.Time?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
        _waveViewModel.LoadFileCommand.Execute(AudioPath);
        DataChanged?.Invoke(this, session);
    }

    [RelayCommand]
    private async Task InitializeAsync(ProviderType providerType)
    {
        var pageVM = this.Get<AudioServicePageViewModel>();
        var serviceVM = pageVM.AvailableServices.FirstOrDefault(p => p.ProviderType == providerType);
        AudioService = serviceVM;
        AudioPath = string.Empty;
        Models.Clear();
        if (providerType == ProviderType.AzureSpeech)
        {
            await _audioClient.InitialAzureSpeechAsync();
        }

        var models = _audioClient.GetModels(providerType);
        foreach (var item in models)
        {
            Models.Add(new AudioModelItemViewModel(item));
        }

        var localModelId = SettingsToolkit.ReadLocalSetting($"{providerType}LastSelectedAudioModel", string.Empty);
        var model = Models.FirstOrDefault(p => p.Id == localModelId) ?? Models.FirstOrDefault();
        ChangeModel(model);
    }

    [RelayCommand]
    private void Clear()
    {
        Prompt = string.Empty;
        AudioPath = string.Empty;
        LastGenerateTime = string.Empty;
        Voice = default;
        SelectedLanguage = default;
        Model = default;
        Models.Clear();
        Languages.Clear();
        Voices.Clear();
    }

    [RelayCommand]
    private void ChangeModel(AudioModelItemViewModel model)
    {
        if (model.IsSelected)
        {
            return;
        }

        foreach (var item in Models)
        {
            item.IsSelected = item.Id == model.Id;
        }

        Model = model.Data;
        var settingName = $"{AudioService.ProviderType}LastSelectedAudioModel";
        SettingsToolkit.WriteLocalSetting(settingName, model.Id);
        Languages.Clear();
        var voices = _audioClient.GetModels(AudioService.ProviderType).FirstOrDefault(p => p.Id == model.Id)?.Voices
            ?? throw new InvalidDataException("Model voices not found");
        var languages = voices.SelectMany(p => p.Languages).Distinct().Select(p => new AudioLanguageViewModel(p)).OrderBy(p => p.Name).ToList();
        languages.ForEach(Languages.Add);
        var localLanguage = SettingsToolkit.ReadLocalSetting($"{AudioService.ProviderType}_{model.Id}_AudioLanguage", string.Empty);
        var language = languages.FirstOrDefault(p => p.Code == localLanguage) ?? languages.FirstOrDefault();
        ChangeLanguage(language);
    }

    [RelayCommand]
    private void ChangeLanguage(AudioLanguageViewModel language)
    {
        if (language.IsSelected)
        {
            return;
        }

        var model = Models.FirstOrDefault(p => p.IsSelected);
        if (model == null)
        {
            return;
        }

        foreach (var item in Languages)
        {
            item.IsSelected = item.Code == language.Code;
        }

        SelectedLanguage = language;
        var settingName = $"{AudioService.ProviderType}_{model.Id}_AudioLanguage";
        SettingsToolkit.WriteLocalSetting(settingName, language.Code);
        Voices.Clear();
        var voices = _audioClient.GetModels(AudioService.ProviderType).FirstOrDefault(p => p.Id == model.Id)?.Voices
            ?? throw new InvalidDataException("Model voices not found");
        voices = voices.Where(p => p.Languages.Contains(language.Code)).OrderBy(p => p.DisplayName).ToList();
        voices.ToList().ForEach(p => Voices.Add(new AudioVoiceViewModel(p)));
        var localVoice = SettingsToolkit.ReadLocalSetting($"{AudioService.ProviderType}_{model.Id}_{language.Code}_AudioVoice", string.Empty);
        var selectedVoice = Voices.FirstOrDefault(p => p.Data.Id == localVoice) ?? Voices.FirstOrDefault();
        ChangeVoice(selectedVoice);
    }

    [RelayCommand]
    private void ChangeVoice(AudioVoiceViewModel voice)
    {
        if (voice.IsSelected)
        {
            return;
        }

        var model = Models.FirstOrDefault(p => p.IsSelected);
        if (model == null)
        {
            return;
        }

        var language = Languages.FirstOrDefault(p => p.IsSelected);
        if (language == null)
        {
            return;
        }

        foreach (var item in Voices)
        {
            item.IsSelected = item.Data.Id == voice.Data.Id;
        }

        var settingName = $"{AudioService.ProviderType}_{model.Id}_{language.Code}_AudioVoice";
        SettingsToolkit.WriteLocalSetting(settingName, voice.Data.Id);
        Voice = voice.Data;
    }

    [RelayCommand]
    private async Task GenerateAsync()
    {
        if (string.IsNullOrEmpty(Prompt))
        {
            return;
        }

        ErrorText = string.Empty;
        var sessionData = new AudioSession
        {
            Id = Guid.NewGuid().ToString("N"),
            Model = Models.FirstOrDefault(p => p.IsSelected)?.Id,
            Provider = AudioService.ProviderType,
            Text = Prompt,
            Voice = Voice.Id,
        };

        try
        {
            IsGenerating = true;
            CancelGenerate();
            AudioPath = string.Empty;
            LastGenerateTime = string.Empty;
            _cancellationTokenSource = new CancellationTokenSource();
            var dispatcherQueue = this.Get<Microsoft.UI.Dispatching.DispatcherQueue>();
            var result = await _audioClient.TextToSpeechAsync(sessionData, _cancellationTokenSource.Token).ConfigureAwait(false);
            dispatcherQueue.TryEnqueue(async () =>
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                await _storageService.AddOrUpdateAudioSessionAsync(sessionData, result.ToArray());
                var pageVM = this.Get<AudioServicePageViewModel>();
                pageVM.UpdateHistoryCommand.Execute(default);
                LastGenerateTime = sessionData.Time!.Value.ToString("yyyy-MM-dd HH:mm:ss");
                AudioPath = AppToolkit.GetSpeechPath(sessionData.Id);
                DataChanged?.Invoke(this, sessionData);
                _waveViewModel.LoadFileCommand.Execute(AudioPath);
            });
        }
        catch (Exception ex)
        {
            HandleGenerateException(ex);
        }
        finally
        {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private void CancelGenerate()
    {
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        _cancellationTokenSource = default;
    }

    [RelayCommand]
    private async Task OpenAudioAsync(string audioPath = default)
    {
        audioPath ??= AudioPath;
        if (!IsAudioValid(audioPath))
        {
            return;
        }

        var file = await StorageFile.GetFileFromPathAsync(audioPath);
        await Launcher.LaunchFileAsync(file);
    }

    [RelayCommand]
    private async Task SaveAudioAsync(string audioPath = default)
    {
        audioPath ??= AudioPath;
        if (!IsAudioValid(audioPath))
        {
            return;
        }

        var appVM = this.Get<AppViewModel>();
        var targetAudio = await FileToolkit.SaveFileAsync(".wav", appVM.ActivatedWindow);
        if (targetAudio == null)
        {
            return;
        }

        var file = await StorageFile.GetFileFromPathAsync(audioPath);
        await file.CopyAndReplaceAsync(targetAudio);
        appVM.ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Saved), InfoType.Success));
    }

    private void HandleGenerateException(Exception ex)
    {
        ErrorText = ex.Message;
        _logger.LogError(ex, "Generate speech failed.");
    }

    partial void OnIsEnterSendChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.AudioServicePageIsEnterSend, value);
}
