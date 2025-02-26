// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Microsoft.Web.WebView2.Core;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.Models;
using RodelAgent.Models.Feature;
using RodelAgent.UI.Toolkits;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;

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

        var interopMessage = message.ToInteropMessage();
        var messageJson = JsonSerializer.Serialize(interopMessage, JsonGenContext.Default.ChatInteropMessage);
        await _webView!.ExecuteScriptAsync($"window.addMessage({messageJson})");
    }

    [RelayCommand]
    private async Task SetInitialInteropHistoryAsync(List<ChatMessage> history)
    {
        if (!IsWebInitialized)
        {
            return;
        }

        var messages = history.ConvertAll(p => p.ToInteropMessage());
        var messageJson = JsonSerializer.Serialize(messages, JsonGenContext.Default.ListChatInteropMessage);
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

        if (Messages.Count > 0)
        {
            if (await ClearMessageInternalAsync())
            {
                Messages.Clear();
            }
        }

        _currentConversation = default;
        RequestFocusInput?.Invoke(this, EventArgs.Empty);
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

    private void OnWebMessageReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
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
            }
        }
    }
}
