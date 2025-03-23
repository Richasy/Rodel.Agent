// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// <see cref="AIFunction"/> 的视图模型.
/// </summary>
public sealed partial class AIFunctionItemViewModel : ViewModelBase<AIFunction>
{
    /// <summary>
    /// 初始化 <see cref="AIFunctionItemViewModel"/> 类的新实例.
    /// </summary>
    public AIFunctionItemViewModel(AIFunction data)
        : base(data)
    {
        Name = data.Name;
        Description = data.Description;
    }

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial string? Description { get; set; }
}
