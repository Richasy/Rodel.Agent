// Copyright (c) Richasy. All rights reserved.

using RodelTranslate.Models.Client;

namespace RodelTranslate.Core;

/// <summary>
/// 预定义的语言.
/// </summary>
internal static partial class PredefinedLanguages
{
    internal static List<Language> TencentLanguages { get; } = new List<Language>
    {
        new ("zh", "zh-Hans"),
        new ("zh-TW", "zh-Hant"),
        new ("en", "en"),
        new ("ja", "ja"),
        new ("ko", "ko"),
        new ("fr", "fr"),
        new ("es", "es"),
        new ("it", "it"),
        new ("de", "de"),
        new ("tr", "tr"),
        new ("ru", "ru"),
        new ("pt", "pt"),
        new ("vi", "vi"),
        new ("id", "id"),
        new ("th", "th"),
        new ("ms", "ms"),
        new ("ar", "ar"),
        new ("hi", "hi"),
    };
}
