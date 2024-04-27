// Copyright (c) Rodel. All rights reserved.

using OpenAI;
using RodelChat.Core.Models.Chat;
using RodelChat.Core.Models.Constants;

namespace RodelChat.Core;

/// <summary>
/// 聊天客户端的请求部分.
/// </summary>
public sealed partial class ChatClient
{
    private async Task<ChatResponse> OpenAISendMessageAsync(OpenAIClient client, ChatSession session, string message, string toolChoice = null, Action<string> streamingAction = null, CancellationToken cancellationToken = default)
    {
        var chatRequest = GetOpenAIChatRequest(session, message, toolChoice);

        var responseContent = string.Empty;
        List<Tool>? tools = default;

        // TODO: Support tool call.
        if (session.UseStreamOutput)
        {
            await foreach (var partialResponse in client.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest, cancellationToken))
            {
                var partialChoice = partialResponse.Choices.FirstOrDefault(p => !string.IsNullOrEmpty(p.Delta?.Content));
                var content = partialChoice?.Delta?.Content;
                if (!string.IsNullOrEmpty(content))
                {
                    streamingAction?.Invoke(content);
                }

                responseContent += content;
            }
        }
        else
        {
            var response = await client.ChatEndpoint.GetCompletionAsync(chatRequest, cancellationToken);
            var choice = response.FirstChoice;
            if (choice.Message.ToolCalls != null && choice.Message.ToolCalls.Count > 0)
            {
                tools ??= new List<Tool>();
                foreach (var t in choice.Message.ToolCalls)
                {
                    tools.Add(t);
                }
            }

            responseContent = response.FirstChoice.Message.ToString();
        }

        var msg = !string.IsNullOrEmpty(responseContent)
            ? ChatMessage.CreateAssistantMessage(responseContent)
            : tools != null && tools.Count > 0
                ? ChatMessage.CreateAssistantMessage(string.Empty, tools)
                : ChatMessage.CreateClientMessage(ClientMessageType.EmptyResponseContent, string.Empty);

        return new ChatResponse
        {
            Message = msg,
            Tools = tools,
        };
    }
}
