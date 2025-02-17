// Copyright (c) Richasy. All rights reserved.

using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;

namespace RodelDraw.Core.Providers;

/// <summary>
/// 讯飞星火服务提供者.
/// </summary>
public sealed class SparkDeskProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkDeskProvider"/> class.
    /// </summary>
    public SparkDeskProvider(SparkDeskClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SecretKey = config.Secret;
        AppId = config.AppId;
        ServerModels = PredefinedModels.SparkDeskModels;
    }

    private string SecretKey { get; }

    private string AppId { get; }

    /// <inheritdoc/>
    public DrawParameters ConvertDrawParameters(DrawSession sessionData)
    {
        var size = sessionData.Request?.Size ?? "512x512";
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
                .AddSparkDeskImageGeneration(AccessKey, SecretKey, AppId, modelId)
                .Build();
        }

        return Kernel;
    }
}
