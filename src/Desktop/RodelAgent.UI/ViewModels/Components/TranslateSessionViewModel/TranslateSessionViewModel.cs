// Copyright (c) Rodel. All rights reserved.

using System.Globalization;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.Pages;
using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;
using RodelTranslate.Models.Constants;
using Windows.ApplicationModel.DataTransfer;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 翻译会话视图模型.
/// </summary>
public sealed partial class TranslateSessionViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateSessionViewModel"/> class.
    /// </summary>
    public TranslateSessionViewModel(
        ITranslateClient translateClient,
        ILogger<TranslateSessionViewModel> logger,
        IStorageService storageService)
    {
        _translateClient = translateClient;
        _logger = logger;
        _storageService = storageService;
    }

    [RelayCommand]
    private void Initialize(ProviderType type)
    {
        var pageVM = this.Get<TranslateServicePageViewModel>();
        var service = pageVM.AvailableServices.FirstOrDefault(p => p.ProviderType == type);
        TranslateService = service;
        SourceLanguage = default;
        TargetLanguage = default;
        SourceLanguages.Clear();
        TargetLanguages.Clear();
        var languages = _translateClient.GetLanguageList(type);
        var sourceLanguages = new List<TranslateLanguageItemViewModel>();
        var targetLanguages = new List<TranslateLanguageItemViewModel>();
        foreach (var item in languages)
        {
            var vmSource = new TranslateLanguageItemViewModel(item);
            var vmTarget = new TranslateLanguageItemViewModel(item);
            sourceLanguages.Add(vmSource);
            targetLanguages.Add(vmTarget);
        }

        sourceLanguages.Sort((x, y) => string.Compare(x.DisplayName, y.DisplayName, StringComparison.CurrentCulture));
        targetLanguages.Sort((x, y) => string.Compare(x.DisplayName, y.DisplayName, StringComparison.CurrentCulture));
        sourceLanguages.ForEach(SourceLanguages.Add);
        targetLanguages.ForEach(TargetLanguages.Add);
        SourceLanguages.Insert(0, new TranslateLanguageItemViewModel(default));

        var lastSelectedSourceLanguage = SettingsToolkit.ReadLocalSetting($"{type}LastSelectedSourceLanguage", string.Empty);
        var lastSelectedTargetLanguage = SettingsToolkit.ReadLocalSetting($"{type}LastSelectedTargetLanguage", string.Empty);
        SourceLanguage = string.IsNullOrEmpty(lastSelectedSourceLanguage)
            ? SourceLanguages.First()
            : SourceLanguages.FirstOrDefault(p => p.Data?.Id?.Equals(lastSelectedSourceLanguage) ?? false) ?? SourceLanguages.First();
        var currentUILang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        TargetLanguage = string.IsNullOrEmpty(lastSelectedTargetLanguage)
            ? TargetLanguages.FirstOrDefault(p => p.Data?.ISOCode?.StartsWith(currentUILang, StringComparison.InvariantCultureIgnoreCase) ?? false) ?? TargetLanguages.First()
            : TargetLanguages.FirstOrDefault(p => p.Data?.Id?.Equals(lastSelectedTargetLanguage) ?? false) ?? TargetLanguages.First();
        CurrentTextLength = SourceText?.Length ?? 0;
        MaxTextLength = Convert.ToInt32(_translateClient.GetMaxTextLength(type));
    }

    [RelayCommand]
    private async Task TranslateAsync()
    {
        if (string.IsNullOrEmpty(SourceText) || IsTranslating)
        {
            return;
        }

        var sourceLan = SourceLanguage?.Data;
        var targetLan = TargetLanguage?.Data;
        var appVM = this.Get<AppViewModel>();
        if (targetLan == null)
        {
            appVM.ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.TargetLanguageCanNotBeEmpty), InfoType.Error));
            return;
        }

        if (IsExceedMaxTextLength)
        {
            appVM.ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.ExceedTextLimit), InfoType.Error));
            return;
        }

        var sessionData = new TranslateSession
        {
            Provider = TranslateService.ProviderType,
            SourceLanguage = sourceLan,
            TargetLanguage = targetLan,
            Id = Guid.NewGuid().ToString("N"),
        };

        try
        {
            IsTranslating = true;
            CancelTranslate();

            _cancellationTokenSource = new System.Threading.CancellationTokenSource();
            var result = await _translateClient.TranslateTextAsync(sessionData, SourceText, _cancellationTokenSource.Token);
            TranslatedText = result.Text;
            if (!string.IsNullOrEmpty(result.Source))
            {
                var source = SourceLanguages.FirstOrDefault(p => p.Data?.Id?.Equals(result.Source) ?? false);
                if (source != null)
                {
                    sessionData.SourceLanguage = source.Data;
                }
            }

            var shouldRecord = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldRecordTranslate, false);
            if (shouldRecord)
            {
                await _storageService.AddOrUpdateTranslateSessionAsync(sessionData);
                var pageVM = this.Get<TranslateServicePageViewModel>();
                pageVM.ReloadHistoryCommand.Execute(default);
            }

            _cancellationTokenSource = default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Translate failed.");
            appVM.ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.TranslationFailed), InfoType.Error));
            return;
        }
        finally
        {
            IsTranslating = false;
        }
    }

    [RelayCommand]
    private void CancelTranslate()
    {
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        _cancellationTokenSource = default;
    }

    [RelayCommand]
    private void CopyTranslatedText()
    {
        if (string.IsNullOrEmpty(TranslatedText))
        {
            return;
        }

        var dataPackage = new DataPackage();
        dataPackage.SetText(TranslatedText);
        Clipboard.SetContent(dataPackage);
        var appVM = this.Get<AppViewModel>();
        appVM.ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
    }

    partial void OnSourceTextChanged(string value)
    {
        CurrentTextLength = value?.Length ?? 0;
        IsExceedMaxTextLength = CurrentTextLength > MaxTextLength;
    }

    partial void OnSourceLanguageChanged(TranslateLanguageItemViewModel value)
    {
        if (value != null)
        {
            SettingsToolkit.WriteLocalSetting($"{TranslateService.ProviderType}LastSelectedSourceLanguage", value.Data?.Id ?? string.Empty);
        }
    }

    partial void OnTargetLanguageChanged(TranslateLanguageItemViewModel value)
    {
        if (value != null)
        {
            SettingsToolkit.WriteLocalSetting($"{TranslateService.ProviderType}LastSelectedTargetLanguage", value.Data.Id);
            if (!string.IsNullOrEmpty(SourceText))
            {
                TranslateCommand.Execute(default);
            }
        }
    }
}
