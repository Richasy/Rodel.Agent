// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Settings;

public sealed partial class LanguageSettingControl : SettingsPageControlBase
{
    public LanguageSettingControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        LanguagePicker.SelectedIndex = ViewModel.AppLanguage switch
        {
            "zh-Hans-CN" => 1,
            "en-US" => 2,
            _ => 0,
        };
    }

    private void OnLanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var language = LanguagePicker.SelectedIndex switch
        {
            1 => "zh-Hans-CN",
            2 => "en-US",
            _ => string.Empty,
        };

        ViewModel.AppLanguage = language;
    }
}
