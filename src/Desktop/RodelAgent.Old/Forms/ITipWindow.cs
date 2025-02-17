// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Forms;

/// <summary>
/// 可以显示提示的窗口.
/// </summary>
public interface ITipWindow
{
    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <param name="text">文本.</param>
    /// <param name="type">显示类型.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task ShowTipAsync(string text, InfoType type = InfoType.Error);
}
