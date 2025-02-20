// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel.Models;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 绘图尺寸项视图模型.
/// </summary>
public sealed partial class DrawSizeItemViewModel(DrawSize size) : ViewModelBase
{
    /// <summary>
    /// 数据.
    /// </summary>
    public DrawSize Data { get; } = size;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is DrawSizeItemViewModel model && Data.Equals(model.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Data);

    /// <inheritdoc/>
    public override string ToString() => $"{Data.Width}x{Data.Height}";
}
