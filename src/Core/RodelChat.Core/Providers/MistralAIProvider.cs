// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// Mistral AI 服务商.
/// </summary>
public sealed class MistralAIProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MistralAIProvider"/> class.
    /// </summary>
    public MistralAIProvider(MistralAIClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        SetBaseUri(ProviderConstants.MistralAIApi);
        ServerModels = PredefinedModels.MistralAIModels;
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
#pragma warning disable SKEXP0070 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
            Kernel = Kernel.CreateBuilder()
                .AddMistralChatCompletion(modelId, AccessKey)
                .Build();
#pragma warning restore SKEXP0070 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
        }

        return Kernel;
    }
}
