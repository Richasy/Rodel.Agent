// Copyright (c) Richasy. All rights reserved.

using Microsoft.UI.Xaml.Markup;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// Localized text extension.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed class LocaleExtension : MarkupExtension
{
    /// <summary>
    /// Language name.
    /// </summary>
    public StringNames Name { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
        => ResourceToolkit.GetLocalizedString(Name);
}
