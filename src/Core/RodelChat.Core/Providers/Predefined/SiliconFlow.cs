// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> SiliconFlowModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "DeepSeek Chat V2",
            Id = "deepseek-ai/DeepSeek-V2-Chat",
            ContextLength = 32_768,
        },
        new ChatModel
        {
            DisplayName = "Qwen2 72B",
            Id = "Qwen/Qwen2-72B-Instruct",
            ContextLength = 32_768,
        },
        new ChatModel
        {
            DisplayName = "ChatGLM4 9B",
            Id = "THUDM/glm-4-9b-chat",
            ContextLength = 32_768,
        },
    };
}
