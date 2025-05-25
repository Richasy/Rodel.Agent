// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Settings;

/// <summary>
/// 关于设置控件.
/// </summary>
public sealed partial class AboutSettingControl : SettingsPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutSettingControl"/> class.
    /// </summary>
    public AboutSettingControl() => InitializeComponent();

    protected override void OnControlUnloaded()
    {
        LinkRepeater.ItemsSource = null;
        LibraryRepeater.ItemsSource = null;
    }
}
