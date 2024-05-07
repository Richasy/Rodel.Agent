// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using RodelChat.Models.Chat;

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
            Id = "gemini-1.5-pro-latest",
            MaxTokens = 1_048_576,
            MaxOutput = 8192,
        },
        new ChatModel
        {
            DisplayName = "Gemini Pro",
            Id = "gemini-pro",
            MaxTokens = 30_720,
            MaxOutput = 2048,
        },
        new ChatModel
        {
            DisplayName = "Gemini 1.0 Pro Vision",
            Id = "gemini-pro-vision",
            IsSupportVision = true,
            MaxTokens = 12_288,
            MaxOutput = 4096,
        },
    };
}
