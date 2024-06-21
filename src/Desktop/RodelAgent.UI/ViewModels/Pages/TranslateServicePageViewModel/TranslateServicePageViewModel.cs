// Copyright (c) Rodel. All rights reserved.

using System.Collections.Specialized;
using RodelAgent.Interfaces;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 翻译页面视图模型.
/// </summary>
public sealed partial class TranslateServicePageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateServicePageViewModel"/> class.
    /// </summary>
    public TranslateServicePageViewModel(
        IStorageService storageService,
        ILogger<TranslateServicePageViewModel> logger,
        TranslateSessionViewModel sessionVM)
    {
        _storageService = storageService;
        _logger = logger;
        Session = sessionVM;

        IsAvailableServicesEmpty = AvailableServices.Count == 0;
        IsHistoryEmpty = History.Count == 0;

        History.CollectionChanged += OnHistoryCollectionChanged;
    }

    [RelayCommand]
    private void InitializeBasis()
        => IsHistoryShown = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.ShouldRecordTranslate, false);

    private void OnHistoryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => IsHistoryEmpty = History.Count == 0;
}
