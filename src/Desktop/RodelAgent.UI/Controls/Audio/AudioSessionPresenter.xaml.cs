// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls.Audio;

/// <summary>
/// 音频会话控件基类.
/// </summary>
public sealed partial class AudioSessionPresenter : AudioSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioSessionPresenter"/> class.
    /// </summary>
    public AudioSessionPresenter()
    {
        InitializeComponent();
    }

    private void OnOpenAudioRequested(object sender, EventArgs e)
        => ViewModel.OpenAudioCommand.Execute(default);

    private void OnSaveAudioRequested(object sender, EventArgs e)
        => ViewModel.SaveAudioCommand.Execute(default);
}
