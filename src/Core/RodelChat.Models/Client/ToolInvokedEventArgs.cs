// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;

namespace RodelChat.Models.Client;

/// <summary>
/// 工具调用完成事件参数.
/// </summary>
public sealed class ToolInvokedEventArgs : ToolInvokingEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolInvokedEventArgs"/> class.
    /// </summary>
    public ToolInvokedEventArgs(FunctionInvocationContext context, string modelId)
        : base(context, modelId)
    {
        Result = context.Result;
    }

    /// <summary>
    /// 结果.
    /// </summary>
    public FunctionResult Result { get; }
}
