// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using RodelAgent.Models.Abstractions;
using RodelAgent.Models.Constants;
using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;

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
        Version = config.Version;
    }

    /// <summary>
    /// 获取 API 版本.
    /// </summary>
    private AzureOpenAIVersion Version { get; }

    /// <inheritdoc/>
    public DrawExecutionSettings ConvertExecutionSettings(DrawSession sessionData)
    {
        var size = sessionData.Request?.Size ?? "1024x1024";
        var split = size.Split('x');
        var width = int.Parse(split[0]);
        var height = int.Parse(split[1]);
        return new OpenAIDrawExecutionSettings
        {
            ModelId = sessionData.Model,
            Width = width,
            Height = height,
            Number = sessionData.Parameters.GetValueOrDefault<int>(nameof(AzureOpenAIDrawParameters.Number)),
        };
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddAzureOpenAITextToImage(modelId, endpoint: BaseUri.AbsoluteUri, apiKey: AccessKey, apiVersion: JsonSerializer.Serialize(Version), modelId: modelId)
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
