// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using System.ComponentModel;

namespace RodelAgent.UI.Controls.Chat;

public sealed partial class McpServerItemControl : McpServerItemControlBase
{
    public McpServerItemControl() => InitializeComponent();

    protected override void OnControlLoaded()
    {
        UpdateFunctionCount();
        UpdateState();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnControlUnloaded()
    {
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        FunctionRepeater.ItemsSource = null;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.FunctionCount))
        {
            UpdateFunctionCount();
        }
        else if (e.PropertyName == nameof(ViewModel.State))
        {
            UpdateState();
        }
    }

    private void UpdateFunctionCount()
    {
        var count = ViewModel.FunctionCount;
        if (count == 0)
        {
            SeeAllButton.IsEnabled = false;
            SeeAllButton.Content = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.NoFunction);
        }
        else
        {
            SeeAllButton.IsEnabled = true;
            SeeAllButton.Content = string.Format(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.FunctionCountTemplate), count);
        }
    }

    private void UpdateState()
    {
        var state = $"{ViewModel.State}State";
        VisualStateManager.GoToState(this, state, false);
    }

    private void OnContextOpened(object sender, object e)
    {
        RunItem.IsEnabled = ViewModel.State == Models.Constants.McpServerState.Stopped || ViewModel.State == Models.Constants.McpServerState.Error;
    }

    private void OnEnabledChanged(object sender, RoutedEventArgs e)
    {
        if (EnableSwitch.IsOn != ViewModel.IsEnabled)
        {
            ViewModel.IsEnabled = EnableSwitch.IsOn;
            ViewModel.Data.IsEnabled = ViewModel.IsEnabled;
            ViewModel.SaveCommand.Execute(default);
        }
    }
}

public abstract class McpServerItemControlBase : LayoutUserControlBase<McpServerItemViewModel>;
