// Copyright (c) Richasy. All rights reserved.

using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;
using System.Text.Json.Serialization;

namespace RodelTranslate.Core.Providers;

/// <summary>
/// Ali 服务提供商.
/// </summary>
public sealed class AliProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AliProvider"/> class.
    /// </summary>
    public AliProvider(AliClientConfig config)
        : base(config.Key)
    {
        Secret = config.Secret;
        Languages = PredefinedLanguages.AliLanguages;
    }

    private string Secret { get; }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel()
    {
        Kernel ??= Kernel.CreateBuilder()
                .AddAliTextTranslation(AccessKey, Secret)
                .Build();
        return Kernel;
    }

    /// <inheritdoc/>
    public override TranslateExecutionSettings ConvertExecutionSettings(TranslateSession sessionData)
        => new AliTranslateExecutionSettings
        {
            From = sessionData.SourceLanguage?.Id,
            To = sessionData.TargetLanguage?.Id,
            FormatType = Enum.Parse<FormatType>(sessionData.Parameters.GetValueOrDefault<string>(nameof(AliTranslateParameters.Format))),
        };

    /// <inheritdoc/>
    public override long GetMaxTextLength() => 5000;

    /// <summary>
    /// Ali 翻译参数.
    /// </summary>
    [JsonConverter(typeof(BaseFieldParametersConverter))]
    public sealed class AliTranslateParameters : BaseFieldParameters
    {
        /// <summary>
        /// 待翻译文本的格式.
        /// </summary>
        [JsonPropertyName("format")]
        [SelectionField("Text", "Html")]
        public string? Format { get; set; } = "Text";
    }
}
