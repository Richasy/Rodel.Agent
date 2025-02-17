// Copyright (c) Richasy. All rights reserved.

using RodelDraw.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<DrawModel> OpenAIModels { get; } = new List<DrawModel>
    {
        new DrawModel
        {
            Id = "dall-e-3",
            DisplayName = "DALL-E 3",
            IsNegativeSupport = false,
            SupportSizes = ["1024x1024", "1792x1024", "1024x1792"],
        },
        new DrawModel
        {
            Id = "dall-e-2",
            DisplayName = "DALL-E 2",
            IsNegativeSupport = false,
            SupportSizes = ["256x256", "512x512", "1024x1024"],
        },
    };
}
