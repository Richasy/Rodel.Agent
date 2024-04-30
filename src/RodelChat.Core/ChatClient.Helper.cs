// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.QianFan;
using Microsoft.SemanticKernel.Connectors.SparkDesk;
using Microsoft.SemanticKernel.Services;
using RodelChat.Core.Models.Chat;
using RodelChat.Core.Models.Constants;
using RodelChat.Core.Models.Providers;

namespace RodelChat.Core;

/// <summary>
/// 聊天客户端的帮助方法.
/// </summary>
public sealed partial class ChatClient
{
    private static Microsoft.SemanticKernel.ChatMessageContent ConvertToKernelMessage(ChatMessage message)
    {
        var role = ConvertToRole(message.Role);
        return new Microsoft.SemanticKernel.ChatMessageContent(role, ConvertToContentItemCollection(message.Content.ToArray()));
    }

    private static AuthorRole ConvertToRole(MessageRole role)
        => role switch
        {
            MessageRole.System => AuthorRole.System,
            MessageRole.User => AuthorRole.User,
            MessageRole.Assistant => AuthorRole.Assistant,
            _ => AuthorRole.Tool,
        };

    private static ChatMessageContentItemCollection ConvertToContentItemCollection(Models.Chat.ChatMessageContent[] contents)
    {
        var items = new ChatMessageContentItemCollection();
        foreach (var item in contents)
        {
            if (item.Type == ChatContentType.Text)
            {
                items.Add(new TextContent(item.Text));
            }
            else if (item.Type == ChatContentType.ImageUrl)
            {
                items.Add(new ImageContent(new Uri(item.Text)));
            }
        }

        return items;
    }

    private static OpenAIClientOptions.ServiceVersion ConvertAzureOpenAIVersion(AzureOpenAIVersion version)
    {
        return version switch
        {
            AzureOpenAIVersion.V2022_12_01 => OpenAIClientOptions.ServiceVersion.V2022_12_01,
            AzureOpenAIVersion.V2023_05_15 or AzureOpenAIVersion.V2023_10_01_Preview => OpenAIClientOptions.ServiceVersion.V2023_05_15,
            AzureOpenAIVersion.V2023_06_01_Preview => OpenAIClientOptions.ServiceVersion.V2023_06_01_Preview,
            AzureOpenAIVersion.V2024_02_15_Preview => OpenAIClientOptions.ServiceVersion.V2024_02_15_Preview,
            AzureOpenAIVersion.V2024_03_01_Preview => OpenAIClientOptions.ServiceVersion.V2024_03_01_Preview,
            AzureOpenAIVersion.V2024_02_01 => OpenAIClientOptions.ServiceVersion.V2024_02_15_Preview,
            _ => throw new NotSupportedException("Version not supported."),
        };
    }

    private static SparkDeskAIVersion ConvertToSparkVersion(string modelId)
        => modelId switch
        {
            "V1_5" => SparkDeskAIVersion.V1_5,
            "V2" => SparkDeskAIVersion.V2,
            "V3" => SparkDeskAIVersion.V3,
            "V3_5" => SparkDeskAIVersion.V3_5,
            _ => throw new NotSupportedException("Version not supported."),
        };

    private static List<ChatMessage> CreateHistoryCopyAndAddUserInput(ChatSession session, ChatMessage? message = default)
    {
        if (message != null)
        {
            session.History.Add(message);
        }

        var history = JsonSerializer.Deserialize<List<ChatMessage>>(JsonSerializer.Serialize(session.History));
        if (!string.IsNullOrEmpty(session.SystemInstruction))
        {
            history.Insert(0, ChatMessage.CreateSystemMessage(session.SystemInstruction));
        }

        return history;
    }

    private static ChatHistory GetChatHistory(ChatSession session, ChatMessage? message = null)
    {
        var history = new ChatHistory();
        if (!string.IsNullOrEmpty(session.SystemInstruction))
        {
            history.AddSystemMessage(session.SystemInstruction);
        }

        foreach (var item in session.History)
        {
            if (item.Role != MessageRole.Client)
            {
                history.Add(ConvertToKernelMessage(item));
            }
        }

        return history;
    }

