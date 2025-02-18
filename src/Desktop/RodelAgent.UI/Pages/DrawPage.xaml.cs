// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.View;

namespace RodelAgent.UI.Pages;

/// <summary>
/// Draw page.
/// </summary>
public sealed partial class DrawPage : DrawPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawPage"/> class.
    /// </summary>
    public DrawPage() => InitializeComponent();
}

/// <summary>
/// Draw page base.
/// </summary>
public abstract class DrawPageBase : LayoutPageBase<DrawPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawPageBase"/> class.
    /// </summary>
    protected DrawPageBase() => ViewModel = this.Get<DrawPageViewModel>();
}