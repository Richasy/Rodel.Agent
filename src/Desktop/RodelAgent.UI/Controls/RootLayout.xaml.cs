// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using Windows.System;

namespace RodelAgent.UI.Controls;

/// <summary>
/// 根布局.
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
    protected override async void OnControlLoaded()
    {
        if (MigrationToolkit.ShouldMigrate())
        {
            try
            {
                MigrateWidget.Visibility = Visibility.Visible;
                await MigrationToolkit.TryMigrateAsync();
                SettingsToolkit.DeleteLocalSetting(Models.Constants.SettingNames.MigrationFailed);
            }
            catch (Exception ex)
            {
                SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.MigrationFailed, true);
                this.Get<ILogger<RootLayout>>().LogError(ex, "Failed to migrate database.");
            }
            finally
            {
                MigrateWidget.Visibility = Visibility.Collapsed;
                AppViewModel.RestartCommand.Execute(default);
            }

            return;
        }

        ViewModel.Initialize(MainFrame, OverlayFrame);
        var selectedItem = ViewModel.MenuItems.FirstOrDefault(p => p.IsSelected);
        if (selectedItem is not null)
        {
            NavView.SelectedItem = selectedItem;
            selectedItem.NavigateCommand.Execute(default);
        }

#if !DEBUG
        AppViewModel.CheckUpdateCommand.Execute(default);
#endif
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

    private void OnBackRequested(object? sender, EventArgs? e)
        => ViewModel.Back();

    private void OnUpdateActionButtonClick(TeachingTip sender, object args)
        => AppViewModel.ShowUpdateCommand.Execute(default);

    private void OnUpdateCloseButtonClick(TeachingTip sender, object args)
        => AppViewModel.HideUpdateCommand.Execute(default);

    private async void OnReadDocumentClick(object sender, RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri(Toolkits.AppToolkit.GetDocumentLink(string.Empty))).AsTask();
    }
}

/// <summary>
/// 根布局基类.
/// </summary>
public abstract class RootLayoutBase : LayoutUserControlBase<NavigationViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RootLayoutBase"/> class.
    /// </summary>
    protected RootLayoutBase() => ViewModel = this.Get<NavigationViewModel>();

    /// <summary>
    /// 应用视图模型.
    /// </summary>
    protected AppViewModel AppViewModel { get; } = GlobalDependencies.Kernel.GetRequiredService<AppViewModel>();
}