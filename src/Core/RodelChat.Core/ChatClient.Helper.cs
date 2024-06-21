// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using RodelAgent.Models.Abstractions;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;

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

    private static ChatMessageContentItemCollection ConvertToContentItemCollection(Models.Client.ChatMessageContent[] contents)
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

    private static ChatHistory GetChatHistory(ChatSession session)
    {
        var history = new ChatHistory();
        if (!string.IsNullOrEmpty(session.SystemInstruction))
        {
            history.AddSystemMessage(session.SystemInstruction);
        }

        var maxItems = (session.MaxRounds * 2) - 1;
        var messages = session.Messages.Where(p => p.Role != MessageRole.Client);
        if (maxItems > 0)
        {
            messages = messages.TakeLast(maxItems).ToList();
        }

        foreach (var item in messages)
        {
            if (item.Role != MessageRole.Client)
            {
                history.Add(ConvertToKernelMessage(item));
            }
        }

        return history;
    }

    private PromptExecutionSettings GetExecutionSettings(ChatSession session)
        => GetProvider(session.Provider).ConvertExecutionSettings(session);

    private Kernel? FindKernelProvider(ProviderType type, string modelId)
        => GetProvider(type).GetOrCreateKernel(modelId);

    private IProvider GetProvider(ProviderType type)
        => _providerFactory.GetOrCreateProvider(type);

    private ChatModel FindModelInProvider(ProviderType type, string modelId)
        => GetProvider(type).GetModelOrDefault(modelId);

    private void ResetSessionModel(ChatSession session, string? modelId = null)
    {
        if (!string.IsNullOrEmpty(modelId))
        {
            var model = FindModelInProvider(session.Provider, modelId);
            session.Model = model?.Id
                ?? throw new ArgumentException("Model not found.");
        }
        else if (string.IsNullOrEmpty(session.Model))
        {
            var provider = GetProvider(session.Provider);
            var firstModel = provider.GetModelList().Where(p => !p.IsDeprecated).FirstOrDefault()
                ?? throw new Exception("No model found.");
            session.Model = firstModel.Id;
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // Release managed resources.
                _providerFactory?.Clear();
            }

            _disposedValue = true;
        }
    }

    /// <summary>
    /// 获取聊天参数.
    /// </summary>
    /// <returns><see cref="BaseFieldParameters"/>.</returns>
    /// <remarks>
    /// <para>这个方法会首先根据 <paramref name="type"/> 获取默认的参数，然后再将 <paramref name="additionalParams"/> 中的字段合并到默认参数中.</para>
    /// <para>这样就允许插入可变数量的参数.</para>
    /// </remarks>
    private BaseFieldParameters GetChatParameters(ProviderType type, BaseFieldParameters? additionalParams = null)
    {
        var parameters = _parameterFactory.CreateChatParameters(type);
        if (additionalParams != null)
        {
            parameters.SetDictionary(additionalParams.Fields);
        }

        return parameters;
    }
}
