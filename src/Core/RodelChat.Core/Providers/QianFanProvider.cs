// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.QianFan;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Chat;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// 百度千帆提供程序.
/// </summary>
public sealed class QianFanProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QianFanProvider"/> class.
    /// </summary>
    public QianFanProvider(QianFanClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.QianFanApi);
        Secret = config.Secret;
        ServerModels = PredefinedModels.QianFanModels;
    }

    /// <summary>
    /// 获取密钥.
    /// </summary>
    private string Secret { get; }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddQianFanChatCompletion(modelId, AccessKey, Secret)
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings ConvertExecutionSettings(ChatParameters parameters)
        => new QianFanPromptExecutionSettings
        {
            MaxTokens = parameters.MaxTokens,
            Temperature = parameters.Temperature,
            TopP = parameters.TopP,
            PenaltyScore = parameters.FrequencyPenalty + 1,
        };
}