    private static PromptExecutionSettings GetExecutionSettings(ChatSession session)
    {
        switch (session.Provider)
        {
            case ProviderType.Gemini:
                return new GeminiPromptExecutionSettings
                {
                    TopP = session.Parameters.TopP,
                    MaxTokens = session.Parameters.MaxTokens,
                    Temperature = session.Parameters.Temperature,
                    ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
                };
            case ProviderType.SparkDesk:
                return new SparkDeskPromptExecutionSettings
                {
                    MaxTokens = session.Parameters.MaxTokens,
                    Temperature = session.Parameters.Temperature,
                    ToolCallBehavior = SparkDeskToolCallBehavior.AutoInvokeKernelFunctions,
                };
            case ProviderType.QianFan:
                return new QianFanPromptExecutionSettings
                {
                    MaxTokens = session.Parameters.MaxTokens,
                    Temperature = session.Parameters.Temperature,
                    TopP = session.Parameters.TopP,
                    PenaltyScore = session.Parameters.FrequencyPenalty + 1,
                };
            default:
                return new OpenAIPromptExecutionSettings
                {
                    PresencePenalty = session.Parameters.PresencePenalty,
                    FrequencyPenalty = session.Parameters.FrequencyPenalty,
                    MaxTokens = session.Parameters.MaxTokens,
                    Temperature = session.Parameters.Temperature,
                    TopP = session.Parameters.TopP,
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                };
        }
    }

