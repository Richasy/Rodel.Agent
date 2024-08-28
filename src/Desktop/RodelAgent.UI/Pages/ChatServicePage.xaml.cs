// Copyright (c) Rodel. All rights reserved.

using Microsoft.Windows.AppLifecycle;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.ViewModels.Pages;

namespace RodelAgent.UI.Pages;

/// <summary>
/// 对话服务页面.
/// </summary>
public sealed partial class ChatServicePage : ChatServicePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServicePage"/> class.
    /// </summary>
    public ChatServicePage()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
    }

    /// <inheritdoc/>
    protected override async void OnPageLoaded()
    {
        InitialRing.IsActive = true;
        if (ViewModel.IsAvailableServicesEmpty)
        {
            await ViewModel.ResetAvailableChatServicesCommand.ExecuteAsync(default);
        }

        if (ViewModel.IsAgentsEmpty)
        {
            await ViewModel.ResetAgentsCommand.ExecuteAsync(default);
        }

        if (ViewModel.IsSessionPresetsEmpty)
        {
            await ViewModel.ResetSessionPresetsCommand.ExecuteAsync(default);
        }

        if (ViewModel.IsGroupsEmpty)
        {
            await ViewModel.ResetGroupsCommand.ExecuteAsync(default);
        }

        InitializeSessionPanelType();
        InitializeGroupPanelType();
        UpdateExtraSizer();
        InitialRing.IsActive = false;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        => UpdateExtraSizer();

    private void UpdateExtraSizer()
    {
        var height = ActualHeight - 200;
        if (height < 0 || ExtraSizer == null)
        {
            return;
        }

        ExtraSizer.Maximum = height;
        ExtraSizer2.Maximum = height;
    }

    private void InitializeSessionPanelType()
    {
        var names = Enum.GetNames(typeof(ChatSessionPanelType));
        var stringToolkit = this.Get<IStringResourceToolkit>();
        for (var i = 0; i < names.Length; i++)
        {
            var name = names[i];
            var item = new SelectorBarItem
            {
                Tag = (ChatSessionPanelType)i,
                Text = stringToolkit.GetString(name),
            };

            SessionPanelTypeSelector.Items.Add(item);
            if (ViewModel.SessionPanelType == (ChatSessionPanelType)i)
            {
                SessionPanelTypeSelector.SelectedItem = item;
            }
        }
    }

    private void InitializeGroupPanelType()
    {
        var names = Enum.GetNames(typeof(ChatGroupPanelType));
        var stringToolkit = this.Get<IStringResourceToolkit>();
        for (var i = 0; i < names.Length; i++)
        {
            var name = names[i];
            var item = new SelectorBarItem
            {
                Tag = (ChatGroupPanelType)i,
                Text = stringToolkit.GetString(name),
            };

            GroupPanelTypeSelector.Items.Add(item);
            if (ViewModel.GroupPanelType == (ChatGroupPanelType)i)
            {
                GroupPanelTypeSelector.SelectedItem = item;
            }
        }
    }

    private void OnSessionPanelTypeChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        var currentType = (ChatSessionPanelType)SessionPanelTypeSelector.SelectedItem.Tag;
        ViewModel.SessionPanelType = currentType;
    }

    private void OnSessionParameterChanged(object sender, EventArgs e)
        => ViewModel.CurrentSession.SaveSessionToDatabaseCommand.Execute(default);

    private void OnRestartButtonClick(object sender, RoutedEventArgs e)
    {
        AppInstance.GetCurrent().UnregisterKey();
        _ = AppInstance.Restart(default);
    }

    private void OnGroupPanelTypeChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        var currentType = (ChatGroupPanelType)GroupPanelTypeSelector.SelectedItem.Tag;
        ViewModel.GroupPanelType = currentType;
    }

    private void OnExtraVisibilityButtonClick(object sender, EventArgs e)
        => ViewModel.IsExtraColumnManualHide = !ViewModel.IsExtraColumnManualHide;

    private void OnServiceVisibilityButtonClick(object sender, EventArgs e)
        => ViewModel.IsServiceColumnManualHide = !ViewModel.IsServiceColumnManualHide;
}

/// <summary>
/// 对话服务页面基类.
/// </summary>
public abstract class ChatServicePageBase : LayoutPageBase<ChatServicePageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServicePageBase"/> class.
    /// </summary>
    protected ChatServicePageBase() => ViewModel = this.Get<ChatServicePageViewModel>();
}
