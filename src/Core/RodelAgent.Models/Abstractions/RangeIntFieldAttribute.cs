// Copyright (c) Rodel. All rights reserved.

using System;
using RodelAgent.Models.Constants;

namespace RodelAgent.Models.Abstractions;

/// <summary>
/// 整数范围字段属性.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RangeIntFieldAttribute : BaseFieldAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RangeIntFieldAttribute"/> class.
    /// </summary>
    public RangeIntFieldAttribute(int minimum, int maximum)
        : base(ParameterFieldType.RangeInt)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// 最小值.
    /// </summary>
    public int Minimum { get; }

    /// <summary>
    /// 最大值.
    /// </summary>
    public int Maximum { get; }
}
