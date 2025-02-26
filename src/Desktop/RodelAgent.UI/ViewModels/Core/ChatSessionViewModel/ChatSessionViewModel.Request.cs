// Copyright (c) Richasy. All rights reserved.

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
            _cancellationTokenSource = new CancellationTokenSource();
            var options = new ChatOptions
            {
                ModelId = SelectedModel?.Id,
            };

            var chatMessage = new ChatMessage(ChatRole.User, UserInput);
            AttachChatMessageProperties(chatMessage);
            Messages.Add(chatMessage.ToInteropMessage());
            AddInteropMessageCommand.Execute(chatMessage);
            await SaveCurrentMessagesAsync();
            UserInput = string.Empty;
            var responseMessage = string.Empty;
            await foreach (var msg in _chatService!.Client!.CompleteStreamingAsync(Messages.ToList().ConvertAll(p => p.ToChatMessage()), options, _cancellationTokenSource.Token))
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
