// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// Group session side header.
/// </summary>
public sealed partial class GroupSessionSideHeader : ChatSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupSessionSideHeader"/> class.
    /// </summary>
    public GroupSessionSideHeader() => InitializeComponent();

    private void OnAgentsButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.IsAgentsVisible = true;

    private void OnOptionsButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.IsGroupOptionsVisible = true;
}
