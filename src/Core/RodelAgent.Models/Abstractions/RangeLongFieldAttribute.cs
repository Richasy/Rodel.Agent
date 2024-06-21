// Copyright (c) Rodel. All rights reserved.

using System;
using RodelAgent.Models.Constants;

namespace RodelAgent.Models.Abstractions;

/// <summary>
/// 整数范围字段属性.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RangeLongFieldAttribute : BaseFieldAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RangeLongFieldAttribute"/> class.
    /// </summary>
    public RangeLongFieldAttribute(long minimum, long maximum)
        : base(ParameterFieldType.RangeLong)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary>
    /// 最小值.
    /// </summary>
    public long Minimum { get; }

    /// <summary>
    /// 最大值.
    /// </summary>
    public long Maximum { get; }
}
