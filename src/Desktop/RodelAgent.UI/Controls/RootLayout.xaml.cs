// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls;

/// <summary>
/// The root layout of the application.
/// </summary>
public sealed partial class RootLayout : RootLayoutBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RootLayout"/> class.
    /// </summary>
    public RootLayout() => InitializeComponent();

    /// <summary>
    /// 获取主标题栏.
    /// </summary>
    /// <returns><see cref="AppTitleBar"/>.</returns>
    public AppTitleBar GetMainTitleBar() => MainTitleBar;

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.Initialize(MainFrame, OverlayFrame);
        var selectedItem = ViewModel.MenuItems.FirstOrDefault(p => p.IsSelected);
        if (selectedItem is not null)
        {
            NavView.SelectedItem = selectedItem;
            selectedItem.NavigateCommand.Execute(default);
        }
    }

    private void OnNavViewBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        => OnBackRequested(default, default);

    private void OnNavViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        _ = this;
        var item = args.InvokedItemContainer as AppNavigationViewItem;
        var context = item?.Tag as AppNavigationItemViewModel;
        context?.NavigateCommand.Execute(default);
    }

    private void OnBackRequested(object sender, EventArgs e)
        => ViewModel.Back();
}

/// <summary>
/// The root layout of the application.
/// </summary>
public abstract class RootLayoutBase : LayoutUserControlBase<NavigationViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RootLayoutBase"/> class.
    /// </summary>
    protected RootLayoutBase() => ViewModel = this.Get<NavigationViewModel>();
}
