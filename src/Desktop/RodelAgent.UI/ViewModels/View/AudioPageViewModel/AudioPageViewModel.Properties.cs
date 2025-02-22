// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel.Audio;
using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 语音页面视图模型.
/// </summary>
public sealed partial class AudioPageViewModel
{
    private IAudioService _audioService;
    private CancellationTokenSource? _audioCts;

    [ObservableProperty]
    public partial List<AudioServiceItemViewModel> Services { get; set; }

    [ObservableProperty]
    public partial AudioServiceItemViewModel? SelectedService { get; set; }

    [ObservableProperty]
    public partial LanguageItemViewModel? SelectedLanguage { get; set; }

    [ObservableProperty]
    public partial AudioModelItemViewModel? SelectedModel { get; set; }

    [ObservableProperty]
    public partial AudioVoiceItemViewModel? SelectedVoice { get; set; }

    [ObservableProperty]
    public partial bool IsEnterSend { get; set; }

    [ObservableProperty]
    public partial int HistoryCount { get; set; }

    [ObservableProperty]
    public partial bool IsHistoryEmpty { get; set; }

    [ObservableProperty]
    public partial bool IsGenerating { get; set; }

    [ObservableProperty]
    public partial string Text { get; set; }

    [ObservableProperty]
    public partial string? AudioPath { get; set; }

    /// <summary>
    /// 模型列表.
    /// </summary>
    public ObservableCollection<AudioModelItemViewModel> Models { get; set; } = [];

    /// <summary>
    /// 语言列表.
    /// </summary>
    public ObservableCollection<LanguageItemViewModel> Languages { get; set; } = [];

    /// <summary>
    /// 声音列表.
    /// </summary>
    public ObservableCollection<AudioVoiceItemViewModel> Voices { get; set; } = [];

    /// <summary>
    /// 历史记录.
    /// </summary>
    public ObservableCollection<AudioRecordItemViewModel> History { get; set; } = [];
}
