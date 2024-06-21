// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.QianFan;
using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;

namespace RodelDraw.Core.Providers;

/// <summary>
/// 百度千帆服务提供者.
/// </summary>
public sealed class QianFanProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QianFanProvider"/> class.
    /// </summary>
    public QianFanProvider(QianFanClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SecretKey = config.Secret;
        ServerModels = PredefinedModels.QianFanModels;
    }

    private string SecretKey { get; }

    /// <inheritdoc/>
    public DrawExecutionSettings ConvertExecutionSettings(DrawSession sessionData)
    {
        var size = sessionData.Request?.Size ?? "768x768";
        var split = size.Split('x');
        var width = int.Parse(split[0]);
        var height = int.Parse(split[1]);
        return new QianFanDrawExecutionSettings
        {
            ModelId = sessionData.Model,
            Width = width,
            Height = height,
            NegativePrompt = sessionData?.Request?.NegativePrompt,
        };
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddQianFanImageGeneration(modelId, AccessKey, SecretKey)
                .Build();
        }

        return Kernel;
    }
}
