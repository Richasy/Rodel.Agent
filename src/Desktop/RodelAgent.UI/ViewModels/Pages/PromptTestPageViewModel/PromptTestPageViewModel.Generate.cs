// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 提示词测试页面视图模型.
/// </summary>
public sealed partial class PromptTestPageViewModel
{
    [RelayCommand]
    private async Task GenerateAsync()
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
            var singleSession = new PromptTestSessionItemViewModel(_chatClient, 0, preset, ReplaceUserInputToLastMessage(input));
            Sessions = [singleSession];
        }
        else
        {
            foreach (var input in _inputs)
            {
                var session = new PromptTestSessionItemViewModel(_chatClient, 0, preset, ReplaceUserInputToLastMessage(input));
                Sessions.Add(session);
            }
        }

        await Task.CompletedTask;

        string ReplaceUserInputToLastMessage(string input)
        {
            var lastMessageContent = lastUserMessage.GetFirstTextContent();
            return string.IsNullOrEmpty(_defaultInputVariable)
                ? lastMessageContent
                : lastMessageContent.Replace($"${_defaultInputVariable}", input);
        }
    }
}
