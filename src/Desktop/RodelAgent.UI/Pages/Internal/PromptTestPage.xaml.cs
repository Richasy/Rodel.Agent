// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Pages.Internal;

/// <summary>
/// 提示词测试.
/// </summary>
public sealed partial class PromptTestPage : PromptTestPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptTestPage"/> class.
    /// </summary>
    public PromptTestPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        ViewModel.InitializeCommand.Execute(default);
    }
}

/// <summary>
/// 提示词测试页面基类.
/// </summary>
public abstract class PromptTestPageBase : LayoutPageBase<PromptTestPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptTestPageBase"/> class.
    /// </summary>
    protected PromptTestPageBase() => ViewModel = this.Get<PromptTestPageViewModel>();
}