    private Kernel FindKernelProvider(ProviderType type, string modelId)
    {
        if (type == ProviderType.AzureOpenAI)
        {
            var shouldRecreate = true;
            if (_azureOpenAIKernel != null)
            {
                var depId = _azureOpenAIKernel.GetRequiredService<IChatCompletionService>().Attributes.GetValueOrDefault("DeploymentName").ToString();
                shouldRecreate = depId != modelId;
            }

            if (shouldRecreate)
            {
                var version = ConvertAzureOpenAIVersion(_azureOpenAIProvider.Version);
                var builder = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(modelId, _azureOpenAIProvider.BaseUrl, _azureOpenAIProvider.AccessKey, apiVersion: version);
                InitializePlugins(builder);
                _azureOpenAIKernel = builder.Build();
            }

            return _azureOpenAIKernel;
        }
        else if (type == ProviderType.OpenAI)
        {
            if (ShouldKernelRecreate(_openAIKernel))
            {
                _openAIKernel = CreateOpenAIKernel(_openAIProvider);
            }

            return _openAIKernel;
        }
        else if (type == ProviderType.Zhipu)
        {
            if (ShouldKernelRecreate(_zhipuKernel))
            {
                _zhipuKernel = CreateOpenAIKernel(_zhipuProvider);
            }

            return _zhipuKernel;
        }
        else if (type == ProviderType.LingYi)
        {
            if (ShouldKernelRecreate(_lingYiKernel))
            {
                _lingYiKernel = CreateOpenAIKernel(_lingYiProvider);
            }

            return _lingYiKernel;
        }
        else if (type == ProviderType.Moonshot)
        {
            if (ShouldKernelRecreate(_moonshotKernel))
            {
                _moonshotKernel = CreateOpenAIKernel(_moonshotProvider);
            }

            return _moonshotKernel;
        }
        else if (type == ProviderType.DashScope)
        {
            if (ShouldKernelRecreate(_dashScopeKernel))
            {
                _dashScopeKernel = CreateOpenAIKernel(_dashScopeProvider);
            }

            return _dashScopeKernel;
        }
        else if (type == ProviderType.Groq)
        {
            if (ShouldKernelRecreate(_groqKernel))
            {
                _groqKernel = CreateOpenAIKernel(_groqProvider);
            }

            return _groqKernel;
        }
        else if (type == ProviderType.MistralAI)
        {
            if (ShouldKernelRecreate(_mistralAIKernel))
            {
                _mistralAIKernel = CreateOpenAIKernel(_mistralAIProvider);
            }

            return _mistralAIKernel;
        }
        else if (type == ProviderType.Perplexity)
        {
            if (ShouldKernelRecreate(_perplexityKernel))
            {
                _perplexityKernel = CreateOpenAIKernel(_perplexityProvider);
            }

            return _perplexityKernel;
        }
        else if (type == ProviderType.Gemini)
        {
            if (ShouldKernelRecreate(_geminiKernel))
            {
                var builder = Kernel.CreateBuilder()
                    .AddGoogleAIGeminiChatCompletion(modelId, _geminiProvider.AccessKey);
                InitializePlugins(builder);
                _geminiKernel = builder.Build();
            }

            return _geminiKernel;
        }
        else if (type == ProviderType.SparkDesk)
        {
            if (ShouldKernelRecreate(_sparkDeskKernel))
            {
                var builder = Kernel.CreateBuilder()
                    .AddSparkDeskChatCompletion(_sparkDeskProvider.AccessKey, _sparkDeskProvider.Secret, _sparkDeskProvider.AppId, ConvertToSparkVersion(modelId));
                InitializePlugins(builder);
                _sparkDeskKernel = builder.Build();
            }

            return _sparkDeskKernel;
        }
        else if (type == ProviderType.QianFan)
        {
            if (ShouldKernelRecreate(_qianFanKernel))
            {
                var builder = Kernel.CreateBuilder()
                    .AddQianFanChatCompletion(modelId, _qianFanProvider.AccessKey, _qianFanProvider.Secret);
                _qianFanKernel = builder.Build();
            }

            return _qianFanKernel;
        }

        return default;

        bool ShouldKernelRecreate(Kernel? kernel)
        {
            if (kernel == null)
            {
                return true;
            }

            var chatService = kernel.GetRequiredService<IChatCompletionService>();
            var model = chatService.Attributes.GetValueOrDefault(AIServiceExtensions.ModelIdKey).ToString();
            return model != modelId;
        }

        Kernel CreateOpenAIKernel(ProviderBase provider)
        {
            var builder = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(modelId, new Uri(provider.BaseUrl), provider.AccessKey);
            InitializePlugins(builder);
            return builder.Build();
        }

        void InitializePlugins(IKernelBuilder builder)
        {
            var model = FindModelInProvider(type, modelId);
            if (_plugins != null && model.IsSupportTool)
            {
                foreach (var item in _plugins)
                {
                    builder.Plugins.AddFromObject(item);
                }
            }
        }
    }

    private ProviderBase GetProvider(ProviderType type)
    {
        return type switch
        {
            ProviderType.OpenAI => _openAIProvider,
            ProviderType.AzureOpenAI => _azureOpenAIProvider,
            ProviderType.Zhipu => _zhipuProvider,
            ProviderType.LingYi => _lingYiProvider,
            ProviderType.Moonshot => _moonshotProvider,
            ProviderType.DashScope => _dashScopeProvider,
            ProviderType.QianFan => _qianFanProvider,
            ProviderType.SparkDesk => _sparkDeskProvider,
            ProviderType.Gemini => _geminiProvider,
            ProviderType.Groq => _groqProvider,
            _ => throw new NotSupportedException("Provider not supported."),
        };
    }

    private ChatModel FindModelInProvider(ProviderType type, string modelId)
    {
        var provider = GetProvider(type);
        return (provider?.ServerModels ?? new List<ChatModel>())
            .Concat(provider?.CustomModels ?? new List<ChatModel>())
            .FirstOrDefault(m => m.Id == modelId);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _openAIKernel = null;
            _zhipuKernel = null;
            _lingYiKernel = null;
            _moonshotKernel = null;
            _azureOpenAIKernel = null;
            _dashScopeKernel = null;
            _qianFanKernel = null;
            _sparkDeskKernel = null;
            _geminiKernel = null;
            _sparkDeskKernel = null;
            _qianFanKernel = null;
            _dashScopeKernel = null;
            _groqKernel = null;
            _disposedValue = true;
        }
    }
}
