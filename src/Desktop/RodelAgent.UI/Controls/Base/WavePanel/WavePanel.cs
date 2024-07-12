// Copyright (c) Rodel. All rights reserved.

using System.ComponentModel;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml.Shapes;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Components;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace RodelAgent.UI.Controls;

/// <summary>
/// 波形面板.
/// </summary>
public sealed class WavePanel : ReactiveControl<AudioWaveModuleViewModel>
{
    private CanvasControl _waveCanvas;
    private Slider _waveSlider;
    private Rectangle _hoverHolder;
    private Button _playPauseButton;

    /// <summary>
    /// Initializes a new instance of the <see cref="WavePanel"/> class.
    /// </summary>
    public WavePanel()
    {
        DefaultStyleKey = typeof(WavePanel);
        ViewModel = ServiceProvider.GetRequiredService<AudioWaveModuleViewModel>();
        SizeChanged += OnSizeChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// 打开音频.
    /// </summary>
    public event EventHandler OpenAudio;

    /// <summary>
    /// 保存音频.
    /// </summary>
    public event EventHandler SaveAudio;

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _waveCanvas = GetTemplateChild("WaveCanvas") as CanvasControl;
        _waveSlider = GetTemplateChild("WaveSlider") as Slider;
        _hoverHolder = GetTemplateChild("HoverHolder") as Rectangle;
        _playPauseButton = GetTemplateChild("PlayPauseButton") as Button;

        var openAudioButton = GetTemplateChild("OpenAudioButton") as Button;
        var saveAudioButton = GetTemplateChild("SaveAudioButton") as Button;

        if (_waveCanvas != null)
        {
            _waveCanvas.Draw += OnWaveCanvasDraw;
        }

        if (_waveSlider != null)
        {
            _waveSlider.PointerEntered += OnWaveSliderPointerEntered;
            _waveSlider.PointerExited += OnWaveSliderPointerExited;
            _waveSlider.PointerMoved += OnWaveSliderPointerMoved;
            _waveSlider.ValueChanged += OnWaveSliderValueChanged;
        }

        if (_playPauseButton != null)
        {
            _playPauseButton.Click += OnPlayPauseButtonClick;
        }

        if (openAudioButton != null)
        {
            openAudioButton.Click += (sender, e) => OpenAudio?.Invoke(this, default);
        }

        if (saveAudioButton != null)
        {
            saveAudioButton.Click += (sender, e) => SaveAudio?.Invoke(this, default);
        }
    }

    private static bool IsDark()
    {
        var theme = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.AppTheme, ElementTheme.Default);
        return theme == ElementTheme.Default ? Application.Current.RequestedTheme == ApplicationTheme.Dark : theme == ElementTheme.Dark;
    }

    private static bool IsHighContrast()
        => new AccessibilitySettings().HighContrast;

    private static Color GetRectColor(bool isHighlight)
    {
        return IsHighContrast()
            ? ColorHelper.FromArgb(255, 200, 200, 200)
            : isHighlight
                ? IsDark() ? ColorHelper.FromArgb(168, 255, 255, 255) : ColorHelper.FromArgb(168, 0, 0, 0)
                : IsDark() ? ColorHelper.FromArgb(52, 255, 255, 255) : ColorHelper.FromArgb(40, 0, 0, 0);
    }

    private void RepositionHoverHolder(PointerRoutedEventArgs e)
    {
        if (ViewModel.Seconds == 0)
        {
            return;
        }

        _hoverHolder.Visibility = Visibility.Visible;
        var x = e.GetCurrentPoint(this).Position.X;
        _hoverHolder.Margin = new Thickness(x - (_hoverHolder.ActualWidth / 2), 16, 0, 12);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.RedrawWave -= OnRedrawWave;
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        if (ViewModel.IsRecording)
        {
            ViewModel.StopRecordingCommand.Execute(default);
        }
        else if (ViewModel.IsPlaying)
        {
            ViewModel.ResetPositionCommand.Execute(default);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        ViewModel.RedrawWave += OnRedrawWave;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Position))
        {
            _waveSlider.Value = ViewModel.Position;
        }
    }

    private void OnPlayPauseButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.TogglePlayPauseCommand.Execute(default);

    private void OnWaveSliderPointerMoved(object sender, PointerRoutedEventArgs e)
        => RepositionHoverHolder(e);

    private void OnWaveSliderPointerExited(object sender, PointerRoutedEventArgs e)
        => _hoverHolder.Visibility = Visibility.Collapsed;

    private void OnWaveSliderPointerEntered(object sender, PointerRoutedEventArgs e)
        => RepositionHoverHolder(e);

    private void OnWaveSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        _hoverHolder.Visibility = Visibility.Collapsed;
        _waveCanvas.Invalidate();
        ViewModel.ChangePositionCommand.Execute(e.NewValue);
    }

    private void OnWaveCanvasDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (ViewModel.Seconds == 0)
        {
            return;
        }

        var itemWidth = 2;
        var points = ViewModel.GetPoints(ActualWidth, ActualHeight, itemWidth, 1);
        if (points == default || points.Count == 0)
        {
            return;
        }

        var drawSession = args.DrawingSession;

        var highlightIndex = ViewModel.IsRecording
            ? points.Count
            : (int)(_waveSlider.Value / ViewModel.Seconds * points.Count);

        for (var i = 0; i < points.Count; i++)
        {
            var point = points[i];
            var color = GetRectColor(i <= highlightIndex);
            var rect = new Rect(point.X - (itemWidth / 2), point.Y, itemWidth, Math.Max(((ActualHeight / 2) - point.Y) * 2, 2));
            drawSession.FillRoundedRectangle(rect, 1, 1, color);
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        => _waveCanvas.Invalidate();

    private void OnRedrawWave(object sender, EventArgs e)
        => _waveCanvas.Invalidate();
}
