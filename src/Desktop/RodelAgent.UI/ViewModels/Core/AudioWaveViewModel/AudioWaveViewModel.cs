// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Dispatching;
using System.Diagnostics;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// 音频波形视图模型.
/// </summary>
public sealed partial class AudioWaveViewModel(
    DispatcherQueue dispatcherQueue,
    ILogger<AudioWaveViewModel> logger) : ViewModelBase
{
    /// <summary>
    /// 获取波形图的点.
    /// </summary>
    /// <returns>波形图点集.</returns>
    public List<Point> GetPoints(double controlWidth, double controlHeight, double itemWidth = 2, double spacing = 1)
    {
        if (_samples == null || _samples.Count == 0)
        {
            return [];
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
            logger.LogError(ex, "Failed to load file.");
        }
        finally
        {
            IsParsing = false;
        }
    }

    [RelayCommand]
    private void CheckRecording()
        => IsRecordingSupported = false;

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
        if (_mediaPlayer?.PlaybackSession != null)
        {
            _mediaPlayer.Pause();
            _mediaPlayer.PlaybackSession.Position = TimeSpan.Zero;
            Position = 0;
        }
    }

    [RelayCommand]
    private void ChangePosition(double position)
    {
        if (_mediaPlayer?.PlaybackSession != null)
        {
            _isMediaEnded = position >= Seconds - 0.25;
            Position = position;
            _mediaPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(position);
        }
    }

    private MediaPlayer CreateMediaPlayer()
    {
        var player = new MediaPlayer
        {
            AutoPlay = true,
        };
        player.CurrentStateChanged += OnPlayerStateChanged;
        player.MediaOpened += OnMediaOpened;
        player.MediaEnded += OnMediaEnded;
        return player;
    }

    private void OnMediaEnded(MediaPlayer sender, object args)
    {
        dispatcherQueue.TryEnqueue(() => Position = Seconds);
        _isMediaEnded = true;
    }

    private void OnMediaOpened(MediaPlayer sender, object args)
    {
        var session = sender.PlaybackSession;
        _isMediaEnded = false;
        if (session != null)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                Position = 0;
                Seconds = session.NaturalDuration.TotalSeconds;
                Debug.WriteLine($"Media opened: {Seconds}");
            });

            session.Position = TimeSpan.Zero;
            session.PositionChanged += OnPlayPositionChanged;
        }
    }

    private void OnPlayPositionChanged(MediaPlaybackSession sender, object args)
    {
        dispatcherQueue.TryEnqueue(() =>
        {
            Seconds = sender.NaturalDuration.TotalSeconds;
            if (sender.Position.TotalSeconds - Position >= 0.1)
            {
                Position = sender.Position.TotalSeconds;
            }
        });
    }

    private void OnPlayerStateChanged(MediaPlayer sender, object args)
        => dispatcherQueue.TryEnqueue(() => IsPlaying = sender.PlaybackSession != null && sender.PlaybackSession.PlaybackState == MediaPlaybackState.Playing);
}
