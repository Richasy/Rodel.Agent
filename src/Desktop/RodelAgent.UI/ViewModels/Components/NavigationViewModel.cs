// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Forms;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Pages;
using RodelAgent.UI.Pages.Internal;
using RodelAgent.UI.Toolkits;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// The navigation view model.
/// </summary>
public sealed partial class NavigationViewModel : ViewModelBase, INavServiceViewModel
{
    private Frame? _navFrame;
    private Frame? _overFrame;

    [ObservableProperty]
    private bool _isOverlayOpen;

    /// <summary>
    /// 导航条目列表.
    /// </summary>
    [ObservableProperty]
    private IReadOnlyCollection<AppNavigationItemViewModel> _menuItems;

    /// <summary>
    /// 底部条目列表.
    /// </summary>
    [ObservableProperty]
    private IReadOnlyCollection<AppNavigationItemViewModel> _footerItems;

    /// <inheritdoc/>
    public void NavigateTo(Type pageType, object? parameter = null)
    {
        if (_navFrame is null)
        {
            throw new InvalidOperationException("导航框架未初始化.");
        }

        ActiveMainWindow();
        var lastSelectedPage = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedFeaturePage, string.Empty);
        if (IsOverlayOpen)
        {
            IsOverlayOpen = false;
            _overFrame.Navigate(typeof(Page));
            _overFrame.BackStack.Clear();

            if (pageType.FullName == lastSelectedPage)
            {
                return;
            }
        }

        if (lastSelectedPage == pageType.FullName && _navFrame.Content is not null && _navFrame.Content.GetType().FullName == lastSelectedPage)
        {
            return;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedFeaturePage, pageType.FullName);
        _navFrame.Navigate(pageType, parameter, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
    }

    /// <inheritdoc/>
    public void NavigateToOver(Type pageType, object? parameter = null)
    {
        if (_overFrame is null)
        {
            throw new InvalidOperationException("导航框架未初始化.");
        }

        if (_overFrame.BackStack.Count > 0)
        {
            _overFrame.BackStack.Clear();
        }

        ActiveMainWindow();
        _overFrame.Navigate(pageType, parameter, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
        IsOverlayOpen = true;
    }

    /// <summary>
    /// 尝试返回.
    /// </summary>
    public void Back()
    {
        for (var i = _overFrame.BackStack.Count - 1; i >= 0; i--)
        {
            if (_overFrame.BackStack[i].SourcePageType.FullName == typeof(Page).FullName)
            {
                _overFrame.BackStack.RemoveAt(i);
            }
        }

        if (_overFrame.CanGoBack)
        {
            _overFrame.GoBack();
        }
        else
        {
            _overFrame.Navigate(typeof(Page));
            _overFrame.BackStack.Clear();
            _overFrame.Content = default;
            IsOverlayOpen = false;
            _navFrame.Focus(FocusState.Programmatic);
        }
    }

    /// <summary>
    /// 初始化导航视图模型.
    /// </summary>
    public void Initialize(Frame navFrame, Frame overFrame)
    {
        if (_navFrame is not null && _overFrame is not null)
        {
            return;
        }

        _navFrame = navFrame;
        _overFrame = overFrame;
        MenuItems = [.. GetMenuItems()];
        FooterItems = [.. GetFooterItems()];
    }

    private IReadOnlyList<AppNavigationItemViewModel> GetMenuItems()
    {
        var lastSelectedPage = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedFeaturePage, typeof(ChatServicePage).FullName);
        var list = new List<AppNavigationItemViewModel>
        {
            GetItem<ChatServicePage>(StringNames.ChatService_Slim, FluentIcons.Common.Symbol.Chat),
            GetItem<DrawServicePage>(StringNames.ImageGenerate_Slim, FluentIcons.Common.Symbol.Image),
            GetItem<AudioServicePage>(StringNames.VoiceGenerate_Slim, FluentIcons.Common.Symbol.Mic),
            GetItem<TranslateServicePage>(StringNames.TranslateService_Slim, FluentIcons.Common.Symbol.Translate),
        };

        foreach (var item in list)
        {
            item.IsSelected = item.PageKey == lastSelectedPage;
        }

        if (!list.Any(p => p.IsSelected))
        {
            list[0].IsSelected = true;
        }

        return list;
    }

    private IReadOnlyList<AppNavigationItemViewModel> GetFooterItems()
    {
        var list = new List<AppNavigationItemViewModel>
        {
            GetItem<SettingsPage>(StringNames.Settings, FluentIcons.Common.Symbol.Settings),
        };

        if (SettingsToolkit.ReadLocalSetting(SettingNames.IsInternalPromptTest, false))
        {
            list.Insert(0, GetItem<PromptTestPage>(StringNames.PromptTest, FluentIcons.Common.Symbol.TextBulletListSquareEdit));
        }

        return list;
    }

    private AppNavigationItemViewModel GetItem<TPage>(StringNames title, FluentIcons.Common.Symbol symbol, bool isSelected = false)
        where TPage : Page
        => new AppNavigationItemViewModel(this, typeof(TPage), ResourceToolkit.GetLocalizedString(title), symbol, isSelected);

    private void ActiveMainWindow()
        => this.Get<AppViewModel>().DisplayWindows.Find(p => p is MainWindow)?.Activate();
}
