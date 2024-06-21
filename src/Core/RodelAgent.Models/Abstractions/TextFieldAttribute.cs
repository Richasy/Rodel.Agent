// Copyright (c) Rodel. All rights reserved.

using System;
using RodelAgent.Models.Constants;

namespace RodelAgent.Models.Abstractions;

/// <summary>
/// 文本字段属性.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class TextFieldAttribute : BaseFieldAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextFieldAttribute"/> class.
    /// </summary>
    public TextFieldAttribute()
        : base(ParameterFieldType.Text)
    {
    }
}
