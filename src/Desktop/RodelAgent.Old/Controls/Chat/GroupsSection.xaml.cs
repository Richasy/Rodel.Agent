// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 群组区.
/// </summary>
public sealed partial class GroupsSection : ChatServicePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupsSection"/> class.
    /// </summary>
    public GroupsSection() => InitializeComponent();

    private void OnPresetItemClick(object sender, ViewModels.Items.GroupPresetItemViewModel e)
        => ViewModel.SetSelectedGroupPresetCommand.Execute(e);

    private void OnPresetItemDeleteClick(object sender, RoutedEventArgs e)
    {
        var vm = (sender as FrameworkElement)?.DataContext as ViewModels.Items.GroupPresetItemViewModel;
        if (vm != null)
        {
            ViewModel.DeleteGroupCommand.Execute(vm);
        }
    }

    private void OnEditItemClick(object sender, RoutedEventArgs e)
    {
        var vm = (sender as FrameworkElement)?.DataContext as ViewModels.Items.GroupPresetItemViewModel;
        if (vm != null)
        {
            ViewModel.EditGroupCommand.Execute(vm);
        }
    }
}
