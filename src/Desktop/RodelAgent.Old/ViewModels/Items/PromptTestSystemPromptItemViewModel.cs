// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 提示词测试系统提示项视图模型.
/// </summary>
public sealed partial class PromptTestSystemPromptItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _index;

    [ObservableProperty]
    private string _content;

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptTestSystemPromptItemViewModel"/> class.
    /// </summary>
    public PromptTestSystemPromptItemViewModel(int index, string content)
    {
        Index = index;
        Content = content;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is PromptTestSystemPromptItemViewModel model && Index == model.Index;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Index);
}
