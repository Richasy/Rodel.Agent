﻿// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using RodelAgent.Models;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// Chat session view model.
/// </summary>
public sealed partial class ChatSessionViewModel
{
    [RelayCommand]
    private async Task StartGenerateAsync()
    {
        if (string.IsNullOrEmpty(UserInput))
        {
            return;
        }

        if (IsGenerating)
        {
            CancelGenerate();
        }

        IsGenerating = true;
        try
        {
            // 检查流式输出.
            var useStream = true;
            _cancellationTokenSource = new CancellationTokenSource();
            var options = _getCurrentOptions?.Invoke() ?? new ChatOptions();
            options.ModelId = SelectedModel?.Id;
            if (options.AdditionalProperties?.ContainsKey("stream") == true)
            {
                useStream = Convert.ToBoolean(options.AdditionalProperties["stream"]);
            }

            // 检查对话轮次.
            var messages = Messages.ToList();
            if (options.AdditionalProperties?.ContainsKey("max_rounds") == true)
            {
                var maxRounds = Convert.ToInt32(options.AdditionalProperties["max_rounds"]);
                if (maxRounds > 0)
                {
                    // 取 user 和 assistant 的消息列表.
                    var userMessages = Messages.Where(p => p.Role == "user").ToList();
                    var assistantMessages = Messages.Where(p => p.Role == "assistant").ToList();
                    // 各保留最后 maxRounds - 1 个消息.
                    if (userMessages.Count > maxRounds - 1)
                    {
                        userMessages.RemoveRange(0, userMessages.Count - maxRounds + 1);
                    }

                    if (assistantMessages.Count > maxRounds - 1)
                    {
                        assistantMessages.RemoveRange(0, assistantMessages.Count - maxRounds + 1);
                    }

                    // 重新按照时间顺序合并成一个消息列表.
                    messages = userMessages.Concat(assistantMessages).OrderBy(p => p.Time).ToList();
                }
            }

            var chatMessage = new ChatMessage(ChatRole.User, UserInput);
            AttachChatMessageProperties(chatMessage);
            Messages.Add(chatMessage.ToInteropMessage());
            messages.Add(chatMessage.ToInteropMessage());
            AddInteropMessageCommand.Execute(chatMessage);
            await SaveCurrentMessagesAsync();
            UserInput = string.Empty;
            var responseMessage = string.Empty;
            

            if (!string.IsNullOrEmpty(SystemInstruction))
            {
                messages.Insert(0, new RodelAgent.Models.Feature.ChatInteropMessage { Role = "system", Message = SystemInstruction, Id = "system" });
            }

            if (useStream)
            {
                await foreach (var msg in _chatService!.Client!.GetStreamingResponseAsync(messages.ConvertAll(p => p.ToChatMessage()), options, _cancellationTokenSource.Token))
                {
                    responseMessage += msg.Text;
#pragma warning disable CA1508 // 避免死条件代码
                    if (_cancellationTokenSource?.IsCancellationRequested != false)
                    {
                        return;
                    }
#pragma warning restore CA1508 // 避免死条件代码
                    SetTempResultCommand.Execute(responseMessage);
                }
            }
            else
            {
                var response = await _chatService!.Client!.GetResponseAsync(messages.ConvertAll(p => p.ToChatMessage()), options, _cancellationTokenSource.Token);
                responseMessage = response.Message.Text;
            }

            var responseMsg = new ChatMessage(ChatRole.Assistant, responseMessage?.Trim());
            AttachChatMessageProperties(responseMsg);
            Messages.Add(responseMsg.ToInteropMessage());
            await SaveCurrentMessagesAsync();
            AddInteropMessageCommand.Execute(responseMsg);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate chat content");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ex.Message, InfoType.Error));
        }
        finally
        {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private async Task RegenerateAsync()
    {
        // TODO: Implement regenerate logic
        await StartGenerateAsync();
    }

    [RelayCommand]
    private void CancelGenerate()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = default;
        IsGenerating = false;
        CancelTempResultCommand.Execute(default);
    }

    private static void AttachChatMessageProperties(ChatMessage msg)
    {
        msg.AdditionalProperties ??= [];
        msg.AdditionalProperties.Add("id", Guid.NewGuid().ToString("N"));
        msg.AdditionalProperties.Add("time", DateTimeOffset.Now.ToUnixTimeSeconds());
    }
}
