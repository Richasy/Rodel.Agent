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

    [ObservableProperty]
    private List<string>? _values;

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableItemViewModel"/> class.
    /// </summary>
    public VariableItemViewModel(
        string name,
        List<string>? selectValues = default,
        string? defaultValue = default)
    {
        Name = name;
        Values = selectValues;
        Value = defaultValue ?? Values?.FirstOrDefault() ?? string.Empty;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VariableItemViewModel model && Name == model.Name;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Name);
}
