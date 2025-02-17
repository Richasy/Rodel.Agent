// Copyright (c) Richasy. All rights reserved.

using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;
using System.Text.Json.Serialization;

namespace RodelTranslate.Core.Providers;

/// <summary>
/// Azure 服务提供商.
/// </summary>
public sealed class AzureProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureProvider"/> class.
    /// </summary>
    public AzureProvider(AzureClientConfig config)
        : base(config.Key)
    {
        Region = config.Region;
        Languages = PredefinedLanguages.AzureLanguages;
    }

    private string Region { get; }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel()
    {
        Kernel ??= Kernel.CreateBuilder()
                .AddAzureTextTranslation(AccessKey, Region)
                .Build();
        return Kernel;
    }

    /// <inheritdoc/>
    public override TranslateExecutionSettings ConvertExecutionSettings(TranslateSession sessionData)
        => new AzureTranslateExecutionSettings
        {
            From = sessionData.SourceLanguage?.Id,
            To = sessionData.TargetLanguage?.Id,
            TextType = Enum.Parse<TextType>(sessionData.Parameters.GetValueOrDefault<string>(nameof(AzureTranslateParameters.Format))),
        };

    /// <inheritdoc/>
    public override long GetMaxTextLength() => 50_000;

    /// <summary>
    /// Azure 翻译参数.
    /// </summary>
    [JsonConverter(typeof(BaseFieldParametersConverter))]
    public sealed class AzureTranslateParameters : BaseFieldParameters
    {
        /// <summary>
        /// 待翻译文本的格式.
        /// </summary>
        [JsonPropertyName("format")]
        [SelectionField("Plain", "Html")]
        public string? Format { get; set; } = "Plain";
    }
}
