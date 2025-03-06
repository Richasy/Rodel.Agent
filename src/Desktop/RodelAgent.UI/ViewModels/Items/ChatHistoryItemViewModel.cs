// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.Models.Feature;
using RodelAgent.UI.Controls.Chat;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;
using System.Globalization;

namespace RodelAgent.UI.ViewModels.Items;

public sealed partial class ChatHistoryItemViewModel : ViewModelBase
{
    private const string GenerateTitleFunctionDefinition = """
        You are a title generator. You will get the first chat message in the chat history, please generate a title for this conversation,
        which must be concise and express the core meaning of the first chat message.
        
        Need response with {{LANGUAGE}}, Try to keep it as simple as possible and don't require punctuation.
        
        Here is the first chat message:
        
        ```
        {{INPUT}}
        ```
        """;

    private readonly Func<ChatHistoryItemViewModel, Task> _removeFunc;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatHistoryItemViewModel"/> class.
    /// </summary>
    public ChatHistoryItemViewModel(ChatConversation conversation, Func<ChatHistoryItemViewModel, Task> removeFunc)
    {
        Conversation = conversation;
        Id = conversation.Id;
        Update();
        _removeFunc = removeFunc;
    }

    /// <summary>
    /// 显示名称.
    /// </summary>
    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    [ObservableProperty]
    public partial string? LastMessageDate { get; set; }

    [ObservableProperty]
    public partial bool IsRenaming { get; set; }

    internal ChatConversation? Conversation { get; set; }

    internal string? Id { get; set; }

    public override bool Equals(object? obj) => obj is ChatHistoryItemViewModel model && Id == model.Id;

    public override int GetHashCode() => HashCode.Combine(Id);

    public long GetLastMessageTime()
    {
        return Conversation?.History?.Count > 0
            ? Conversation.History.Last().Time
            : 0;
    }

    public void Update()
    {
        Name = Conversation?.Name ?? string.Empty;
        LastMessageDate = Conversation?.History?.Count > 0
                ? DateTimeOffset.FromUnixTimeSeconds(Conversation.History.Last().Time).ToString("MM/dd")
                : string.Empty;
    }

    [RelayCommand]
    private async Task RenameAsync()
    {
        var dialog = new ChatHistoryRenameDialog(this);
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            Conversation!.Name = dialog.NewTitle;
            Update();
            await this.Get<IStorageService>().AddOrUpdateChatConversationAsync(Conversation);
        }
    }

    [RelayCommand]
    private async Task SmartRenameAsync()
    {
        var firstMsg = Conversation?.History?.FirstOrDefault(p => p.Role is not "system");
        if (firstMsg is null)
        {
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.NoValidMessage), InfoType.Error));
            return;
        }

        var sessionVM = this.Get<ChatSessionViewModel>();
        var currentLan = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var prompt = GenerateTitleFunctionDefinition
            .Replace("{{LANGUAGE}}", currentLan, StringComparison.Ordinal)
            .Replace("{{INPUT}}", firstMsg.Message, StringComparison.Ordinal);
        try
        {
            IsRenaming = true;
            var newTitle = await sessionVM.GenerateContentAsync(prompt);
            if (!string.IsNullOrEmpty(newTitle))
            {
                Conversation!.Name = newTitle;
                Update();
                if (IsSelected)
                {
                    sessionVM.Title = newTitle;
                }

                await this.Get<IStorageService>().AddOrUpdateChatConversationAsync(Conversation);
            }
        }
        catch (Exception ex)
        {
            this.Get<ILogger<ChatHistoryItemViewModel>>().LogError(ex, "Failed to generate title.");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ex.Message, InfoType.Error));
        }

        IsRenaming = false;
    }

    [RelayCommand]
    private async Task RemoveAsync()
    {
        if (_removeFunc is not null)
        {
            await _removeFunc(this);
        }
    }
}
