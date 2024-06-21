// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Controls.Items;

namespace RodelAgent.UI.Controls.Startup;

/// <summary>
/// 启动页面聊天部分.
/// </summary>
public sealed partial class StartupTranslateSection : StartupPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupChatSection"/> class.
    /// </summary>
    public StartupTranslateSection() => InitializeComponent();

    private void OnItemClick(object sender, ViewModels.Items.TranslateServiceItemViewModel e)
        => ViewModel.SideNavigate(typeof(Pages.Startup.TranslateConfigurationPage), e);

    private void OnServiceItemLoaded(object sender, RoutedEventArgs e)
    {
        var ele = (TranslateServiceItemControl)sender;
        var vm = ele.ViewModel;
        if (vm.ProviderType == RodelTranslate.Models.Constants.ProviderType.Baidu)
        {
            ele.ChangeLogoHeight(28);
        }
    }
}
