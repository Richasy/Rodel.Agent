// Copyright (c) Rodel. All rights reserved.

using RodelDraw.Models.Client;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 绘图会话项视图模型.
/// </summary>
public sealed partial class DrawSessionItemViewModel : ViewModelBase<DrawSession>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawSessionItemViewModel"/> class.
    /// </summary>
    public DrawSessionItemViewModel(DrawSession data)
        : base(data)
    {
    }
}
