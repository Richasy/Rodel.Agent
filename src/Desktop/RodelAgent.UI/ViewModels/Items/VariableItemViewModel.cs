// Copyright (c) Rodel. All rights reserved.

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 变量项视图模型.
/// </summary>
public sealed partial class VariableItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _value;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VariableItemViewModel model && Name == model.Name;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Name);
}
