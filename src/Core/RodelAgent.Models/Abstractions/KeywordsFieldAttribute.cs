// Copyright (c) Rodel. All rights reserved.

using System;
using RodelAgent.Models.Constants;

namespace RodelAgent.Models.Abstractions;

/// <summary>
/// 关键字字段属性.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class KeywordsFieldAttribute : BaseFieldAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeywordsFieldAttribute"/> class.
    /// </summary>
    public KeywordsFieldAttribute()
           : base(ParameterFieldType.Keywords)
    {
    }
}
