// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 通知视图模型.
/// </summary>
public sealed partial class NotificationViewModel : INotificationViewModel
{
    [RelayCommand]
    private static async Task ShowTipAsync((string, InfoType) data)
        => await GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().ShowTipCommand.ExecuteAsync(data);
}
