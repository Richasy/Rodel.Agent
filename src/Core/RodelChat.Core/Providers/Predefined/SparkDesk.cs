// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Chat;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> SparkDeskModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "V1.5",
            Id = "V1_5",
        },
        new ChatModel
        {
            DisplayName = "V2.0",
            Id = "V2",
        },
        new ChatModel
        {
            DisplayName = "V3.0",
            Id = "V3",
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "V3.5",
            Id = "V3_5",
            IsSupportTool = true,
        },
    };
}
