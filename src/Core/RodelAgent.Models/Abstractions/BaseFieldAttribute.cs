// Copyright (c) Rodel. All rights reserved.

using System;
using RodelAgent.Models.Constants;

namespace RodelAgent.Models.Abstractions;

/// <summary>
/// 字段类型属性.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public abstract class BaseFieldAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseFieldAttribute"/> class.
    /// </summary>
    /// <param name="fieldType">字段类型.</param>
    public BaseFieldAttribute(ParameterFieldType fieldType) => FieldType = fieldType;

    /// <summary>
    /// 字段类型.
    /// </summary>
    public ParameterFieldType FieldType { get; }
}
