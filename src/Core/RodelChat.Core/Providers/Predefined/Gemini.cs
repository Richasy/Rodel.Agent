// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> GeminiModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Gemini 1.5 Pro",
            Id = "gemini-1.5-pro",
            ContextLength = 1_048_576,
            MaxOutput = 8192,
        },
        new ChatModel
        {
            DisplayName = "Gemini 1.5 Flash",
            Id = "gemini-1.5-flash",
            ContextLength = 1_048_576,
            MaxOutput = 8192,
        },
        new ChatModel
        {
            DisplayName = "Gemini Pro",
            Id = "gemini-1.0-pro",
            ContextLength = 30_720,
            MaxOutput = 2048,
        },
        new ChatModel
        {
            DisplayName = "Gemini 1.0 Pro Vision",
            Id = "gemini-pro-vision",
            IsSupportVision = true,
            ContextLength = 12_288,
            MaxOutput = 4096,
        },
    };
}
