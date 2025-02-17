// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelChat.Models.Client;
using RodelChat.Models.Constants;
using Windows.ApplicationModel.DataTransfer;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 聊天消息项视图模型.
/// </summary>
public sealed partial class ChatMessageItemViewModel : ViewModelBase<ChatMessage>
{
    private readonly Func<ChatMessage, Task> _editFunc;
    private readonly Func<ChatMessage, Task> _deleteFunc;

    [ObservableProperty]
    private string _content;

    [ObservableProperty]
    private bool _isUser;

    [ObservableProperty]
    private bool _isAssistant;

    [ObservableProperty]
    private string _time;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _author;

    [ObservableProperty]
    private string _agentId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageItemViewModel"/> class.
    /// </summary>
    public ChatMessageItemViewModel(
        ChatMessage message,
        Func<ChatMessage, Task> editFunc,
        Func<ChatMessage, Task> deleteFunc)
        : base(message)
    {
        AgentId = message.AuthorId ?? string.Empty;
        Author = message.Author ?? string.Empty;
        Content = message.Content.FirstOrDefault(p => p.Type == ChatContentType.Text)?.Text ?? string.Empty;
        IsAssistant = message.Role == MessageRole.Assistant;
        IsUser = message.Role == MessageRole.User;
        if (message.Time != null)
        {
            Time = message.Time.Value.ToLocalTime().ToString("MM/dd HH:mm:ss");
        }

        _editFunc = editFunc;
        _deleteFunc = deleteFunc;
    }

    [RelayCommand]
    private void Copy()
    {
        var dp = new DataPackage();
        dp.SetText(Content);
        Clipboard.SetContent(dp);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
    }

    [RelayCommand]
    private async Task EditAsync()
    {
        if (string.IsNullOrEmpty(Content))
        {
            Delete();
            return;
        }

        var firstText = Data.Content.FirstOrDefault(p => p.Type == ChatContentType.Text);
        if (firstText != null)
        {
            firstText.Text = Content;
        }

        await _editFunc?.Invoke(Data);
    }

    [RelayCommand]
    private void Delete()
        => _deleteFunc?.Invoke(Data);
}
