// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Controls;
using RodelAgent.UI.Forms;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;

namespace RodelAgent.UI.ViewModels;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppViewModel"/> class.
    /// </summary>
    public AppViewModel(ILogger<AppViewModel> logger)
        => _logger = logger;

    /// <summary>
    /// 强制更新指定预设的头像.
    /// </summary>
    /// <param name="presetId">预设标识符.</param>
    public void ForceUpdatePresetAvatar(string presetId)
        => PresetAvatarUpdateRequested?.Invoke(this, presetId);

    /// <summary>
    /// 显示消息通知.
    /// </summary>
    /// <param name="message">消息.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task ShowMessageDialogAsync(string message)
    {
        var tipDialog = new TipDialog(message)
        {
            XamlRoot = ActivatedWindow.Content.XamlRoot,
        };
        await tipDialog.ShowAsync();
    }

    /// <summary>
    /// 显示消息通知.
    /// </summary>
    /// <param name="messageName">消息.</param>
    /// <returns><see cref="Task"/>.</returns>
    public Task ShowMessageDialogAsync(StringNames messageName)
        => ShowMessageDialogAsync(ResourceToolkit.GetLocalizedString(messageName));

    /// <summary>
    /// 修改主题.
    /// </summary>
    /// <param name="theme">主题类型.</param>
    public void ChangeTheme(ElementTheme theme)
    {
        if (ActivatedWindow == null)
        {
            return;
        }

        (ActivatedWindow.Content as FrameworkElement).RequestedTheme = theme;
        if (theme == ElementTheme.Dark)
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
        }
        else if (theme == ElementTheme.Light)
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
        }
        else
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = default;
        }
    }

    /// <summary>
    /// 显示提示.
    /// </summary>
    [RelayCommand]
    private async Task ShowTipAsync((string, InfoType) data)
    {
        if (ActivatedWindow is ITipWindow tipWindow)
        {
            await tipWindow.ShowTipAsync(data.Item1, data.Item2);
        }
        else
        {
            var firstWindow = DisplayWindows.OfType<ITipWindow>().FirstOrDefault();
            if (firstWindow is not null)
            {
                await firstWindow.ShowTipAsync(data.Item1, data.Item2);
            }
        }
    }
}
