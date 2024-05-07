// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using RodelChat.Models.Chat;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> MistralAIModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Mistral 7B",
            Id = "open-mistral-7b",
            Tokens = 32_768,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Mixtral 8x7b",
            Id = "open-mixtral-8x7b",
            Tokens = 32_768,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Mixtral 8x22B",
            Id = "open-mixtral-8x22b",
            Tokens = 65_536,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Mistral Small",
            Id = "mistral-small-latest",
            Tokens = 32_768,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Mistral Large",
            Id = "mistral-large-latest",
            Tokens = 32_768,
            IsSupportTool = true,
        },
    };
}
