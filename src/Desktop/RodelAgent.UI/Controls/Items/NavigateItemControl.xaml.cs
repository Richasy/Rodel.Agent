// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels;
using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Items;

/// <summary>
/// 导航项控件.
/// </summary>
public sealed partial class NavigateItemControl : NavigateItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigateItemControl"/> class.
    /// </summary>
    public NavigateItemControl()
    {
        InitializeComponent();
    }

    private void OnNavItemClick(object sender, RoutedEventArgs e)
        => this.Get<AppViewModel>().ChangeFeatureCommand.Execute(ViewModel.FeatureType);
}

/// <summary>
/// 导航项控件基类.
/// </summary>
public abstract class NavigateItemControlBase : ReactiveUserControl<NavigateItemViewModel>
{
}
