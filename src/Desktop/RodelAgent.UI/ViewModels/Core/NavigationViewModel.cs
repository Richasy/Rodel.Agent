// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Pages;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.Forms;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// 导航视图模型.
/// </summary>
public sealed partial class NavigationViewModel : ViewModelBase, INavServiceViewModel
{
    private Frame? _navFrame;
    private Frame? _overFrame;

    [ObservableProperty]
    public partial bool IsOverlayOpen { get; set; }

    /// <summary>
    /// 导航条目列表.
    /// </summary>
    [ObservableProperty]
    public partial List<AppNavigationItemViewModel> MenuItems { get; set; }

    /// <summary>
    /// 底部条目列表.
    /// </summary>
    public ObservableCollection<AppNavigationItemViewModel> FooterItems { get; } = new();

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
            _overFrame!.Navigate(typeof(Page));
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
        for (var i = _overFrame!.BackStack.Count - 1; i >= 0; i--)
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
            _navFrame!.Focus(FocusState.Programmatic);
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

        foreach (var item in GetFooterItems())
        {
            FooterItems.Add(item);
        }
    }

    /// <summary>
    /// 快速卸载设置页，以触发保存逻辑.
    /// </summary>
    public void QuickUnloadIfInSettings()
    {
        if (_navFrame?.Content is SettingsPage)
        {
            _navFrame.Navigate(typeof(Page));
        }
    }

    private List<AppNavigationItemViewModel> GetMenuItems()
    {
        var lastSelectedPage = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedFeaturePage, typeof(ChatPage).FullName);
        var list = new List<AppNavigationItemViewModel>
        {
            GetItem<ChatPage>(StringNames.Chat, FluentIcons.Common.Symbol.Chat, accessKey: "1"),
            GetItem<DrawPage>(StringNames.Draw, FluentIcons.Common.Symbol.PenSparkle, accessKey: "2"),
            GetItem<AudioPage>(StringNames.Audio, FluentIcons.Common.Symbol.MicSparkle, accessKey: "3"),
            GetItem<TranslatePage>(StringNames.Translate, FluentIcons.Common.Symbol.Translate, accessKey: "4"),
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

    private List<AppNavigationItemViewModel> GetFooterItems()
    {
        return
        [
            GetItem<SettingsPage>(StringNames.Settings, FluentIcons.Common.Symbol.Settings, accessKey: "0"),
        ];
    }

    private AppNavigationItemViewModel GetItem<TPage>(StringNames title, FluentIcons.Common.Symbol symbol, bool isSelected = false, string accessKey = "")
        where TPage : Page
        => new AppNavigationItemViewModel(this, typeof(TPage), ResourceToolkit.GetLocalizedString(title), symbol, isSelected, accessKey);

    private void ActiveMainWindow()
        => this.Get<AppViewModel>().Windows.Find(p => p is MainWindow)?.Activate();
}
