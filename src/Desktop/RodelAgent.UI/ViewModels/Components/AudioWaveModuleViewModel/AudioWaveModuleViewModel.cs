// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Dispatching;
using NAudio.Wave;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 音频波形模块视图模型.
/// </summary>
public sealed partial class AudioWaveModuleViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioWaveModuleViewModel"/> class.
    /// </summary>
    public AudioWaveModuleViewModel(
        DispatcherQueue dispatcherQueue,
        ILogger<AudioWaveModuleViewModel> logger)
    {
        _dispatcherQueue = dispatcherQueue;
        _logger = logger;
    }

    /// <summary>
    /// 获取波形图的点.
    /// </summary>
    /// <returns>波形图点集.</returns>
    public List<Point> GetPoints(double controlWidth, double controlHeight, double itemWidth = 2, double spacing = 1)
    {
        if (_samples == null || _samples.Count == 0)
        {
            return new List<Point>();
        }

        var numOfItems = (int)(controlWidth / (itemWidth + spacing));
        var numOfSamplesPerItem = IsRecording ? 4800 : _samples.Count / numOfItems;

        var points = new List<Point>();
        for (var i = 0; i < numOfItems; i++)
        {
            float max = 0;
            for (var j = 0; j < numOfSamplesPerItem; j++)
            {
                var index = (i * numOfSamplesPerItem) + j;
                var sampleIndex = IsRecording ? _samples.Count - (numOfItems * numOfSamplesPerItem) + index : index;
                if (sampleIndex < 0 || _samples.Count <= sampleIndex)
                {
                    continue;
                }

                var sample = Math.Abs(_samples[sampleIndex]); // 使用绝对值，因为波形图只包含正值
                max = Math.Max(max, sample);
            }

            var height = (int)(max * controlHeight); // 高度与播放音量一致
            points.Add(new Point(i * (itemWidth + spacing), (controlHeight / 2) - height)); // 波形图以 (0,controlHeight/2) 为原点
        }

        return points;
    }

    [RelayCommand]
    private async Task LoadFileAsync(string file)
    {
        try
        {
            IsParsing = true;
            await ParseFileAsync(file);
            _mediaPlayer ??= CreateMediaPlayer();
            if (_mediaPlayer.PlaybackSession != null)
            {
                _mediaPlayer.Position = TimeSpan.Zero;
                _mediaPlayer.PlaybackSession.PositionChanged -= OnPlayPositionChanged;
            }

            _mediaPlayer.Source = MediaSource.CreateFromStorageFile(await StorageFile.GetFileFromPathAsync(file));
            _mediaPlayer.Play();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load file.");
        }
        finally
        {
            IsParsing = false;
        }
    }

    [RelayCommand]
    private void CheckRecording()
        => IsRecordingSupported = WaveIn.DeviceCount > 0;

    [RelayCommand]
    private async Task StartRecordingAsync(string sessionId = default)
    {
        CheckRecording();
        if (!IsRecordingSupported)
        {
            return;
        }

        if (_waveIn != null)
        {
            await StopRecordingAsync();
        }

        ResetRecording(sessionId);
        await Task.Run(() =>
        {
            _waveIn.StartRecording();
        });
    }

    [RelayCommand]
    private async Task StopRecordingAsync()
    {
        Position = 0;
        await Task.Run(() =>
        {
            _waveIn?.StopRecording();
        });
    }

    [RelayCommand]
    private void TogglePlayPause()
    {
        if (_mediaPlayer != null)
        {
            if (_mediaPlayer.CurrentState == MediaPlayerState.Playing)
            {
                _mediaPlayer.Pause();
            }
            else
            {
                if (_isMediaEnded)
                {
                    Position = 0;
                    _mediaPlayer.Position = TimeSpan.Zero;
                }

                _mediaPlayer.Play();
            }
        }
    }

    [RelayCommand]
    private void ResetPosition()
    {
        if (_mediaPlayer != null && _mediaPlayer.PlaybackSession != null)
        {
            _mediaPlayer.Pause();
            _mediaPlayer.PlaybackSession.Position = TimeSpan.Zero;
            Position = 0;
        }
    }

    [RelayCommand]
    private void ChangePosition(double position)
    {
        if (_mediaPlayer != null && _mediaPlayer.PlaybackSession != null)
        {
            _isMediaEnded = position >= Seconds - 0.25;
            Position = position;
            _mediaPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(position);
        }
    }

    private MediaPlayer CreateMediaPlayer()
    {
        var player = new MediaPlayer();
        player.AutoPlay = true;
        player.CurrentStateChanged += OnPlayerStateChanged;
        player.MediaOpened += OnMediaOpened;
        player.MediaEnded += OnMediaEnded;
        return player;
    }

    private void OnMediaEnded(MediaPlayer sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            Position = Seconds;
        });

        _isMediaEnded = true;
    }

    private void OnMediaOpened(MediaPlayer sender, object args)
    {
        var session = sender.PlaybackSession;
        _isMediaEnded = false;
        if (session != null)
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                Position = 0;
                Seconds = session.NaturalDuration.TotalSeconds;
            });

            session.Position = TimeSpan.Zero;
            session.PositionChanged += OnPlayPositionChanged;
        }
    }

    private void OnPlayPositionChanged(MediaPlaybackSession sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (sender.Position.TotalSeconds - Position >= 0.1)
            {
                Position = sender.Position.TotalSeconds;
            }
        });
    }

    private void OnPlayerStateChanged(MediaPlayer sender, object args)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            IsPlaying = sender.PlaybackSession != null && sender.PlaybackSession.PlaybackState == MediaPlaybackState.Playing;
        });
    }
}
