// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;
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

    private static dynamic ConvertToDashScopeMessage(ChatMessage message, bool isVisionMessage)
    {
        var role = message.Role.ToString().ToLower();
        if (isVisionMessage)
        {
            var contentList = new List<Sdcb.DashScope.TextGeneration.ContentItem>();
            message.Content.ForEach(c =>
            {
                if (c.Type == ChatContentType.Text)
                {
                    contentList.Add(Sdcb.DashScope.TextGeneration.ContentItem.FromText(c.Text));
                }
                else if (c.Type == ChatContentType.ImageUrl)
                {
                    contentList.Add(Sdcb.DashScope.TextGeneration.ContentItem.FromImage(c.Text));
                }
            });

            return new Sdcb.DashScope.TextGeneration.ChatVLMessage(role, contentList.ToArray());
        }
        else
        {
            return new Sdcb.DashScope.TextGeneration.ChatMessage(role, message.Content[0].Text);
        }
    }

    private static Sdcb.WenXinQianFan.ChatMessage ConvertToQianFanMessage(ChatMessage message)
    {
        var role = message.Role.ToString().ToLower();
        var content = message.Content[0].Text;
        return new Sdcb.WenXinQianFan.ChatMessage(role, content);
    }

    private static Sdcb.SparkDesk.ChatMessage ConvertToSparkDeskMessage(ChatMessage message)
    {
        var role = message.Role.ToString().ToLower();
        var content = message.Content[0].Text;
        return new Sdcb.SparkDesk.ChatMessage(role, content);
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

    private (List<dynamic>, Sdcb.DashScope.TextGeneration.ChatParameters) GetDashScopeRequest(ChatSession session, ChatMessage? message = null)
    {
        var history = CreateHistoryCopyAndAddUserInput(session, message);
        var msgs = new List<dynamic>();
        var model = FindModelInProvider(ProviderType.DashScope, session.Model);
        foreach (var item in history)
        {
            if (item.Role == MessageRole.Client)
            {
                continue;
            }

            var m = ConvertToDashScopeMessage(item, model.IsSupportVision);
            if (m is Sdcb.DashScope.TextGeneration.ChatMessage cm)
            {
                msgs.Add(cm);
            }
            else if (m is Sdcb.DashScope.TextGeneration.ChatVLMessage cvlm)
            {
                msgs.Add(cvlm);
            }
        }

        var parameters = new Sdcb.DashScope.TextGeneration.ChatParameters
        {
            ResultFormat = "text",
            MaxTokens = session.Parameters.MaxTokens,
            Temperature = (float)session.Parameters.Temperature,
            TopP = (float)session.Parameters.TopP,
            RepetitionPenalty = (float)session.Parameters.FrequencyPenalty + 1,
        };

        return (msgs, parameters);
    }

    private (List<Sdcb.WenXinQianFan.ChatMessage>, Sdcb.WenXinQianFan.ChatRequestParameters) GetQianFanRequest(ChatSession session, ChatMessage? message = null)
    {
        var history = CreateHistoryCopyAndAddUserInput(session, message);
        var model = FindModelInProvider(ProviderType.DashScope, session.Model);
        var messages = history.Where(p => p.Role != MessageRole.Client).Select(ConvertToQianFanMessage).ToList();
        var parameters = new Sdcb.WenXinQianFan.ChatRequestParameters
        {
            Temperature = (float)session.Parameters.Temperature,
            TopP = (float)session.Parameters.TopP,
            PenaltyScore = (float)session.Parameters.FrequencyPenalty + 1,
        };

        return (messages, parameters);
    }

    private (List<Sdcb.SparkDesk.ChatMessage>, Sdcb.SparkDesk.ChatRequestParameters) GetSparkDeskRequest(ChatSession session, ChatMessage? message = null)
    {
        var history = CreateHistoryCopyAndAddUserInput(session, message);
        var model = FindModelInProvider(ProviderType.DashScope, session.Model);
        var messages = history.Where(p => p.Role != MessageRole.Client).Select(ConvertToSparkDeskMessage).ToList();
        var parameters = new Sdcb.SparkDesk.ChatRequestParameters
        {
            Temperature = (float)session.Parameters.Temperature,
            MaxTokens = session.Parameters.MaxTokens,
        };

        return (messages, parameters);
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
            if (disposing)
            {
                _dashScopeClient?.Dispose();
                _qianFanClient?.Dispose();
            }

            _openAIKernel = null;
            _zhipuKernel = null;
            _lingYiKernel = null;
            _moonshotKernel = null;
            _azureOpenAIKernel = null;
            _dashScopeClient = null;
            _qianFanClient = null;
            _sparkDeskClient = null;
            _geminiKernel = null;
            _disposedValue = true;
        }
    }
}
