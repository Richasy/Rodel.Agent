// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> OpenAIModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "GPT-3.5 Turbo",
            Id = "gpt-3.5-turbo",
            IsSupportTool = true,
            ContextLength = 16_385,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 Turbo Preview",
            Id = "gpt-4-turbo-preview",
            IsSupportTool = true,
            ContextLength = 128_000,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 Turbo Vision Preview",
            Id = "gpt-4-vision-preview",
            ContextLength = 128_000,
            IsSupportVision = true,
            IsSupportBase64Image = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4",
            Id = "gpt-4",
            ContextLength = 8192,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 32K",
            Id = "gpt-4-32k",
            ContextLength = 32_768,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 Omni",
            Id = "gpt-4o",
            ContextLength = 128_000,
            IsSupportTool = true,
            IsSupportVision = true,
            IsSupportBase64Image = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4o mini",
            Id = "gpt-4o-mini",
            ContextLength = 128_000,
            IsSupportVision = true,
            IsSupportBase64Image = true,
        },
    };
}
