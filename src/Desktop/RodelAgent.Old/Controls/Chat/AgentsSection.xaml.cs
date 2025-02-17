// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 助理区.
/// </summary>
public sealed partial class AgentsSection : ChatServicePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentsSection"/> class.
    /// </summary>
    public AgentsSection() => InitializeComponent();

    private void OnPresetItemClick(object sender, ViewModels.Items.ChatPresetItemViewModel e)
        => ViewModel.SetSelectedAgentCommand.Execute(e);

    private void OnPresetItemDeleteClick(object sender, RoutedEventArgs e)
    {
        var vm = (sender as FrameworkElement)?.DataContext as ViewModels.Items.ChatPresetItemViewModel;
        if (vm != null)
        {
            ViewModel.DeleteAgentCommand.Execute(vm);
        }
    }

    private void OnEditItemClick(object sender, RoutedEventArgs e)
    {
        var vm = (sender as FrameworkElement)?.DataContext as ViewModels.Items.ChatPresetItemViewModel;
        if (vm != null)
        {
            ViewModel.EditAgentCommand.Execute(vm);
        }
    }

    private void OnCreateCopyItemClick(object sender, RoutedEventArgs e)
    {
        var vm = (sender as FrameworkElement)?.DataContext as ViewModels.Items.ChatPresetItemViewModel;
        if (vm != null)
        {
            ViewModel.CreateAgentCopyCommand.Execute(vm);
        }
    }
}
