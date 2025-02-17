// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelChat.Models.Client;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 提示词测试页面视图模型.
/// </summary>
public sealed partial class PromptTestPageViewModel
{
    [RelayCommand]
    private void Generate()
    {
        // 初始化预设.
        var preset = Preset.Data.Clone();
        for (var i = 0; i < SystemPrompts.Count; i++)
        {
            var targetMessage = _predefinedMessages.Where(p => p.Role == RodelChat.Models.Constants.MessageRole.System).ElementAtOrDefault(i);
            if (targetMessage is ChatMessage message)
            {
                message.Content[0].Text = SystemPrompts[i].Content;
            }
        }

        var lastUserMessage = _predefinedMessages.LastOrDefault(p => p.Role == RodelChat.Models.Constants.MessageRole.User);
        if (lastUserMessage is ChatMessage userMessage)
        {
            var template = UserPromptTemplate;
            foreach (var variable in Variables)
            {
                if (variable.Name != _defaultInputVariable)
                {
                    template = template.Replace($"${variable.Name}", variable.Value);
                }
            }

            userMessage.Content.First().Text = template;
        }

        preset.Messages = _predefinedMessages.Take(_predefinedMessages.Count - 1).ToList();
        preset.Provider = SelectedService.ProviderType;
        preset.Model = SelectedModel.Id;
        preset.Id = Guid.NewGuid().ToString();
        if (_inputs is null || _inputs.Count == 0)
        {
            var input = string.IsNullOrEmpty(_defaultInputVariable) ? string.Empty : Variables.FirstOrDefault(p => p.Name == _defaultInputVariable)?.Value ?? string.Empty;
            var singleSession = new PromptTestSessionItemViewModel(_chatClient, 0, preset, ReplaceUserInputToLastMessage(input), input);
            Sessions = [singleSession];
        }
        else
        {
            var sessions = new List<PromptTestSessionItemViewModel>();
            var index = 0;
            foreach (var input in _inputs)
            {
                var session = new PromptTestSessionItemViewModel(_chatClient, index, preset, ReplaceUserInputToLastMessage(input), input);
                sessions.Add(session);
                index++;
            }

            Sessions = sessions;
        }

        IsFinished = false;
        RecordCurrentContext();
        CheckSessionCount();
        if (!IsSessionsEmpty)
        {
            IsGenerating = true;
            StartGeneratingTimer();
        }

        string ReplaceUserInputToLastMessage(string input)
        {
            var lastMessageContent = lastUserMessage.GetFirstTextContent();
            return string.IsNullOrEmpty(_defaultInputVariable)
                ? lastMessageContent
                : lastMessageContent.Replace($"${_defaultInputVariable}", input);
        }
    }

    private void RecordCurrentContext()
    {
        var currentVariables = Variables.Where(p => p.Name != _defaultInputVariable).Select(p => (p.Name, p.Value)).ToDictionary();
        _lastContext = string.Join('-', currentVariables.Select(p => $"{p.Key}:{p.Value}"));
    }

    private void StartGeneratingTimer()
    {
        if (_generateTimer is null)
        {
            _generateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300),
            };

            _generateTimer.Tick += OnGenerateTimerTick;
        }

        _generateTimer.Start();
    }

    [RelayCommand]
    private void CancelGenerating()
    {
        _generateTimer?.Stop();
        IsGenerating = false;
        foreach (var session in Sessions)
        {
            session.CancelCommand.Execute(default);
        }
    }

    [RelayCommand]
    private async Task ExportResultAsync()
    {
        var successSessions = Sessions.Where(p => p.IsSuccess).ToList();
        if (successSessions.Count == 0)
        {
            return;
        }

        var file = await FileToolkit.SaveFileAsync(".jsonl", this.Get<AppViewModel>().ActivatedWindow);
        if (file is null)
        {
            return;
        }

        var jsonLines = successSessions.Select(p => p.GetItemJson(_lastContext));
        await File.WriteAllLinesAsync(file.Path, jsonLines);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.ExportSuccess), InfoType.Success));
    }

    private void OnGenerateTimerTick(object? sender, object e)
    {
        // 检查当前的 Sessions，如果全部完成（没有 State = InfoType.Loading，且没有 State = InfoType.Information），则停止计时器.
        if (Sessions.All(p => p.State != InfoType.Loading && p.State != InfoType.Information))
        {
            _generateTimer.Stop();
            IsGenerating = false;
            IsFinished = true;
            return;
        }

        // 检查当前的 Sessions，如果当前处于 `State = InfoType.Loading` 的任务不满10个，则检查是否有处于 `State = InfoType.Information` 的会话，如果有，则调用其 StartCommand，直到补全10个.
        var loadingSessions = Sessions.Where(p => p.State == InfoType.Loading).ToList();
        if (loadingSessions.Count < 10)
        {
            foreach (var session in Sessions.Where(p => p.State == InfoType.Information).ToList())
            {
                session.StartCommand.Execute(null);
                var loadingCount = Sessions.Count(p => p.State == InfoType.Loading);
                if (loadingCount >= 10)
                {
                    break;
                }
            }
        }
    }
}
