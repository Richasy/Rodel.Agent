// Copyright (c) Rodel. All rights reserved.

using System;
using RodelAgent.Models.Constants;

namespace RodelAgent.Models.Abstractions;

/// <summary>
/// 单选字段属性.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SelectionFieldAttribute : BaseFieldAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionFieldAttribute"/> class.
    /// </summary>
    public SelectionFieldAttribute(params string[] options)
           : base(ParameterFieldType.Selection)
    {
        Options = options;
    }

    /// <summary>
    /// 选项.
    /// </summary>
    public string[] Options { get; }
}
