// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Toolkits;
using System.Globalization;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 语言项视图模型.
/// </summary>
public sealed partial class LanguageItemViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Name { get; set; }

    /// <summary>
    /// 语言代码.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageItemViewModel"/> class.
    /// </summary>
    public LanguageItemViewModel(string code, CultureInfo? culture)
    {
        Code = code;
        Name = culture?.DisplayName ?? (culture != null ? code : ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AutoLanguage));
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is LanguageItemViewModel model && Code == model.Code;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Code);
}
