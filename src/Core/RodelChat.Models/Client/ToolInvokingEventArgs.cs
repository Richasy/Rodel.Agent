// Copyright (c) Rodel. All rights reserved.

using System;
using Microsoft.SemanticKernel;

namespace RodelChat.Models.Client;

/// <summary>
/// 工具调用事件参数.
/// </summary>
public class ToolInvokingEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolInvokingEventArgs"/> class.
    /// </summary>
    public ToolInvokingEventArgs(FunctionInvocationContext context, string modelId)
    {
        Function = context.Function;
        ModelId = modelId;
        Parameters = context.Arguments;
    }

    /// <summary>
    /// 插件 ID.
    /// </summary>
    public KernelFunction Function { get; }

    /// <summary>
    /// 模型 ID.
    /// </summary>
    public string ModelId { get; }

    /// <summary>
    /// 参数.
    /// </summary>
    public KernelArguments Parameters { get; }
}
