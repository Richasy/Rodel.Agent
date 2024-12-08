// Copyright (c) Rodel. All rights reserved.

using Microsoft.Graphics.Canvas.UI.Xaml;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Components;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace RodelAgent.UI.Controls;

/// <summary>
/// 音频波形面板.
/// </summary>
public sealed partial class WavePanel : WavePanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WavePanel"/> class.
    /// </summary>
    public WavePanel()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
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
    protected override void OnControlLoaded()
    {
        WaveCanvas.Draw += OnWaveCanvasDraw;
        ViewModel.RedrawWave += OnRedrawWave;
        if (ViewModel.IsPlaying)
        {
            ViewModel.ResetPositionCommand.Execute(default);
        }
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        WaveCanvas.Draw -= OnWaveCanvasDraw;
        ViewModel.RedrawWave -= OnRedrawWave;
        if (ViewModel.IsPlaying)
        {
            ViewModel.TogglePlayPauseCommand.Execute(default);
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

        HoverHolder.Visibility = Visibility.Visible;
        var x = e.GetCurrentPoint(this).Position.X;
        HoverHolder.Margin = new Thickness(x - (HoverHolder.ActualWidth / 2), 16, 0, 12);
    }

    private void OnWaveSliderPointerMoved(object sender, PointerRoutedEventArgs e)
        => RepositionHoverHolder(e);

    private void OnWaveSliderPointerExited(object sender, PointerRoutedEventArgs e)
        => HoverHolder.Visibility = Visibility.Collapsed;

    private void OnWaveSliderPointerEntered(object sender, PointerRoutedEventArgs e)
        => RepositionHoverHolder(e);

    private void OnWaveSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        HoverHolder.Visibility = Visibility.Collapsed;
        WaveCanvas.Invalidate();
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
            : (int)(WaveSlider.Value / ViewModel.Seconds * points.Count);

        for (var i = 0; i < points.Count; i++)
        {
            var point = points[i];
            var color = GetRectColor(i <= highlightIndex);
            var rect = new Rect(point.X - (itemWidth / 2), point.Y, itemWidth, Math.Max(((ActualHeight / 2) - point.Y) * 2, 2));
            drawSession.FillRoundedRectangle(rect, 1, 1, color);
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        => WaveCanvas.Invalidate();

    private void OnRedrawWave(object sender, EventArgs e)
        => WaveCanvas.Invalidate();

    private void OnOpenAudioButtonClick(object sender, RoutedEventArgs e)
        => OpenAudio?.Invoke(this, EventArgs.Empty);

    private void OnSaveAudioButtonClick(object sender, RoutedEventArgs e)
        => SaveAudio?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// 音频波形面板基类.
/// </summary>
public abstract class WavePanelBase : LayoutUserControlBase<AudioWaveModuleViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WavePanelBase"/> class.
    /// </summary>
    protected WavePanelBase() => ViewModel = this.Get<AudioWaveModuleViewModel>();
}
