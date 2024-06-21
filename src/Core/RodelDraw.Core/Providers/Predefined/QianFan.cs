// Copyright (c) Rodel. All rights reserved.

using RodelDraw.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<DrawModel> QianFanModels { get; } = new List<DrawModel>
    {
        new DrawModel
        {
            Id = "sd_xl",
            DisplayName = "Stable-Diffusion-XL",
            IsNegativeSupport = true,
            SupportSizes = [
                "768x768",
                "1024x1024",
                "1536x1536",
                "2048x2048",
                "1024x768",
                "2048x1536",
                "768x1024",
                "1536x2048",
                "1024x576",
                "2048x1152",
                "576x1024",
                "1152x2048",
              ],
        },
    };
}
