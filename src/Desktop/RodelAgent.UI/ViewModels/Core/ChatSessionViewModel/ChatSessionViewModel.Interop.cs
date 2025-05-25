// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Microsoft.Web.WebView2.Core;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.Models;
using RodelAgent.Models.Feature;
using RodelAgent.Statics;
using RodelAgent.UI.Toolkits;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace RodelAgent.UI.ViewModels.Core;

public sealed partial class ChatSessionViewModel
{
    private const string ID_Loaded = "loaded";

    [RelayCommand]
    private async Task AddInteropMessageAsync(ChatMessage message)
    {
        if (!IsWebInitialized)
        {
            return;
        }

        var interopMessage = new ChatWebInteropMessage(message.ToInteropMessage());
        if (IsAgent && message.Role == ChatRole.Assistant)
        {
            ApplyAgentMessage(CurrentAgent!, interopMessage);
        }
        else if (IsGroup)
        {
            var agent = Agents.FirstOrDefault(p => p.Data.Id == message.AuthorName);
            if (agent != null)
            {
                ApplyAgentMessage(agent.Data, interopMessage);
            }
        }

        var messageJson = JsonSerializer.Serialize(interopMessage, JsonGenContext.Default.ChatWebInteropMessage);
        await _webView!.ExecuteScriptAsync($"window.addMessage({messageJson})");
    }

    [RelayCommand]
    private async Task SetInitialInteropHistoryAsync(List<ChatInteropMessage> history)
    {
        if (!IsWebInitialized)
        {
            return;
        }

        var interopHistory = new List<ChatWebInteropMessage>();
        foreach (var item in history)
        {
            var msg = new ChatWebInteropMessage(item);
            if (IsAgent && msg.Role == "assistant")
            {
                ApplyAgentMessage(CurrentAgent!, msg);
            }
            else if (IsGroup)
            {
                var agent = Agents.FirstOrDefault(p => p.Data.Id == msg.AgentId);
                if (agent != null)
                {
                    ApplyAgentMessage(agent.Data, msg);
                }
            }

            interopHistory.Add(msg);
        }

        var messageJson = JsonSerializer.Serialize(interopHistory, JsonGenContext.Default.ListChatWebInteropMessage);
        await _webView!.ExecuteScriptAsync($"window.setHistory({messageJson})");
    }

    [RelayCommand]
    private async Task ReloadWebThemeAsync()
    {
        if (!IsWebInitialized)
        {
            return;
        }

        var theme = this.Get<IAppToolkit>().GetCurrentTheme();
        await _webView!.ExecuteScriptAsync($"window.changeTheme('{theme.ToString().ToLowerInvariant()}')");
    }

    [RelayCommand]
    private async Task SetTempResultAsync(string msg)
    {
        if (!IsWebInitialized)
        {
            return;
        }

        var interopMsg = new ChatInteropMessage
        {
            Message = msg,
            Role = "assistant",
        };

        var messageJson = JsonSerializer.Serialize(interopMsg, JsonGenContext.Default.ChatInteropMessage);
        await _webView!.ExecuteScriptAsync($"window.setOutput({messageJson})");
    }

    [RelayCommand]
    private async Task SetTempLoadingAsync(bool isLoading)
    {
        if (!IsWebInitialized)
        {
            return;
        }

        await _webView!.ExecuteScriptAsync($"window.setLoading({isLoading.ToString().ToLowerInvariant()})");
    }

