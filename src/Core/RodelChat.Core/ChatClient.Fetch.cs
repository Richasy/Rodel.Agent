// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using RodelChat.Models.Client;
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
            session.Messages.Add(message);
        }

        var history = GetChatHistory(session);
        var settings = GetExecutionSettings(session);
        var responseContent = string.Empty;
        var chatService = kernel.GetRequiredService<IChatCompletionService>();

        if (session.UseStreamOutput)
        {
            await foreach (var partialResponse in chatService.GetStreamingChatMessageContentsAsync(history, settings, kernel, cancellationToken: cancellationToken).ConfigureAwait(false))
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
            var response = await chatService.GetChatMessageContentAsync(history, settings, cancellationToken: cancellationToken, kernel: kernel).ConfigureAwait(false);
            responseContent = response.Content;
        }

        if (session.FilterCharacters != null)
        {
            responseContent = session.FilterCharacters.Aggregate(responseContent, (current, character) => current.Replace(character, string.Empty));
            responseContent = responseContent.Trim();
        }

        var msg = !string.IsNullOrEmpty(responseContent)
                ? ChatMessage.CreateAssistantMessage(responseContent)
                : ChatMessage.CreateClientMessage(ClientMessageType.EmptyResponseContent, string.Empty);

        return msg;
    }
}
