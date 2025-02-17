// Copyright (c) Richasy. All rights reserved.

using RodelDraw.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<DrawModel> HunYuanModels { get; } = new List<DrawModel>
    {
        new DrawModel
        {
            Id = "hunyuan_image",
            DisplayName = "混元文生图",
            IsNegativeSupport = false,
            SupportSizes = [
                "768x768",
                "768x1024",
                "1024x768",
                "720x1280",
                "1280x720",
                "768x1280",
                "1280x768",
                "1024x1024"
              ],
        },
    };
}
