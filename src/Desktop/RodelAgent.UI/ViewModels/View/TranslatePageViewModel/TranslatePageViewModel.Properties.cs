// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel.Translation;
using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 翻译页面视图模型.
/// </summary>
public sealed partial class TranslatePageViewModel
{
    private CancellationTokenSource? _cancellationTokenSource;
    private ITranslateService? _translateService;

    [ObservableProperty]
    public partial List<TranslateServiceItemViewModel> Services { get; set; }

    [ObservableProperty]
    public partial TranslateServiceItemViewModel SelectedService { get; set; }

    [ObservableProperty]
    public partial TranslateLanguageItemViewModel? SelectedSourceLanguage { get; set; }

    [ObservableProperty]
    public partial TranslateLanguageItemViewModel? SelectedTargetLanguage { get; set; }

    [ObservableProperty]
    public partial string SourceText { get; set; }

    [ObservableProperty]
    public partial string ResultText { get; set; }

    [ObservableProperty]
    public partial int SourceTextLength { get; set; }

    [ObservableProperty]
    public partial int MaxTextLength { get; private set; }

    [ObservableProperty]
    public partial bool IsExceedLimit { get; private set; }

    [ObservableProperty]
    public partial bool IsTranslating { get; set; }

    /// <summary>
    /// 源语言列表.
    /// </summary>
    public ObservableCollection<TranslateLanguageItemViewModel> SourceLanguages { get; } = [];

    /// <summary>
    /// 目标语言列表.
    /// </summary>
    public ObservableCollection<TranslateLanguageItemViewModel> TargetLanguages { get; } = [];
}
