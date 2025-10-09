// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Pages;

/// <summary>
/// Audio page.
/// </summary>
public sealed partial class AudioPage : AudioPageBase, IInitializePage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AudioPage"/> class.
    /// </summary>
    public AudioPage() => InitializeComponent();

    public void Initialize()
        => ViewModel.InitializeCommand.Execute(default);

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => Initialize();
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