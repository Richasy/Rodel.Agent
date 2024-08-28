// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.ViewModels;
using RodelAgent.UI.ViewModels.Components;

namespace RodelAgent.UI.Controls.Chat;

/// <summary>
/// 预设设置面板.
/// </summary>
public sealed partial class ChatPresetSettingsDialog : AppContentDialog
{
    /// <summary>
    /// <see cref="ViewModel"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(ChatPresetModuleViewModel), typeof(ChatPresetSettingsDialog), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPresetSettingsDialog"/> class.
    /// </summary>
    public ChatPresetSettingsDialog()
    {
        InitializeComponent();
        ViewModel = this.Get<ChatPresetModuleViewModel>();
        Closed += (_, _) => ViewModel.CloseRequested -= OnCloseRequested;
        ViewModel.CloseRequested += OnCloseRequested;
    }

    /// <summary>
    /// 视图模型.
    /// </summary>
    public ChatPresetModuleViewModel ViewModel
    {
        get => (ChatPresetModuleViewModel)GetValue(ViewModelProperty);
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
        if (!ModelPanel.IsValid())
        {
            this.Get<AppViewModel>()
                .ShowTip(StringNames.MustFillRequireFields, InfoType.Warning);
            btn.IsEnabled = true;
            return;
        }

        try
        {
            await ModelPanel.SaveAvatarAsync();

            if (ViewModel.IsAgentPreset)
            {
                await ViewModel.SaveAgentPresetCommand.ExecuteAsync(default);
            }
            else if (ViewModel.IsSessionPreset)
            {
                await ViewModel.SaveSessionPresetCommand.ExecuteAsync(default);
            }
        }
        catch (Exception)
        {
        }

        btn.IsEnabled = true;
    }
}
