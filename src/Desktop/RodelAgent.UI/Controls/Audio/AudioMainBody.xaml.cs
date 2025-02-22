// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Audio;

/// <summary>
/// 音频主体.
/// </summary>
public sealed partial class AudioMainBody : AudioPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioMainBody"/> class.
    /// </summary>
    public AudioMainBody() => InitializeComponent();

    private void OnOpenAudioRequested(object sender, EventArgs e)
        => ViewModel.OpenAudioCommand.Execute(default);

    private void OnSaveAudioRequested(object sender, EventArgs e)
        => ViewModel.SaveAudioCommand.Execute(default);
}
