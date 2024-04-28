// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Chat;

internal static partial class PredefinedModels
{
    internal static List<ChatModel> SparkDeskModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "V1.5",
            Id = "v1.1",
        },
        new ChatModel
        {
            DisplayName = "V2.0",
            Id = "v2.1",
        },
        new ChatModel
        {
            DisplayName = "V3.0",
            Id = "v3.1",
        },
        new ChatModel
        {
            DisplayName = "V3.5",
            Id = "v3.5",
        },
    };
}