    [RelayCommand]
    private async Task SetInteropResourcesAsync()
    {
        if (!IsWebInitialized)
        {
            return;
        }

        var resources = new ChatInteropResources
        {
            Copy = ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.Copy),
            Save = ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.Save),
            Delete = ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.Delete),
            Discard = ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.Discard),
            Edit = ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.Edit),
            ThoughtProcess = ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.ThoughtProcess),
            Thinking = ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.Thinking),
        };

        var resourcesJson = JsonSerializer.Serialize(resources, JsonGenContext.Default.ChatInteropResources);
        await _webView!.ExecuteScriptAsync($"window.setResources({resourcesJson})");
    }

    [RelayCommand]
    private async Task CancelTempResultAsync()
    {
        if (!IsWebInitialized)
        {
            return;
        }

        await _webView!.ExecuteScriptAsync("window.setCancel()");
    }

    [RelayCommand]
    private async Task ClearMessageAsync()
    {
        if (await ClearMessageInternalAsync())
        {
            Messages.Clear();
            if (_currentConversation != null)
            {
                _currentConversation.History?.Clear();
                await _storageService.AddOrUpdateChatConversationAsync(_currentConversation);
            }

            RequestFocusInput?.Invoke(this, EventArgs.Empty);
        }
    }

    [RelayCommand]
    private async Task DeleteInteropMessageAsync(string id)
    {
        if (!IsWebInitialized)
        {
            return;
        }

        await _webView!.ExecuteScriptAsync($"window.deleteMessage(\'{id}\')");
    }

    [RelayCommand]
    private async Task AddNewSessionAsync()
    {
        if (IsGroup)
        {
            // TODO: Add group session.
            return;
        }

        if (_currentConversation != null)
        {
            await _storageService.AddOrUpdateChatConversationAsync(_currentConversation);
        }

        SetCurrentConversation(null);
        RequestFocusInput?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyAgentMessage(ChatAgent agent, ChatWebInteropMessage interopMessage)
    {
        interopMessage.Author = agent.Name ?? string.Empty;
        if (string.IsNullOrEmpty(agent.Emoji))
        {
            var avatarPath = AppToolkit.GetPresetAvatarPath(agent.Id);
            if (File.Exists(avatarPath))
            {
                interopMessage.Avatar = $"http://work.example/Avatars/{agent.Id}.png";
            }
        }
        else
        {
            var emoji = EmojiStatics.GetEmojis().Find(x => x.Unicode == agent.Emoji);
            interopMessage.Emoji = emoji?.ToEmoji();
        }

        interopMessage.ShowLogo = (!string.IsNullOrEmpty(interopMessage.Avatar) || !string.IsNullOrEmpty(interopMessage.Emoji)) && IsGroup;
    }

    private async Task<bool> ClearMessageInternalAsync()
    {
        if (!IsWebInitialized)
        {
            return false;
        }

        await _webView!.ExecuteScriptAsync("window.clearMessages()");
        return true;
    }

    private async void OnWebMessageReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
    {
        var msg = args.TryGetWebMessageAsString();
        if (msg.StartsWith("msg:", StringComparison.OrdinalIgnoreCase))
        {
            var msgText = msg[4..];
            if (msgText == ID_Loaded)
            {
                IsWebInitializing = false;
                IsWebInitialized = true;

                ReloadWebThemeCommand.Execute(default);
                SetInteropResourcesCommand.Execute(default);
                if (_currentConversation != null)
                {
                    SetCurrentConversation(_currentConversation);
                    SetInitialInteropHistoryCommand.Execute(_currentConversation?.History ?? []);
                }
            }
        }
        else if (msg.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            var contentStr = msg[5..];
            var data = JsonSerializer.Deserialize(contentStr, JsonGenContext.Default.WebDataObject);
            if (data!.Type == "copy")
            {
                var dp = new DataPackage();
                dp.SetText(data.Content);
                Clipboard.SetContent(dp);
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.Copied), InfoType.Success));
            }
            else if (data!.Type == "edit")
            {
                var editedData = JsonSerializer.Deserialize(data.Content, JsonGenContext.Default.EditedInteropMessage);
                if (editedData != null)
                {
                    var targetMsg = Messages.FirstOrDefault(p => p.Id == editedData.Id);
                    if (targetMsg != null)
                    {
                        targetMsg.Message = editedData.Message;
                        await SaveCurrentMessagesAsync();
                        CalcTotalTokenCountCommand.Execute(default);
                    }
                }
            }
            else if (data.Type == "delete")
            {
                var id = data.Content;
                var source = Messages.FirstOrDefault(p => p.Id == id);
                if (source != null)
                {
                    Messages.Remove(source);
                }

                await SaveCurrentMessagesAsync();
            }
            else if (data.Type == "copyText")
            {
                var text = data.Content;
                var dp = new DataPackage();
                dp.SetText(text);
                Clipboard.SetContent(dp);
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(UI.Models.Constants.StringNames.Copied), InfoType.Success));
            }
        }
    }

    private async void OnNavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
    {
        if (!args.Uri.Contains(".example", StringComparison.Ordinal))
        {
            args.Cancel = true;
            await Launcher.LaunchUriAsync(new Uri(args.Uri));
        }
    }
}
