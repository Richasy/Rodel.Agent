// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using RodelChat.Core.Models.Chat;
using RodelChat.Core.Models.Constants;

namespace RodelChat.Core;

/// <summary>
/// 聊天客户端的请求部分.
/// </summary>
public sealed partial class ChatClient
{
    private static async Task<ChatMessage> KernelSendMessageAsync(Kernel kernel, ChatSession session, ChatMessage message, Action<string> streamingAction = null, CancellationToken cancellationToken = default)
    {
        if (message.Role == MessageRole.User)
        {
            session.History.Add(message);
        }

        var history = GetChatHistory(session, message);
        var settings = GetExecutionSettings(session);
        var responseContent = string.Empty;
        var chatService = kernel.GetRequiredService<IChatCompletionService>();

        // TODO: Support tool call.
        if (session.UseStreamOutput)
        {
            await foreach (var partialResponse in chatService.GetStreamingChatMessageContentsAsync(history, settings, kernel, cancellationToken: cancellationToken))
            {
                if (!string.IsNullOrEmpty(partialResponse.Content))
                {
                    streamingAction?.Invoke(partialResponse.Content);
                }

                responseContent += partialResponse.Content;
            }
        }
        else
        {
            var response = await chatService.GetChatMessageContentAsync(history, settings, cancellationToken: cancellationToken, kernel: kernel);
            responseContent = response.Content;
        }

        var msg = !string.IsNullOrEmpty(responseContent)
                ? ChatMessage.CreateAssistantMessage(responseContent)
                : ChatMessage.CreateClientMessage(ClientMessageType.EmptyResponseContent, string.Empty);

        return msg;
    }

    private async Task<ChatMessage> DashScopeSendMessageAsync(ChatSession session, ChatMessage message, Action<string> streamingAction = null, CancellationToken cancellationToken = default)
    {
        var data = GetDashScopeRequest(session, message);
        var model = FindModelInProvider(session.Provider!.Value, session.Model);

        var responseContent = string.Empty;
        if (model.IsSupportVision)
        {
            var msgs = data.Item1.OfType<Sdcb.DashScope.TextGeneration.ChatVLMessage>().ToList();
            await foreach (var partialResponse in _dashScopeClient.TextGeneration.ChatVLStreamedAsync(session.Model, msgs, data.Item2, cancellationToken))
            {
                var content = partialResponse.Output;
                if (!string.IsNullOrEmpty(content))
                {
                    streamingAction?.Invoke(content);
                }

                responseContent += content;
            }
        }
        else
        {
            var msgs = data.Item1.OfType<Sdcb.DashScope.TextGeneration.ChatMessage>().ToList();
            if (session.UseStreamOutput)
            {
                await foreach (var partialResponse in _dashScopeClient.TextGeneration.ChatStreamedAsync(session.Model, msgs, data.Item2, cancellationToken))
                {
                    var content = partialResponse.Output.Text;
                    if (!string.IsNullOrEmpty(content))
                    {
                        streamingAction?.Invoke(content);
                    }

                    responseContent += content;
                }
            }
            else
            {
                var response = await _dashScopeClient.TextGeneration.ChatAsync(session.Model, msgs, data.Item2, cancellationToken);
                responseContent = response.Output.Text;
            }
        }

        var msg = !string.IsNullOrEmpty(responseContent)
            ? ChatMessage.CreateAssistantMessage(responseContent)
            : ChatMessage.CreateClientMessage(ClientMessageType.EmptyResponseContent, string.Empty);

        return msg;
    }

    private async Task<ChatMessage> QianFanSendMessageAsync(ChatSession session, ChatMessage message, Action<string> streamingAction = null, CancellationToken cancellationToken = default)
    {
        var (messages, parameters) = GetQianFanRequest(session, message);
        var model = FindModelInProvider(session.Provider!.Value, session.Model);
        var responseContent = string.Empty;
        if (session.UseStreamOutput)
        {
            await foreach (var partialResponse in _qianFanClient.ChatAsStreamAsync(session.Model, messages, parameters, cancellationToken))
            {
                var content = partialResponse.Result;
                if (!string.IsNullOrEmpty(content))
                {
                    streamingAction?.Invoke(content);
                }

                responseContent += content;
            }
        }
        else
        {
            var response = await _qianFanClient.ChatAsync(session.Model, messages, parameters, cancellationToken);
            responseContent = response.Result;
        }

        var msg = !string.IsNullOrEmpty(responseContent)
            ? ChatMessage.CreateAssistantMessage(responseContent)
            : ChatMessage.CreateClientMessage(ClientMessageType.EmptyResponseContent, string.Empty);

        return msg;
    }
}
