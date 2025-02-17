// Copyright (c) Richasy. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Migration.V1.Models;

/// <summary>
/// 助理.
/// </summary>
internal sealed class Assistant
{
    /// <summary>
    /// 助理标识符.
    /// </summary>
    [Key]
    public string Id { get; set; } = null!;

    /// <summary>
    /// 助理名称.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 助理描述.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 内核类型.
    /// </summary>
    public KernelType Kernel { get; set; }

    /// <summary>
    /// 模型名称.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// 指令（系统提示词）.
    /// </summary>
    public string? Instruction { get; set; }

    /// <summary>
    /// 备注信息.
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 是否使用默认内核.
    /// </summary>
    public bool UseDefaultKernel { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Assistant assistant && Id == assistant.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
