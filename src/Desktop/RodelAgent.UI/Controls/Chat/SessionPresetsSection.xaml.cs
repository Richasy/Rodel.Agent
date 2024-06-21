// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 本地模型区.
/// </summary>
public sealed partial class SessionPresetsSection : ChatServicePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SessionPresetsSection"/> class.
    /// </summary>
    public SessionPresetsSection() => InitializeComponent();

    private void OnPresetItemClick(object sender, ViewModels.Items.ChatPresetItemViewModel e)
        => ViewModel.SetSelectedSessionPresetCommand.Execute(e);

    private void OnPresetItemDeleteClick(object sender, RoutedEventArgs e)
    {
        var vm = (sender as FrameworkElement)?.DataContext as ViewModels.Items.ChatPresetItemViewModel;
        if (vm != null)
        {
            ViewModel.DeleteSessionPresetCommand.Execute(vm);
        }
    }

    private void OnEditItemClick(object sender, RoutedEventArgs e)
    {
        var vm = (sender as FrameworkElement)?.DataContext as ViewModels.Items.ChatPresetItemViewModel;
        if (vm != null)
        {
            ViewModel.EditSessionPresetCommand.Execute(vm);
        }
    }
}
