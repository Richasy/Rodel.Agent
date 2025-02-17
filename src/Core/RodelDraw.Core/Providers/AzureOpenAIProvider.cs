// Copyright (c) Richasy. All rights reserved.

using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;
using System.Text.Json.Serialization;

namespace RodelDraw.Core.Providers;

/// <summary>
/// Azure OpenAI 服务提供者.
/// </summary>
public sealed class AzureOpenAIProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIProvider"/> class.
    /// </summary>
    public AzureOpenAIProvider(AzureOpenAIClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(config.Endpoint);
    }

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
                .AddAzureOpenAITextToImage(modelId, endpoint: BaseUri.AbsoluteUri, apiKey: AccessKey, modelId: modelId)
                .Build();
        }

        return Kernel;
    }

    /// <summary>
    /// Azure Open AI 绘图参数.
    /// </summary>
    [JsonConverter(typeof(BaseFieldParametersConverter))]
    public sealed class AzureOpenAIDrawParameters : OpenAIProvider.OpenAIDrawParameters
    {
    }
}
