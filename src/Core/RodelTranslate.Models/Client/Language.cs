// Copyright (c) Rodel. All rights reserved.

using System;

namespace RodelTranslate.Models.Client;

/// <summary>
/// 语言.
/// </summary>
public sealed class Language
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Language"/> class.
    /// </summary>
    public Language()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Language"/> class.
    /// </summary>
    public Language(string id, string isoCode)
    {
        Id = id;
        ISOCode = isoCode;
    }

    /// <summary>
    /// 在对应服务下语言的标识符.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 标准 ISO 639-1 语言代码.
    /// </summary>
    public string ISOCode { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Language language && Id == language.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
