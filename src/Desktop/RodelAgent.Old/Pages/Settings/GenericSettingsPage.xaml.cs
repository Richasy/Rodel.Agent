// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Pages.Settings;

/// <summary>
/// 通用设置.
/// </summary>
public sealed partial class GenericSettingsPage : SettingsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericSettingsPage"/> class.
    /// </summary>
    public GenericSettingsPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeGenericSettingsCommand.Execute(default);

    private void OnJoinGroupButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}
