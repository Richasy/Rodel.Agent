// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.Tools;

namespace RodelAgent.UI.ViewModels.Items;

public sealed partial class AIToolsetItemViewModel : ViewModelBase
{
    public AIToolsetItemViewModel(CoreToolType type, List<AIFunction> functions)
    {
        ToolType = type;
        Functions = functions;
        var resourceToolkit = this.Get<IResourceToolkit>();
        ToolName = resourceToolkit.GetLocalizedString($"Tool_{type}_Name");
        ToolDescription = resourceToolkit.GetLocalizedString($"Tool_{type}_Description");
        ToolCount = functions.Count;
        ToolEmoji = type switch
        {
            CoreToolType.Weather => "🌤️",
            _ => string.Empty,
        };
    }

    [ObservableProperty]
    public partial string ToolName { get; set; }

    [ObservableProperty]
    public partial string ToolDescription { get; set; }

    [ObservableProperty]
    public partial int ToolCount { get; set; }

    [ObservableProperty]
    public partial string ToolEmoji { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public List<AIFunction> Functions { get; }

    public CoreToolType ToolType { get; }

    public override bool Equals(object? obj) => obj is AIToolsetItemViewModel model && ToolType == model.ToolType;

    public override int GetHashCode() => HashCode.Combine(ToolType);
}
