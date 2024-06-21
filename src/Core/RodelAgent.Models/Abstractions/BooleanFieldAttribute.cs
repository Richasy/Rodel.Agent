// Copyright (c) Rodel. All rights reserved.

using System;
using RodelAgent.Models.Constants;

namespace RodelAgent.Models.Abstractions;

/// <summary>
/// 布尔字段属性.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class BooleanFieldAttribute : BaseFieldAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanFieldAttribute"/> class.
    /// </summary>
    public BooleanFieldAttribute()
        : base(ParameterFieldType.Boolean)
    {
    }
}
