// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Xaml.Media.Animation;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Pages;

/// <summary>
/// 设置页面.
/// </summary>
public sealed partial class SettingsPage : SettingsPageBase
{
    private SettingSectionType? _previousSection;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    public SettingsPage()
    {
        InitializeComponent();
        InitializeSections();
    }

    /// <inheritdoc/>
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        => SaveSettings();

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => SaveSettings();

    private void SaveSettings()
    {
        ViewModel.SaveOnlineChatServicesCommand.Execute(default);
        ViewModel.SaveOnlineTranslateServicesCommand.Execute(default);
        ViewModel.SaveOnlineDrawServicesCommand.Execute(default);
        ViewModel.SaveOnlineAudioServicesCommand.Execute(default);
    }

    private void InitializeSections()
    {
        var names = Enum.GetNames<SettingSectionType>();
        var resToolkit = ServiceProvider.GetRequiredService<IStringResourceToolkit>();
        var values = names.Select(resToolkit.GetString);
        for (var i = 0; i < values.Count(); i++)
        {
            var v = values.ElementAt(i);
            SettingSectionSelector.Items.Add(new SelectorBarItem { Text = v, Tag = i });
        }

        SettingSectionSelector.SelectedItem = SettingSectionSelector.Items[0];
    }

    private void OnSettingSectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        var sectionType = (SettingSectionType)Convert.ToInt32(sender.SelectedItem.Tag);
        var animationType = _previousSection == null
            ? SlideNavigationTransitionEffect.FromBottom
            : sectionType - _previousSection > 0
                ? SlideNavigationTransitionEffect.FromRight
                : SlideNavigationTransitionEffect.FromLeft;
        _previousSection = sectionType;
        var pageType = GetType().Assembly.GetType($"{GetType().Namespace}.Settings.{sectionType}Page");
        if (pageType is not null)
        {
            SectionFrame.Navigate(pageType, default, new SlideNavigationTransitionInfo { Effect = animationType });
        }
    }
}

/// <summary>
/// 设置页面基类.
/// </summary>
public abstract class SettingsPageBase : PageBase<SettingsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageBase"/> class.
    /// </summary>
    protected SettingsPageBase() => ViewModel = ServiceProvider.GetRequiredService<SettingsPageViewModel>();
}
