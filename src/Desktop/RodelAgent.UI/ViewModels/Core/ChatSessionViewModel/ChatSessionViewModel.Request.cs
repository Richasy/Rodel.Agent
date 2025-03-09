// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using RodelAgent.Models;
using RodelAgent.UI.Toolkits;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// Chat session view model.
/// </summary>
public sealed partial class ChatSessionViewModel
{
    public async Task<string> GenerateContentAsync(string prompt)
    {
        if (string.IsNullOrEmpty(prompt))
        {
            return string.Empty;
        }

        if (_chatService is null || SelectedModel is null)
        {
            throw new InvalidOperationException(ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.NeedSelectServiceAndModel));
        }

        var options = new ChatOptions();
        options.ModelId = SelectedModel?.Id;
        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, prompt),
        };

        var response = await _chatService!.Client!.GetResponseAsync(messages, options);
        return response.Message?.Text ?? string.Empty;
    }

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
            _cancellationTokenSource = new CancellationTokenSource();
            var options = _getCurrentOptions?.Invoke() ?? new ChatOptions();
            options.ModelId = SelectedModel?.Id;
            var useStream = _getIsStreamOutput?.Invoke() ?? true;
            var maxRounds = _getMaxRounds?.Invoke() ?? 0;

            // 检查对话轮次.
            var messages = Messages.ToList();
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

            var chatMessage = new ChatMessage(ChatRole.User, UserInput);
            AttachChatMessageProperties(chatMessage);
            Messages.Add(chatMessage.ToInteropMessage());
            messages.Add(chatMessage.ToInteropMessage());
            AddInteropMessageCommand.Execute(chatMessage);
            await SaveCurrentMessagesAsync();
            SetTempLoadingCommand.Execute(true);
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
        SetTempLoadingCommand.Execute(false);
    }

    private static void AttachChatMessageProperties(ChatMessage msg)
    {
        msg.AdditionalProperties ??= [];
        msg.AdditionalProperties.Add("id", Guid.NewGuid().ToString("N"));
        msg.AdditionalProperties.Add("time", DateTimeOffset.Now.ToUnixTimeSeconds());
    }
}
