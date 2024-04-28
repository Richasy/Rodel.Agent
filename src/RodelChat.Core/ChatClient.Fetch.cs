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
    private async Task<ChatResponse> OpenAISendMessageAsync(OpenAIClient client, ChatSession session, ChatMessage message, string toolChoice = null, Action<string> streamingAction = null, CancellationToken cancellationToken = default)
    {
        var chatRequest = GetOpenAIChatRequest(session, message, toolChoice);

        var responseContent = string.Empty;
        List<Tool>? tools = default;

        // TODO: Support tool call.
        if (session.UseStreamOutput)
        {
            await foreach (var partialResponse in client.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest, cancellationToken))
            {
                var partialChoice = partialResponse.Choices.FirstOrDefault(p => !string.IsNullOrEmpty(p.Delta?.Content) || p.Delta?.ToolCalls != null);
                if (partialChoice == null)
                {
                    continue;
                }

                var content = partialChoice?.Delta?.Content;
                if (partialChoice.Delta.ToolCalls != null && partialChoice.Delta.ToolCalls.Count > 0 && partialChoice.FinishReason == "tool_calls")
                {
                    tools ??= new List<Tool>();
                    foreach (var t in partialChoice.Delta.ToolCalls)
                    {
                        if (!string.IsNullOrEmpty(t.Function?.Name))
                        {
                            tools.Add(t);
                        }
                    }
                }

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

    private async Task<ChatResponse> DashScopeSendMessageAsync(ChatSession session, ChatMessage message, string toolChoice = null, Action<string> streamingAction = null, CancellationToken cancellationToken = default)
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

        return new ChatResponse
        {
            Message = msg,
        };
    }

    private async Task<ChatResponse> QianFanSendMessageAsync(ChatSession session, ChatMessage message, string toolChoice = null, Action<string> streamingAction = null, CancellationToken cancellationToken = default)
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

        return new ChatResponse
        {
            Message = msg,
        };
    }

    private async Task<ChatResponse> SparkDeskSendMessageAsync(ChatSession session, ChatMessage message, string toolChoice = null, Action<string> streamingAction = null, CancellationToken cancellationToken = default)
    {
        var (messages, parameters) = GetSparkDeskRequest(session, message);
        var model = FindModelInProvider(session.Provider!.Value, session.Model);
        var responseContent = string.Empty;
        if (session.UseStreamOutput)
        {
            await _sparkDeskClient.ChatAsStreamAsync(session.Model, messages.ToArray(), StreamCallback, parameters, cancellationToken: cancellationToken);
        }
        else
        {
            var response = await _sparkDeskClient.ChatAsync(session.Model, messages.ToArray(), parameters, cancellationToken: cancellationToken);
            responseContent = response.Text;
        }

        var msg = !string.IsNullOrEmpty(responseContent)
            ? ChatMessage.CreateAssistantMessage(responseContent)
            : ChatMessage.CreateClientMessage(ClientMessageType.EmptyResponseContent, string.Empty);

        return new ChatResponse
        {
            Message = msg,
        };

        void StreamCallback(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                streamingAction?.Invoke(content);
            }

            responseContent += content;
        }
    }
}
