// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Components;
using RodelAgent.UI.ViewModels.Pages;
using RodelAudio.Models.Client;

namespace RodelAgent.UI.Controls.Audio;

/// <summary>
/// 会话项控件.
/// </summary>
public sealed partial class AudioSessionItemControl : AudioSessionItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioSessionItemControl"/> class.
    /// </summary>
    public AudioSessionItemControl()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
        => Initialize();

    private AudioSessionViewModel GetSessionViewModel()
        => this.Get<AudioServicePageViewModel>().Session;

    private void OnLoaded(object sender, RoutedEventArgs e)
        => Initialize();

    private void Initialize()
    {
        if (ViewModel == null || DateBlock == null)
        {
            return;
        }

        var time = !ViewModel.Time.HasValue
            ? "-/-"
            : ViewModel.Time.Value.ToString("yyyy-MM-dd HH:mm:ss");
        DateBlock.Text = time;
    }

    private void OnSessionClick(object sender, RoutedEventArgs e)
        => GetSessionViewModel().LoadSessionCommand.Execute(ViewModel);

    private void OnOpenItemClick(object sender, RoutedEventArgs e)
        => GetSessionViewModel().OpenAudioCommand.Execute(AppToolkit.GetSpeechPath(ViewModel.Id));

    private void OnDeleteItemClick(object sender, RoutedEventArgs e)
    {
        var pageVM = this.Get<AudioServicePageViewModel>();
        pageVM.DeleteHistoryItemCommand.Execute(ViewModel);
    }
}

/// <summary>
/// 会话项控件基类.
/// </summary>
public abstract class AudioSessionItemControlBase : ReactiveUserControl<AudioSession>
{
}
