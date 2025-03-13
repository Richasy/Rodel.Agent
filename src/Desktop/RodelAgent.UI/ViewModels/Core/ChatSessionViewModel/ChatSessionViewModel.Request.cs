// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Richasy.AgentKernel;
using Richasy.AgentKernel.Chat;
using RodelAgent.Models;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.View;
using System.Text.RegularExpressions;

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
    private async Task StartGenerateAsync(bool force = false)
    {
        if (string.IsNullOrEmpty(UserInput) && !force)
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
            if (IsGroup)
            {
                await GenerateGroupContentAsync();
            }
            else
            {
                await GenerateAgentContentAsync();
            }
        }
        catch (Exception ex)
        {
            _currentAgentIndex = 0;
            SetTempLoadingCommand.Execute(false);
            _logger.LogError(ex, "Failed to generate chat content");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ex.Message, InfoType.Error));
        }
        finally
        {
            IsGenerating = false;
            UpdateAgentSelection();
        }
    }

    [RelayCommand]
    private async Task RegenerateAsync()
    {
        var lastMessage = Messages.LastOrDefault();
        if (lastMessage is null)
        {
            return;
        }

        if (lastMessage.Role == "assistant")
        {
            Messages.Remove(lastMessage);
            await DeleteInteropMessageAsync(lastMessage.Id);
            await SaveCurrentMessagesAsync();
            await StartGenerateAsync(true);
        }
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

    private async Task GenerateAgentContentAsync()
    {
        var options = _getSessionCurrentOptions?.Invoke() ?? new ChatOptions();
        options.ModelId = SelectedModel?.Id;
        FormatChatOptions(options);
        var useStream = _getSessionIsStreamOutput?.Invoke() ?? true;
        var maxRounds = _getSessionMaxRounds?.Invoke() ?? 0;
        UpdateGeneratingTip();

        // 检查对话轮次.
        var messages = Messages.ToList();

        if (!string.IsNullOrEmpty(UserInput))
        {
            var chatMessage = new ChatMessage(ChatRole.User, UserInput);
            AttachChatMessageProperties(chatMessage);
            Messages.Add(chatMessage.ToInteropMessage());
            messages.Add(chatMessage.ToInteropMessage());
            AddInteropMessageCommand.Execute(chatMessage);
            await SaveCurrentMessagesAsync();
        }

        if (maxRounds > 0)
        {
            // 取 user 和 assistant 的消息列表.
            var userMessages = Messages.Where(p => p.Role == "user").ToList();
            var assistantMessages = Messages.Where(p => p.Role == "assistant").ToList();
            // 忽略最后一条user消息，然后取最后maxRounds-1条消息.
            var lastUserMessage = userMessages.LastOrDefault();
            if (userMessages.Count > 0)
            {
                userMessages.RemoveAt(userMessages.Count - 1);
            }

            if (userMessages.Count > maxRounds - 1)
            {
                userMessages.RemoveRange(0, userMessages.Count - maxRounds + 1);
            }

            if (assistantMessages.Count > maxRounds - 1)
            {
                assistantMessages.RemoveRange(0, assistantMessages.Count - maxRounds + 1);
            }

            // 重新按照时间顺序合并成一个消息列表.
            if (lastUserMessage != null)
            {
                userMessages.Add(lastUserMessage);
            }

            messages = userMessages.Concat(assistantMessages).OrderBy(p => p.Time).ToList();
        }

        if (IsAgent && CurrentAgent!.History is { Count: > 0 })
        {
            for (var i = CurrentAgent.History.Count - 1; i >= 0; i--)
            {
                var msg = CurrentAgent.History[i];
                messages.Insert(0, msg);
            }
        }

        SetTempLoadingCommand.Execute(true);
        UserInput = string.Empty;
        var responseMessage = string.Empty;


        if (!string.IsNullOrEmpty(SystemInstruction))
        {
            messages.Insert(0, new RodelAgent.Models.Feature.ChatInteropMessage { Role = "system", Message = SystemInstruction, Id = "system" });
        }

        if (useStream)
        {
            await foreach (var msg in _chatService!.Client!.GetStreamingResponseAsync(messages.ConvertAll(p => p.ToChatMessage(GetAgentName)), options, _cancellationTokenSource!.Token))
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
            var response = await _chatService!.Client!.GetResponseAsync(messages.ConvertAll(p => p.ToChatMessage(GetAgentName)), options, _cancellationTokenSource!.Token);
            responseMessage = response.Message.Text;
        }

        var responseMsg = new ChatMessage(ChatRole.Assistant, responseMessage?.Trim());
        if (IsAgent)
        {
            responseMsg.AuthorName = GetAgentName(CurrentAgent!.Id);
            responseMsg.AdditionalProperties ??= [];
            responseMsg.AdditionalProperties.Add("agentId", CurrentAgent!.Id);
        }

        AttachChatMessageProperties(responseMsg);
        Messages.Add(responseMsg.ToInteropMessage());
        await SaveCurrentMessagesAsync();
        AddInteropMessageCommand.Execute(responseMsg);
    }

    private async Task GenerateGroupContentAsync()
    {
        _currentAgentIndex = 0;
        var maxRounds = _getGroupMaxRounds?.Invoke() ?? 1;
        var stopSequence = CurrentGroup?.TerminateSequence ?? [];

        var currentRound = 0;
        var chatMessage = new ChatMessage(ChatRole.User, UserInput);
        AttachChatMessageProperties(chatMessage);
        Messages.Add(chatMessage.ToInteropMessage());
        AddInteropMessageCommand.Execute(chatMessage);
        await SaveCurrentMessagesAsync();
        SetTempLoadingCommand.Execute(true);
        UserInput = string.Empty;

        while (currentRound < maxRounds)
        {
            UpdateGeneratingTip();
            UpdateAgentSelection();
            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();
            var responseMessage = string.Empty;
            var currentAgent = Agents[_currentAgentIndex];
            var history = Messages.ToList();
            if (!string.IsNullOrEmpty(currentAgent.Data.SystemInstruction))
            {
                history.Insert(0, new RodelAgent.Models.Feature.ChatInteropMessage { Role = "system", Message = currentAgent.Data.SystemInstruction, Id = "system" });
            }

            var options = currentAgent.Data.Options ?? new ChatOptions();
            options.ModelId = currentAgent.Data.Model;
            FormatChatOptions(options);
            var chatService = this.Get<IChatService>(currentAgent.Data.Provider!.Value.ToString());
            var serviceConfig = await this.Get<IChatConfigManager>().GetServiceConfigAsync(currentAgent.Data.Provider!.Value, new(options.ModelId!, string.Empty));
            chatService.Initialize(serviceConfig);
            if (currentAgent.Data.UseStreamOutput ?? true)
            {
                await foreach (var msg in chatService!.Client!.GetStreamingResponseAsync(history.ConvertAll(p => p.ToChatMessage(GetAgentName)), options, _cancellationTokenSource!.Token))
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
                var response = await chatService!.Client!.GetResponseAsync(history.ConvertAll(p => p.ToChatMessage(GetAgentName)), options, _cancellationTokenSource!.Token);
                responseMessage = response.Message.Text;
            }

            // 检查是否满足终止条件.
            var canStop = false;
            if (stopSequence.Any(p => responseMessage!.Contains(p, StringComparison.Ordinal)))
            {
                canStop = true;
            }

            var responseMsg = new ChatMessage(ChatRole.Assistant, responseMessage?.Trim());
            responseMsg.AuthorName = GetAgentName(currentAgent.Data.Id);
            responseMsg.AdditionalProperties ??= [];
            responseMsg.AdditionalProperties.Add("agentId", currentAgent.Data.Id);
            AttachChatMessageProperties(responseMsg);
            Messages.Add(responseMsg.ToInteropMessage());
            await SaveCurrentMessagesAsync();
            AddInteropMessageCommand.Execute(responseMsg);

            if (canStop)
            {
                break;
            }

            _currentAgentIndex++;
            if (_currentAgentIndex >= Agents.Count)
            {
                _currentAgentIndex = 0;
                currentRound++;
            }
        }
    }

    private string GetAgentName(string agentId)
    {
        var name = string.Empty;
        if (CurrentAgent?.Id == agentId)
        {
            name = CurrentAgent.Name;
        }
        else if (Agents.Any(p => p.Data.Id == agentId))
        {
            name = Agents.First(p => p.Data.Id == agentId).Data.Name;
        }
        else
        {
            var agent = this.Get<ChatPageViewModel>().Agents.FirstOrDefault(p => p.Data.Id == agentId);
            if (agent != null)
            {
                name = agent.Data.Name;
            }
            else
            {
                return string.Empty;
            }
        }

        var regex = new Regex("[^a-zA-Z0-9_-]");
        var actualName = new string(name.Where(c => regex.IsMatch(c.ToString())).ToArray());
        return actualName;
    }

    private static void AttachChatMessageProperties(ChatMessage msg)
    {
        msg.AdditionalProperties ??= [];
        msg.AdditionalProperties.Add("id", Guid.NewGuid().ToString("N"));
        msg.AdditionalProperties.Add("time", DateTimeOffset.Now.ToUnixTimeSeconds());
    }

    private static void FormatChatOptions(ChatOptions options)
    {
        if (options.MaxOutputTokens == 0)
        {
            options.MaxOutputTokens = null;
        }
    }
}
