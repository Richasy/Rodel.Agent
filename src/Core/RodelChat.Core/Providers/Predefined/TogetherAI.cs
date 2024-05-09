// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Chat;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> TogetherAIModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "LLaMA-2-7B-32K-Instruct (7B)",
            Id = "togethercomputer/Llama-2-7B-32K-Instruct",
            MaxTokens = 32_768,
        },
        new ChatModel
        {
            DisplayName = "Mistral (7B) Instruct",
            Id = "mistralai/Mistral-7B-Instruct-v0.1",
            MaxTokens = 8192,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Mixtral-8x7B Instruct",
            Id = "mistralai/Mixtral-8x7B-Instruct-v0.1",
            MaxTokens = 32_768,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Qwen 1.5 Chat (72B)",
            Id = "Qwen/Qwen1.5-72B-Chat",
            MaxTokens = 32_768,
        },
    };
}
