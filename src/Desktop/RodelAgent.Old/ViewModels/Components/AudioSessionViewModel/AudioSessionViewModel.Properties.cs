// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Items;
using RodelAudio.Interfaces.Client;
using RodelAudio.Models.Client;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 绘图会话视图模型.
/// </summary>
public sealed partial class AudioSessionViewModel
{
    private readonly IAudioClient _audioClient;
    private readonly ILogger<AudioSessionViewModel> _logger;
    private readonly IStorageService _storageService;
    private readonly AudioWaveModuleViewModel _waveViewModel;
    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty]
    private string _prompt;

    [ObservableProperty]
    private AudioVoice _voice;

    [ObservableProperty]
    private AudioModel _model;

    [ObservableProperty]
    private AudioLanguageViewModel _selectedLanguage;

    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private string _audioPath;

    [ObservableProperty]
    private AudioServiceItemViewModel _audioService;

    [ObservableProperty]
    private bool _isEnterSend;

    [ObservableProperty]
    private string _lastGenerateTime;

    [ObservableProperty]
    private bool _isAudioHistoryShown;

    [ObservableProperty]
    private string _errorText;

    /// <summary>
    /// 当前加载的音频会话改变事件.
    /// </summary>
    public event EventHandler<AudioSession> DataChanged;

    /// <summary>
    /// 支持的音频语言.
    /// </summary>
    public ObservableCollection<AudioLanguageViewModel> Languages { get; } = new();

    /// <summary>
    /// 支持的音频声音.
    /// </summary>
    public ObservableCollection<AudioVoiceViewModel> Voices { get; } = new();

    /// <summary>
    /// 音频模型列表.
    /// </summary>
    public ObservableCollection<AudioModelItemViewModel> Models { get; } = new();
}
