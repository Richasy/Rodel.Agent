// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.SparkDesk;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Chat;
using RodelChat.Models.Client;

namespace RodelChat.Core.Providers;

/// <summary>
/// 百度千帆提供程序.
/// </summary>
public sealed class SparkDeskProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SparkDeskProvider"/> class.
    /// </summary>
    public SparkDeskProvider(SparkDeskClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        Secret = config.Secret;
        AppId = config.AppId;
        ServerModels = PredefinedModels.SparkDeskModels;
    }

    /// <summary>
    /// 获取应用程序 ID.
    /// </summary>
    private string AppId { get; }

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
                .AddSparkDeskChatCompletion(AccessKey, Secret, AppId, ConvertToSparkVersion(modelId))
                .Build();
        }

        return Kernel;
    }

    /// <inheritdoc/>
    public override PromptExecutionSettings ConvertExecutionSettings(ChatParameters parameters)
        => new SparkDeskPromptExecutionSettings
        {
            MaxTokens = parameters.MaxTokens,
            Temperature = parameters.Temperature,
            ToolCallBehavior = SparkDeskToolCallBehavior.AutoInvokeKernelFunctions,
        };

    private static SparkDeskAIVersion ConvertToSparkVersion(string modelId)
        => modelId switch
        {
            "V1_5" => SparkDeskAIVersion.V1_5,
            "V2" => SparkDeskAIVersion.V2,
            "V3" => SparkDeskAIVersion.V3,
            "V3_5" => SparkDeskAIVersion.V3_5,
            _ => throw new NotSupportedException("Version not supported."),
        };
}
