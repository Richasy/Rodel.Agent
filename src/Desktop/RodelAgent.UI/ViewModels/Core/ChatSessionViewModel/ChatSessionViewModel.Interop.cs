// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Microsoft.Web.WebView2.Core;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.Models;
using RodelAgent.Models.Common;
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
        if (!IsWebInitialized)
        {
            return;
        }

        await _webView!.ExecuteScriptAsync("window.clearMessages()");
        _currentHistory.Clear();
        // TODO: 更新数据库.
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
                    var targetMsg = _currentHistory.FirstOrDefault(p => p.AdditionalProperties!.GetValueOrDefault("id", string.Empty)!.ToString() == editedData.Id);
                    if (targetMsg != null)
                    {
                        targetMsg.Text = editedData.Message;
                    }
                }
            }
            else if (data.Type == "delete")
            {
                var id = data.Content;
                var source = _currentHistory.FirstOrDefault(p => p.AdditionalProperties!.GetValueOrDefault("id", string.Empty)!.ToString() == id);
                if (source != null)
                {
                    _currentHistory.Remove(source);
                }
            }
        }
    }
}
