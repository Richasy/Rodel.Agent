// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels;

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// 主题设置部分.
/// </summary>
public sealed partial class ThemeSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeSettingSection"/> class.
    /// </summary>
    public ThemeSettingSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        var index = ViewModel.AppTheme switch
        {
            ElementTheme.Light => 0,
            ElementTheme.Dark => 1,
            _ => 2,
        };

        ThemePicker.SelectedIndex = index;
    }

    private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var theme = ThemePicker.SelectedIndex switch
        {
            0 => ElementTheme.Light,
            1 => ElementTheme.Dark,
            _ => ElementTheme.Default,
        };

        ViewModel.AppTheme = theme;
        this.Get<AppViewModel>().ChangeTheme(theme);
    }
}
