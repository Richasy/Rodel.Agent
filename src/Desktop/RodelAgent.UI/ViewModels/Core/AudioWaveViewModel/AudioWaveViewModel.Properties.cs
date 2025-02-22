// Copyright (c) Richasy. All rights reserved.

using Windows.Media.Playback;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// 音频波形模块视图模型.
/// </summary>
public sealed partial class AudioWaveViewModel
{
    private List<float> _samples;
    private MediaPlayer _mediaPlayer;
    private bool _isMediaEnded;

    [ObservableProperty]
    public partial bool IsRecording { get; set; }

    [ObservableProperty]
    public partial double Seconds { get; set; }

    [ObservableProperty]
    public partial double Position { get; set; }

    [ObservableProperty]
    public partial bool IsRecordingSupported { get; set; }

    [ObservableProperty]
    public partial bool IsParsing { get; set; }

    [ObservableProperty]
    public partial bool IsPlaying { get; set; }

    /// <summary>
    /// 重新绘制波形.
    /// </summary>
    public event EventHandler RedrawWave;
}
