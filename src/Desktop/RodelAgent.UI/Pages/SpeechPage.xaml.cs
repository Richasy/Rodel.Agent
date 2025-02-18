// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Pages;

/// <summary>
/// Speech page.
/// </summary>
public sealed partial class SpeechPage : SpeechPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechPage"/> class.
    /// </summary>
    public SpeechPage() => InitializeComponent();
}

/// <summary>
/// Speech page base.
/// </summary>
public abstract class SpeechPageBase : LayoutPageBase<SpeechPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechPageBase"/> class.
    /// </summary>
    protected SpeechPageBase() => ViewModel = this.Get<SpeechPageViewModel>();
}