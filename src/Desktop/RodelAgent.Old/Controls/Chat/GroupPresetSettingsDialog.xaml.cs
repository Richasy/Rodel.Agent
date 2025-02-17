// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels;
using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 预设设置面板.
/// </summary>
public sealed partial class GroupPresetSettingsDialog : AppContentDialog
{
    /// <summary>
    /// <see cref="ViewModel"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(GroupPresetModuleViewModel), typeof(ChatPresetSettingsDialog), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupPresetSettingsDialog"/> class.
    /// </summary>
    public GroupPresetSettingsDialog()
    {
        InitializeComponent();
        ViewModel = this.Get<GroupPresetModuleViewModel>();
        Closed += (_, _) => ViewModel.CloseRequested -= OnCloseRequested;
        ViewModel.CloseRequested += OnCloseRequested;
    }

    /// <summary>
    /// 视图模型.
    /// </summary>
    public GroupPresetModuleViewModel ViewModel
    {
        get => (GroupPresetModuleViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private void OnCloseRequested(object sender, EventArgs e)
        => Hide();

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        ViewModel.IsManualClose = true;
        Hide();
    }

    private async void OnSaveClickAsync(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        btn.IsEnabled = false;
        if (!GroupPanel.IsValid())
        {
            this.Get<AppViewModel>()
                .ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.MustFillRequireFields), InfoType.Warning));
            btn.IsEnabled = true;
            return;
        }

        try
        {
            await GroupPanel.SaveAvatarAsync();
            ViewModel.SaveGroupPresetCommand.Execute(default);
        }
        catch (Exception)
        {
        }

        btn.IsEnabled = true;
    }
}
