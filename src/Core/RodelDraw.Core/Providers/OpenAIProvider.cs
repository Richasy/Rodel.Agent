// Copyright (c) Rodel. All rights reserved.

using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using RodelAgent.Models.Abstractions;
using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;

namespace RodelDraw.Core.Providers;

/// <summary>
/// Open AI 服务商.
/// </summary>
public sealed class OpenAIProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIProvider"/> class.
    /// </summary>
    public OpenAIProvider(OpenAIClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.OpenAIApi, config.Endpoint);
        ServerModels = PredefinedModels.OpenAIModels;
        OrganizationId = config.OrganizationId;
    }

    /// <summary>
    /// 组织标识符.
    /// </summary>
    private string OrganizationId { get; }

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
                .AddOpenAITextToImage(AccessKey, OrganizationId, modelId, BaseUri?.ToString())
                .Build();
        }

        return Kernel;
    }

    /// <summary>
    /// Open AI 绘图参数.
    /// </summary>
    [JsonConverter(typeof(BaseFieldParametersConverter))]
    public class OpenAIDrawParameters : BaseFieldParameters
    {
        /// <summary>
        /// 生成数量.
        /// </summary>
        [RangeIntField(1, 10)]
        [JsonPropertyName("n")]
        public int? Number { get; set; }
    }
}
