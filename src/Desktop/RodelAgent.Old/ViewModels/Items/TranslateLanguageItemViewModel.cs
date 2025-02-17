// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Toolkits;
using RodelTranslate.Models.Client;
using System.Globalization;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 翻译语言视图模型.
/// </summary>
public sealed partial class TranslateLanguageItemViewModel : ViewModelBase<Language>
{
    [ObservableProperty]
    private string _displayName;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateLanguageItemViewModel"/> class.
    /// </summary>
    public TranslateLanguageItemViewModel(Language data)
        : base(data)
    {
        if (data != null)
        {
            var culture = new CultureInfo(data.ISOCode);
            DisplayName = culture.DisplayName;
        }
        else
        {
            DisplayName = ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.AutoDetected);
        }
    }

    /// <inheritdoc/>
    public override string ToString()
        => DisplayName;
}
