// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using RodelChat.Models.Chat;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> GroqModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Llama3 8B",
            Id = "llama3-8b-8192",
            MaxTokens = 8192,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Llama3 70B",
            Id = "llama3-70b-8192",
            MaxOutput = 8192,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Mixtral 8x7b",
            Id = "mixtral-8x7b-32768",
            MaxTokens = 32_768,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Gemma 7b",
            Id = "gemma-7b-it",
            MaxTokens = 8192,
            IsSupportTool = true,
        },
    };
}
