// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 聊天群组视图模型.
/// </summary>
public sealed partial class ChatGroupViewModel
{
    private static string GetClientMessageContent(ChatMessage message)
        => message.Content.FirstOrDefault(p => p.Type == ChatContentType.Text)?.Text ?? ResourceToolkit.GetLocalizedString(StringNames.UnknowError);

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
                if (Messages.Count == 1 && Messages.Last().IsUser)
                {
                    var lastUserMsg = Messages.Last();
                    Messages.Remove(lastUserMsg);
                    Data.Messages.Remove(lastUserMsg.Data);
                    UserInput = lastUserMsg.Content;

                    SaveSessionToDatabaseCommand.Execute(default);
                }
            });
        }

        ResetAgentSelection();
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
        if (IsResponding)
        {
            return;
        }

        try
        {
            IsResponding = true;
            CancelMessage();
            _cancellationTokenSource = new CancellationTokenSource();

            _currentGeneratingIndex = 0;
            ErrorText = string.Empty;
            UpdateGeneratingTip();
            UpdateAgentSelection();

            var chatMessage = CreateUserMessage();

            if (addUserMsg)
            {
                Messages.Add(new ChatMessageItemViewModel(
                                   chatMessage,
                                   EditMessageAsync,
                                   DeleteMessageAsync));
            }

            UserInput = string.Empty;
            var selectedAgents = Agents.Select(p => p.Data).ToList();
            await _chatClient.SendGroupMessageAsync(
                    SessionId,
                    chatMessage,
                    response =>
                    {
                        _ = _dispatcherQueue.TryEnqueue(() =>
                        {
                            if (_cancellationTokenSource is null && Messages.Count == 0)
                            {
                                return;
                            }

                            var name = response.Author;
                            var agent = Agents.FirstOrDefault(p => p.Data.Name == name);
                            if (agent != null)
                            {
                                var index = Agents.IndexOf(agent);
                                if (index >= Agents.Count - 1)
                                {
                                    index = IsResponding ? 0 : _currentGeneratingIndex;
                                }
                                else
                                {
                                    index = IsResponding ? index + 1 : _currentGeneratingIndex;
                                }

                                _currentGeneratingIndex = index;
                                UpdateGeneratingTip();
                                UpdateAgentSelection();
                            }

                            if (response.Role == MessageRole.Assistant)
                            {
                                var msg = new ChatMessageItemViewModel(
                                    response,
                                    EditMessageAsync,
                                    DeleteMessageAsync);
                                Messages.Add(msg);
                            }
                            else if (response.Role == MessageRole.Client)
                            {
                                ErrorText = GetClientMessageContent(response);
                                _logger.LogError("Send message failed: {0}", ErrorText);
                            }
                        });
                    },
                    selectedAgents,
                    _cancellationTokenSource.Token);

            if (_cancellationTokenSource is null || _cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            await SaveSessionToDatabaseAsync();
            ResetAgentSelection();
            RequestFocusInput?.Invoke(this, EventArgs.Empty);
            _cancellationTokenSource = null;
        }
        catch (Exception ex)
        {
            HandleSendMessageException(ex);
        }
        finally
        {
            IsResponding = false;
        }
    }

    private void UpdateGeneratingTip()
    {
        var currentAgent = Agents.Count > _currentGeneratingIndex && _currentGeneratingIndex >= 0 ? Agents[_currentGeneratingIndex] : default;
        var name = currentAgent?.Name ?? string.Empty;
        GeneratingTipText = $"{name} {ResourceToolkit.GetLocalizedString(StringNames.Generating)}".Trim();
    }

    private void UpdateAgentSelection()
    {
        for (var i = 0; i < Agents.Count; i++)
        {
            var agent = Agents[i];
            agent.IsSelected = i == _currentGeneratingIndex;
        }
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
        ResetAgentSelection();
    }

    private void ResetAgentSelection()
    {
        _currentGeneratingIndex = -1;
        UpdateAgentSelection();
    }
}
