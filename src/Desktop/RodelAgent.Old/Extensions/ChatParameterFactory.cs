// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Extensions;

/// <summary>
/// 聊天参数工厂.
/// </summary>
public sealed class ChatParametersFactory : IChatParametersFactory
{
    /// <inheritdoc/>
    public BaseFieldParameters CreateChatParameters(ProviderType provider)
    {
        return provider switch
        {
            ProviderType.SparkDesk => new SparkDeskProvider.SparkDeskChatParameters(),
            ProviderType.Gemini => new GeminiProvider.GeminiChatParameters(),
            ProviderType.Anthropic => new AnthropicProvider.AnthropicChatParameters(),
            ProviderType.QianFan => new QianFanProvider.QianFanChatParameters(),
            ProviderType.HunYuan => new HunYuanProvider.HunYuanChatParameters(),
            ProviderType.ZhiPu => new ZhiPuProvider.ZhiPuChatParameters(),
            ProviderType.DouBao => new DouBaoProvider.DouBaoChatParameters(),
            _ => new ProviderBase.OpenAIChatParameters(),
        };
    }
}
