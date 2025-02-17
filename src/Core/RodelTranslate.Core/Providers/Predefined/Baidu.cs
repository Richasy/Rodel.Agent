// Copyright (c) Richasy. All rights reserved.

using RodelTranslate.Models.Client;

namespace RodelTranslate.Core;

/// <summary>
/// 预定义的语言.
/// </summary>
internal static partial class PredefinedLanguages
{
    internal static List<Language> BaiduLanguages { get; } = new List<Language>
    {
        new ("zh", "zh-Hans"),
        new ("cht", "zh-Hant"),
        new ("yue", "yue"),
        new ("en", "en"),
        new ("jp", "ja"),
        new ("kor", "ko"),
        new ("fra", "fr"),
        new ("spa", "es"),
        new ("th", "th"),
        new ("ara", "ar"),
        new ("ru", "ru"),
        new ("pt", "pt"),
        new ("de", "de"),
        new ("it", "it"),
        new ("el", "el"),
        new ("nl", "nl"),
        new ("pl", "pl"),
        new ("bul", "bg"),
        new ("est", "et"),
        new ("dan", "da"),
        new ("fin", "fi"),
        new ("cs", "cs"),
        new ("rom", "ro"),
        new ("slo", "sk"),
        new ("swe", "sv"),
        new ("hu", "hu"),
        new ("vie", "vi"),
    };
}
