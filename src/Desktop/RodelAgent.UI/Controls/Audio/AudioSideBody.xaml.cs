// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Audio;

/// <summary>
/// 音频侧边栏主体.
/// </summary>
public sealed partial class AudioSideBody : AudioPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioSideBody"/> class.
    /// </summary>
    public AudioSideBody() => InitializeComponent();

    protected override void OnControlUnloaded()
        => HistoryRepeater.ItemsSource = null;
}
