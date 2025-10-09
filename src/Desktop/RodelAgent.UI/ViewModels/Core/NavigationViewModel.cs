// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Pages;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.Forms;
using RodelAgent.UI.Controls;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// 导航视图模型.
/// </summary>
public sealed partial class NavigationViewModel : ViewModelBase, INavServiceViewModel
{
    private MainFrame? _navFrame;

    /// <summary>
    /// 导航条目列表.
    /// </summary>
    [ObservableProperty]
    public partial List<AppNavigationItemViewModel> MenuItems { get; set; }

    /// <summary>
    /// 底部条目列表.
    /// </summary>
    public ObservableCollection<AppNavigationItemViewModel> FooterItems { get; } = [];

    /// <inheritdoc/>
    public void NavigateTo(Type pageType, object? parameter = null)
    {
        if (_navFrame is null)
        {
            throw new InvalidOperationException("导航框架未初始化.");
        }

        ActiveMainWindow();
        var lastSelectedPage = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedFeaturePage, string.Empty);
        if (lastSelectedPage == pageType.FullName && _navFrame.GetCurrentContent() is not null && _navFrame.GetCurrentContent()!.GetType().FullName == lastSelectedPage)
        {
            return;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedFeaturePage, pageType.FullName);
        _navFrame.NavigateTo(pageType, parameter);
    }

    /// <summary>
    /// 初始化导航视图模型.
    /// </summary>
    public void Initialize(MainFrame navFrame)
    {
        if (_navFrame is not null)
        {
            return;
        }

        _navFrame = navFrame;

        MenuItems = [.. GetMenuItems()];

        foreach (var item in GetFooterItems())
        {
            FooterItems.Add(item);
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
