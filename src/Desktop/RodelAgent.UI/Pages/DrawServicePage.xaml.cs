﻿// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Pages;

/// <summary>
/// 图像生成页面.
/// </summary>
public sealed partial class DrawServicePage : DrawServicePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawServicePage"/> class.
    /// </summary>
    public DrawServicePage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        if (ViewModel.IsAvailableServicesEmpty)
        {
            ViewModel.ResetAvailableDrawServicesCommand.Execute(default);
        }
    }
}

/// <summary>
/// 绘图服务页面.
/// </summary>
public abstract class DrawServicePageBase : LayoutPageBase<DrawServicePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawServicePageBase"/> class.
    /// </summary>
    protected DrawServicePageBase() => ViewModel = this.Get<DrawServicePageViewModel>();
}
