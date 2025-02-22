// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Pages;

/// <summary>
/// Audio page.
/// </summary>
public sealed partial class AudioPage : AudioPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioPage"/> class.
    /// </summary>
    public AudioPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// Audio page base.
/// </summary>
public abstract class AudioPageBase : LayoutPageBase<AudioPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioPageBase"/> class.
    /// </summary>
    protected AudioPageBase() => ViewModel = this.Get<AudioPageViewModel>();
}