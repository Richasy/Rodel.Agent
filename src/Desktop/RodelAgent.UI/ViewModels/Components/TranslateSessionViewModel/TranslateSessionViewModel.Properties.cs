// Copyright (c) Rodel. All rights reserved.

using System.Threading;
using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Items;
using RodelTranslate.Interfaces.Client;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 翻译会话视图模型.
/// </summary>
public sealed partial class TranslateSessionViewModel
{
    private readonly ITranslateClient _translateClient;
    private readonly ILogger<TranslateSessionViewModel> _logger;
    private readonly IStorageService _storageService;
    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty]
    private TranslateServiceItemViewModel _translateService;

    [ObservableProperty]
    private TranslateLanguageItemViewModel _sourceLanguage;

    [ObservableProperty]
    private TranslateLanguageItemViewModel _targetLanguage;

    [ObservableProperty]
    private string _sourceText;

    [ObservableProperty]
    private string _translatedText;

    [ObservableProperty]
    private int _maxTextLength;

    [ObservableProperty]
    private int _currentTextLength;

    [ObservableProperty]
    private bool _isExceedMaxTextLength;

    [ObservableProperty]
    private bool _isTranslating;

    /// <summary>
    /// 源语言列表.
    /// </summary>
    public ObservableCollection<TranslateLanguageItemViewModel> SourceLanguages { get; } = new();

    /// <summary>
    /// 目标语言列表.
    /// </summary>
    public ObservableCollection<TranslateLanguageItemViewModel> TargetLanguages { get; } = new();
}
