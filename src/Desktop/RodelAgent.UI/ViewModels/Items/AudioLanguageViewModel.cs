// Copyright (c) Rodel. All rights reserved.

using System.Globalization;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 音频语言视图模型.
/// </summary>
public sealed partial class AudioLanguageViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioLanguageViewModel"/> class.
    /// </summary>
    public AudioLanguageViewModel(string code)
    {
        Code = code;
        var culture = new CultureInfo(code);
        Name = culture.DisplayName;
    }

    /// <summary>
    /// 语言代码.
    /// </summary>
    public string Code { get; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is AudioLanguageViewModel model && Code == model.Code;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Code);

    /// <inheritdoc/>
    public override string ToString() => Name;
}
