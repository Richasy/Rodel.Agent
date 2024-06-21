// Copyright (c) Rodel. All rights reserved.

using System;
using RodelAgent.Models.Constants;

namespace RodelAgent.Models.Abstractions;

/// <summary>
/// 浮点数范围字段属性.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RangeFloatFieldAttribute : BaseFieldAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RangeFloatFieldAttribute"/> class.
    /// </summary>
    public RangeFloatFieldAttribute(double minimum, double maximum)
        : base(ParameterFieldType.RangeFloat)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// 最小值.
    /// </summary>
    public double Minimum { get; }

    /// <summary>
    /// 最大值.
    /// </summary>
    public double Maximum { get; }
}
