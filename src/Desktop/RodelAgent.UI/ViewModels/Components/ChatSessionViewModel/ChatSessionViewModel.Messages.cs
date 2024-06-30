// Copyright (c) Rodel. All rights reserved.

using System.Threading;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 聊天会话视图模型关于消息收发的部分.
/// </summary>
public sealed partial class ChatSessionViewModel
{
    private static string GetClientMessageContent(ChatMessage message)
    {
        var text = message.ClientMessageType switch
        {
            ClientMessageType.ModelNotSupportImage => ResourceToolkit.GetLocalizedString(StringNames.ModelNotSupportImage),
            ClientMessageType.ProviderNotSupported => ResourceToolkit.GetLocalizedString(StringNames.ProviderNotSupported),
            ClientMessageType.GenerateCancelled => ResourceToolkit.GetLocalizedString(StringNames.GenerateCancelled),
            _ => string.Empty,
        };

        if (string.IsNullOrEmpty(text))
        {
            text = message.Content.FirstOrDefault(p => p.Type == ChatContentType.Text)?.Text ?? ResourceToolkit.GetLocalizedString(StringNames.UnknowError);
        }

        return text;
    }

    [RelayCommand]
    private void AddUserInput()
    {
        if (string.IsNullOrEmpty(UserInput) || IsResponding)
        {
            return;
        }

        var msg = CreateUserMessage();
        Data.Messages.Add(msg);
        Messages.Add(new ChatMessageItemViewModel(
                       msg,
                       EditMessageAsync,
                       DeleteMessageAsync));
        UserInput = string.Empty;
        SaveSessionToDatabaseCommand.Execute(default);
    }

    [RelayCommand]
    private async Task SendAsync()
    {
        if (string.IsNullOrEmpty(UserInput) || IsResponding)
        {
            return;
        }

        await SendMessageInternalAsync();
    }

    [RelayCommand]
    private void CancelMessage()
    {
        if (_cancellationTokenSource != null && _cancellationTokenSource.Token.CanBeCanceled)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception)
            {
            }

            _cancellationTokenSource = default;
            _ = _dispatcherQueue.TryEnqueue(() =>
            {
                TempMessage = string.Empty;
                var lastUserMsg = Messages.LastOrDefault(p => p.IsUser);
                if (lastUserMsg is not null)
                {
                    _ = Messages.Remove(lastUserMsg);
                    Data.Messages.Remove(lastUserMsg.Data);
                    UserInput = lastUserMsg.Content;

                    SaveSessionToDatabaseCommand.Execute(default);
                }
            });
        }

        RequestFocusInput?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task ClearMessageAsync()
    {
        Messages.Clear();
        UserInput = string.Empty;
        ErrorText = string.Empty;
        Data.Messages.Clear();
        RequestFocusInput?.Invoke(this, EventArgs.Empty);
        await SaveSessionToDatabaseAsync(true);
    }

    [RelayCommand]
    private async Task RegenerateAsync()
    {
        var lastUserMessage = Messages.LastOrDefault(p => p.IsUser);
        if (lastUserMessage is null)
        {
            return;
        }

        UserInput = lastUserMessage.Content;
        TempMessage = string.Empty;
        Data.Messages.RemoveAt(Data.Messages.Count - 1);
        Data.Messages.Remove(lastUserMessage.Data);
        Messages.RemoveAt(Messages.Count - 1);
        await SaveSessionToDatabaseAsync();
        await SendMessageInternalAsync(false);
    }

    private ChatMessage CreateUserMessage()
    {
        var chatMessage = new ChatMessage
        {
            Role = MessageRole.User,
            Content = new()
            {
                new ChatMessageContent
                {
                    Text = UserInput,
                    Type = ChatContentType.Text,
                },
            },
            Time = DateTimeOffset.Now,
        };

        return chatMessage;
    }

    private async Task SendMessageInternalAsync(bool addUserMsg = true)
    {
        CancelMessage();
        _cancellationTokenSource = new CancellationTokenSource();

        ErrorText = string.Empty;
        TempMessage = string.Empty;
        GeneratingTipText = ResourceToolkit.GetLocalizedString(StringNames.Generating);

        // TODO: 支持多模态消息.
        var chatMessage = CreateUserMessage();
        UserInput = "\n";
        UserInput = string.Empty;

        if (addUserMsg)
        {
            Messages.Add(new ChatMessageItemViewModel(
                               chatMessage,
                               EditMessageAsync,
                               DeleteMessageAsync));
        }

        var selectedPlugins = Plugins.Where(p => p.IsSelected).Select(p => p.Data).ToList();
        var functions = selectedPlugins.SelectMany(p => p);
        List<Microsoft.SemanticKernel.KernelPlugin> tempPlugins = functions.Count() > 0 ? [Microsoft.SemanticKernel.KernelPluginFactory.CreateFromFunctions("temp", functions)] : default;
        var response = await _chatClient.SendMessageAsync(
                SessionId,
                chatMessage,
                Data.Model,
                text =>
                {
                    _ = _dispatcherQueue.TryEnqueue(() =>
                    {
                        if (_cancellationTokenSource is null || _cancellationTokenSource.IsCancellationRequested)
                        {
                            return;
                        }

                        TempMessage += text;
                    });
                },
                tempPlugins,
                _cancellationTokenSource.Token);

        if (response.Content.Count > 0 && string.IsNullOrEmpty(response.GetFirstTextContent()))
        {
            response.Content.First(p => p.Type == ChatContentType.Text).Text = TempMessage;
        }

        if (_cancellationTokenSource is null || _cancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        CheckPluginSelectedStatus();
        await SaveSessionToDatabaseAsync();

        if (response.Role == MessageRole.Assistant)
        {
            Messages.Add(new ChatMessageItemViewModel(
            response,
            EditMessageAsync,
            DeleteMessageAsync));
        }
        else if (response.Role == MessageRole.Client)
        {
            ErrorText = GetClientMessageContent(response);
            _logger.LogError("Send message failed: {0}", ErrorText);
        }

        TempMessage = string.Empty;
        RequestFocusInput?.Invoke(this, EventArgs.Empty);
        _cancellationTokenSource = null;
    }

    private Task EditMessageAsync(ChatMessage msg)
        => SaveSessionToDatabaseAsync();

    private async Task DeleteMessageAsync(ChatMessage msg)
    {
        var source = Messages.FirstOrDefault(p => p.Data.Equals(msg));
        Messages.Remove(source);
        Data.Messages.Remove(msg);

        await SaveSessionToDatabaseAsync();
    }

    private void HandleSendMessageException(Exception ex)
    {
        ErrorText = ex.Message;
        _logger.LogDebug(ex, "Failed to send message.");
        CancelMessage();
    }
}
