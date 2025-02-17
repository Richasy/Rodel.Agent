// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Dispatching;
using Windows.Media.Playback;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 音频波形模块视图模型.
/// </summary>
public sealed partial class AudioWaveModuleViewModel
{
    private const int SampleRate = 44100;
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly ILogger<AudioWaveModuleViewModel> _logger;
    private List<float> _samples;
    private MediaPlayer _mediaPlayer;
    private bool _isMediaEnded;

    [ObservableProperty]
    private bool _isRecording;

    [ObservableProperty]
    private double _seconds;

    [ObservableProperty]
    private double _position;

    [ObservableProperty]
    private bool _isRecordingSupported;

    [ObservableProperty]
    private bool _isParsing;

    [ObservableProperty]
    private bool _isPlaying;

    /// <summary>
    /// 重新绘制波形.
    /// </summary>
    public event EventHandler RedrawWave;
}
