// Copyright (c) Rodel. All rights reserved.

using System;
using System.ComponentModel.DataAnnotations;

namespace RodelAgent.Models.Common;

/// <summary>
/// 元数据.
/// </summary>
public class Metadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Metadata"/> class.
    /// </summary>
    public Metadata()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Metadata"/> class.
    /// </summary>
    public Metadata(string id, string value)
    {
        Id = id;
        Value = value;
    }

    /// <summary>
    /// 键名.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// 键值.
    /// </summary>
    public string Value { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Metadata metadata && Id == metadata.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
