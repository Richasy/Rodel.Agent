// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> SparkDeskModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Spark Lite",
            Id = "v1.1",
        },
        new ChatModel
        {
            DisplayName = "Spark V2.0",
            Id = "v2.1",
        },
        new ChatModel
        {
            DisplayName = "Spark Pro",
            Id = "v3.1",
        },
        new ChatModel
        {
            DisplayName = "Spark Max",
            Id = "v3.5",
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Spark4.0 Ultra",
            Id = "v4.0",
            IsSupportTool = true,
        },
    };
}
