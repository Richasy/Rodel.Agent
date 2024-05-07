// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using RodelChat.Models.Chat;
using RodelChat.Models.Constants;

namespace RodelChat.Core;

/// <summary>
/// 聊天客户端的请求部分.
/// </summary>
public sealed partial class ChatClient
{
    private async Task<ChatMessage> KernelSendMessageAsync(Kernel kernel, ChatSession session, ChatMessage message, Action<string> streamingAction = null, CancellationToken cancellationToken = default)
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
}
