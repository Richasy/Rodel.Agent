// Copyright (c) Richasy. All rights reserved.

using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace RodelAgent.Samples.Plugin;

/// <summary>
/// BiliBili plugin.
/// </summary>
[DisplayName("哔哩哔哩插件")]
[Description("该插件可以获取哔哩哔哩相关的信息")]
public sealed class BiliPlugin
{
    /// <summary>
    /// 获取哔哩哔哩视频信息.
    /// </summary>
    /// <returns>热搜信息.</returns>
    [KernelFunction]
    [Description("获取哔哩哔哩热搜.")]
#pragma warning disable CA1822 // 将成员标记为 static
    public async Task<string> GetHotSearchAsync(CancellationToken cancellationToken = default)
#pragma warning restore CA1822 // 将成员标记为 static
    {
        await Task.Delay(1000, cancellationToken);
        var text = """
            1. 星海飞驰
            2. 鬼灭之刃
            3. 百大
            4. 圣斗士
            5. balabala
            """;
        return text;
    }
}
