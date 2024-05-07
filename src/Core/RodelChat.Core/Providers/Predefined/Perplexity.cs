// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using RodelChat.Models.Chat;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> PerplexityModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Sonar Small Chat",
            Id = "sonar-small-chat",
            Tokens = 16_384,
        },
        new ChatModel
        {
            DisplayName = "Sonar Small Online",
            Id = "sonar-small-online",
            Tokens = 12_000,
        },
        new ChatModel
        {
            DisplayName = "Sonar Medium Chat",
            Id = "sonar-medium-chat",
            Tokens = 16_384,
        },
        new ChatModel
        {
            DisplayName = "Sonar Medium Online",
            Id = "sonar-medium-online",
            Tokens = 12_000,
        },
    };
}
