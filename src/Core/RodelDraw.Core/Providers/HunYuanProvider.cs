// Copyright (c) Richasy. All rights reserved.

using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;

namespace RodelDraw.Core.Providers;

/// <summary>
/// 腾讯混元服务提供者.
/// </summary>
public sealed class HunYuanProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HunYuanProvider"/> class.
    /// </summary>
    public HunYuanProvider(HunYuanClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        Secret = config.SecretId;
        ServerModels = PredefinedModels.HunYuanModels;
    }

    private string Secret { get; set; }

    /// <inheritdoc/>
    public DrawParameters ConvertDrawParameters(DrawSession sessionData)
    {
        var size = sessionData.Request?.Size ?? "1024x1024";
        var split = size.Split('x');
        var width = int.Parse(split[0]);
        var height = int.Parse(split[1]);
        return new DrawParameters(sessionData.Model, width, height);
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddHunYuanImageGeneration(modelId, Secret, AccessKey)
                .Build();
        }

        return Kernel;
    }
}
