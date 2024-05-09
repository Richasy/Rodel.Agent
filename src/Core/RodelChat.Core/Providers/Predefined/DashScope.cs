// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Chat;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    // 通义千问对 Function call 的支持并不完善，无法兼容标准的 OpenAI 接口，因此暂时不支持工具调用。
    internal static List<ChatModel> DashScopeModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Qwen Turbo",
            Id = "qwen-turbo",
            MaxTokens = 6000,
        },
        new ChatModel
        {
            DisplayName = "Qwen Plus",
            Id = "qwen-plus",
            MaxTokens = 30_000,
        },
        new ChatModel
        {
            DisplayName = "Qwen Max",
            Id = "qwen-max",
            MaxTokens = 6000,
        },
        new ChatModel
        {
            DisplayName = "Qwen Max Long Context",
            Id = "qwen-max-longcontext",
            MaxTokens = 28_000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 1.5 72B Chat",
            Id = "qwen1.5-72b-chat",
            MaxTokens = 30_000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 1.5 32B Chat",
            Id = "qwen1.5-32b-chat",
            MaxTokens = 30_000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 1.5 14B Chat",
            Id = "qwen1.5-14b-chat",
            MaxTokens = 6000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 1.5 7B Chat",
            Id = "qwen1.5-7b-chat",
            MaxTokens = 6000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 72B Chat",
            Id = "qwen-72b-chat",
            MaxTokens = 30_000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 14B Chat",
            Id = "qwen-14b-chat",
            MaxTokens = 6000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 7B Chat",
            Id = "qwen-7b-chat",
            MaxTokens = 6000,
        },
    };
}
