// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> OpenRouterModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Auto",
            Id = "openrouter/auto",
            ContextLength = 128_000,
        },
        new ChatModel
        {
            DisplayName = "Mistral 7B Instruct (free)",
            Id = "mistralai/mistral-7b-instruct:free",
            ContextLength = 32_768,
        },
        new ChatModel
        {
            DisplayName = "Yi 34B Chat",
            Id = "01-ai/yi-34b-chat",
            ContextLength = 4096,
        },
    };
}
