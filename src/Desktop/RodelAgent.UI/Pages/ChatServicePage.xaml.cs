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
        ViewModel = ServiceProvider.GetRequiredService<ChatServicePageViewModel>();
        SizeChanged += OnSizeChanged;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        if (ViewModel.IsAvailableServicesEmpty)
        {
            ViewModel.ResetAvailableChatServicesCommand.Execute(default);
        }

        if (ViewModel.IsAgentsEmpty)
        {
            ViewModel.ResetAgentsCommand.Execute(default);
        }

        if (ViewModel.IsSessionPresetsEmpty)
        {
            ViewModel.ResetSessionPresetsCommand.Execute(default);
        }

        InitializePanelType();
        UpdateExtraSizer();
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
    }

    private void InitializePanelType()
    {
        var names = Enum.GetNames(typeof(ChatSessionPanelType));
        var stringToolkit = ServiceProvider.GetRequiredService<IStringResourceToolkit>();
        for (var i = 0; i < names.Length; i++)
        {
            var name = names[i];
            var item = new SelectorBarItem
            {
                Tag = (ChatSessionPanelType)i,
                Text = stringToolkit.GetString(name),
            };

            PanelTypeSelector.Items.Add(item);
            if (ViewModel.PanelType == (ChatSessionPanelType)i)
            {
                PanelTypeSelector.SelectedItem = item;
            }
        }
    }

    private void OnPanelTypeChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        var currentType = (ChatSessionPanelType)PanelTypeSelector.SelectedItem.Tag;
        ViewModel.PanelType = currentType;
    }

    private void OnSessionParameterChanged(object sender, EventArgs e)
        => ViewModel.CurrentSession.SaveSessionToDatabaseCommand.Execute(default);

    private void OnRestartButtonClick(object sender, RoutedEventArgs e)
    {
        AppInstance.GetCurrent().UnregisterKey();
        _ = AppInstance.Restart(default);
    }
}

/// <summary>
/// 对话服务页面基类.
/// </summary>
public abstract class ChatServicePageBase : PageBase<ChatServicePageViewModel>
{
}
