﻿// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using OpenAI;
using RodelChat.Core.Models.Chat;
using RodelChat.Core.Models.Constants;
using RodelChat.Core.Models.Providers;

namespace RodelChat.Core;

/// <summary>
/// 聊天客户端的帮助方法.
/// </summary>
public sealed partial class ChatClient
{
    private static string ExtractAzureResourceName(string url)
    {
        Uri uriResult;
        if (!Uri.TryCreate(url, UriKind.Absolute, out uriResult))
        {
            throw new ArgumentException("Invalid URL");
        }

        var host = uriResult.Host;
        var parts = host.Split('.');
        return parts.Length > 0 ? parts[0] : throw new Exception($"Resource name not found in {url}");
    }

    private static OpenAIClient CreateOpenAIClient(string apiKey, string proxyUrl = "", string organizationId = "")
    {
        var auth = new OpenAIAuthentication(apiKey, string.IsNullOrEmpty(organizationId) ? null : organizationId);

        var settings = OpenAIClientSettings.Default;
        if (!string.IsNullOrEmpty(proxyUrl))
        {
            var version = Uri.TryCreate(proxyUrl, UriKind.Absolute, out var uri) ? uri.Segments.LastOrDefault() ?? string.Empty : string.Empty;
            settings = new OpenAIClientSettings(proxyUrl.Replace(version, string.Empty).Trim('/'), version.Trim('/'));
        }

        return new OpenAIClient(auth, settings);
    }

    private static List<OpenAI.Chat.Message> CovnertToMessages(List<ChatMessage> messages)
    {
        var result = new List<OpenAI.Chat.Message>();
        foreach (var item in messages)
        {
            if (item.Role == MessageRole.Client)
            {
                continue;
            }

            result.Add(ConvertToMessage(item));
        }

        return result;
    }

    private static OpenAI.Chat.Message ConvertToMessage(ChatMessage message)
    {
        var role = ConvertToRole(message.Role);
        OpenAI.Chat.Message msg;
        if (role == Role.Assistant && !string.IsNullOrEmpty(message.ToolCalls))
        {
            var tools = JsonSerializer.Deserialize<List<Tool>>(message.ToolCalls);
            msg = new OpenAI.Chat.Message(Role.Assistant, message.Content.Select(ConvertToContent));
            msg.ToolCalls = tools;
        }
        else if (role == Role.Tool && !string.IsNullOrEmpty(message.ToolId))
        {
            msg = new OpenAI.Chat.Message(Role.Tool, message.Content.Select(ConvertToContent));
            msg.ToolCallId = message.ToolId;
        }
        else
        {
            msg = new OpenAI.Chat.Message(role, message.Content.Select(ConvertToContent), message.Name);
        }

        return msg;
    }

    private static OpenAI.Chat.Content ConvertToContent(ChatMessageContent content)
    {
        return content.Type switch
        {
            ChatContentType.Text => new OpenAI.Chat.Content(content.Text),
            ChatContentType.ImageUrl => ConvertToImageUrl(content),
            _ => throw new NotSupportedException("Content type not supported."),
        };
    }

    private static ImageUrl ConvertToImageUrl(ChatMessageContent content)
    {
        Enum.TryParse<ImageDetail>(content.Detail, true, out var detail);
        return new ImageUrl(content.Text, detail);
    }

    private static Role ConvertToRole(MessageRole role)
        => role switch
        {
            MessageRole.System => Role.System,
            MessageRole.User => Role.User,
            MessageRole.Assistant => Role.Assistant,
            _ => Role.Tool,
        };

    private static string ConvertAzureOpenAIVersionToString(AzureOpenAIVersion version)
    {
        return version switch
        {
            AzureOpenAIVersion.V2022_12_01 => "2022-12-01",
            AzureOpenAIVersion.V2023_05_15 => "2023-05-15",
            AzureOpenAIVersion.V2023_06_01_Preview => "2023-06-01-preview",
            AzureOpenAIVersion.V2023_10_01_Preview => "2023-10-01-preview",
            AzureOpenAIVersion.V2024_02_15_Preview => "2024-02-15-preview",
            AzureOpenAIVersion.V2024_03_01_Preview => "2024-03-01-preview",
            AzureOpenAIVersion.V2024_02_01 => "2024-02-01",
            _ => throw new NotSupportedException("Version not supported."),
        };
    }

    private OpenAI.Chat.ChatRequest GetOpenAIChatRequest(ChatSession session, ChatMessage? message = null, string toolChoice = null)
    {
        var model = FindModelInProvider(session.Provider ?? ProviderType.OpenAI, session.Model);
        if (message != null)
        {
            session.History.Add(message);
        }

        var history = JsonSerializer.Deserialize<List<ChatMessage>>(JsonSerializer.Serialize(session.History));
        if(!string.IsNullOrEmpty(session.SystemInstruction))
        {
            history.Insert(0, ChatMessage.CreateSystemMessage(session.SystemInstruction));
        }

        var messages = CovnertToMessages(session.History);
        return model?.IsSupportTool ?? false
            ? new OpenAI.Chat.ChatRequest(
                messages,
                Tools,
                toolChoice,
                session.Model,
                session.Parameters.FrequencyPenalty,
                maxTokens: session.Parameters.MaxTokens,
                presencePenalty: session.Parameters.PresencePenalty,
                temperature: session.Parameters.Temperature,
                topP: session.Parameters.TopP)
            : new OpenAI.Chat.ChatRequest(
                messages,
                session.Model,
                session.Parameters.FrequencyPenalty,
                maxTokens: session.Parameters.MaxTokens,
                presencePenalty: session.Parameters.PresencePenalty,
                temperature: session.Parameters.Temperature,
                topP: session.Parameters.TopP);
    }

    private OpenAIClient GetOpenAIClient(ProviderType type, string modelId = "")
    {
        if (type == ProviderType.AzureOpenAI && !string.IsNullOrEmpty(modelId) && _azureOpenAIClient.OpenAIClientSettings.DeploymentId != modelId)
        {
            _azureOpenAIClient.OpenAIClientSettings.UpdateDeploymentId(modelId);
        }

        return type switch
        {
            ProviderType.OpenAI => _openAIClient,
            ProviderType.AzureOpenAI => _azureOpenAIClient,
            ProviderType.Zhipu => _zhipuAIClient,
            ProviderType.LingYi => _lingYiAIClient,
            ProviderType.Moonshot => _moonshotAIClient,
            _ => throw new NotSupportedException("Provider not supported."),
        };
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
                _openAIClient?.Dispose();
                _zhipuAIClient?.Dispose();
                _lingYiAIClient?.Dispose();
                _moonshotAIClient?.Dispose();
                _azureOpenAIClient?.Dispose();
            }

            _openAIClient = null;
            _zhipuAIClient = null;
            _lingYiAIClient = null;
            _moonshotAIClient = null;
            _azureOpenAIClient = null;
            _disposedValue = true;
        }
    }
}
