// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls;

/// <summary>
/// 聊天服务配置控件基类.
/// </summary>
public abstract class TranslateServiceConfigControlBase : LayoutUserControlBase<TranslateServiceItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateServiceConfigControlBase"/> class.
    /// </summary>
    protected TranslateServiceConfigControlBase() => IsTabStop = false;
}
