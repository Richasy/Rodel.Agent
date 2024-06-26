﻿// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> LingYiModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Yi 34B Chat",
            Id = "yi-34b-chat-0205",
            ContextLength = 4000,
        },
        new ChatModel
        {
            DisplayName = "Yi 34B Chat 200k",
            Id = "yi-34b-chat-200k",
            ContextLength = 200_000,
        },
        new ChatModel
        {
            DisplayName = "Yi Vision Plus",
            Id = "yi-vl-plus",
            IsSupportVision = true,
            ContextLength = 4000,
        },
    };
}
