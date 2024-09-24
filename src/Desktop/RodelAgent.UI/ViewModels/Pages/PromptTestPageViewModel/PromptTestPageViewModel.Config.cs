// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using System.Text.RegularExpressions;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Client;
using Windows.Storage;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 提示词测试页面视图模型.
/// </summary>
public sealed partial class PromptTestPageViewModel
{
    [RelayCommand]
    private async Task ImportHistoryAsync()
    {
        var jsonFile = await FileToolkit.PickFileAsync(".json", this.Get<AppViewModel>().ActivatedWindow);
        if (jsonFile is StorageFile file)
        {
            MessageJsonFilePath = file.Path;
            SettingsToolkit.WriteLocalSetting(SettingNames.PromptTestMessageJsonFilePath, MessageJsonFilePath);
            await ParseHistoryAsync(MessageJsonFilePath);
        }
    }

    [RelayCommand]
    private async Task ImportSystemPromptAsync()
    {
        var txtFile = await FileToolkit.PickFileAsync(".txt", this.Get<AppViewModel>().ActivatedWindow);
        if (txtFile is StorageFile file)
        {
            SystemPromptFilePath = file.Path;
            SettingsToolkit.WriteLocalSetting(SettingNames.PromptTestSystemPromptPath, file.Path);
            await ParseSystemPromptAsync(SystemPromptFilePath);
        }
    }

    [RelayCommand]
    private async Task SaveSystemPromptAsync()
    {
        var file = await FileToolkit.SaveFileAsync(".txt", this.Get<AppViewModel>().ActivatedWindow);
        if (file is StorageFile storageFile)
        {
            await FileIO.WriteTextAsync(storageFile, SystemPrompt);
            SystemPromptFilePath = storageFile.Path;
            SettingsToolkit.WriteLocalSetting(SettingNames.PromptTestSystemPromptPath, SystemPromptFilePath);
        }
    }

    private async Task ParseHistoryAsync(string filePath, bool showTip = true)
    {
        var file = await StorageFile.GetFileFromPathAsync(filePath);
        var json = await FileIO.ReadTextAsync(file);
        try
        {
            var messages = JsonSerializer.Deserialize<List<ChatMessage>>(json);
            if (messages is List<ChatMessage> chatMessages && chatMessages.Count > 0)
            {
                _predefinedMessages = chatMessages;
                var tip = ResourceToolkit.GetLocalizedString(StringNames.HistoryImported);
                tip = string.Format(tip, _predefinedMessages.Count);

                if (showTip)
                {
                    this.Get<AppViewModel>().ShowTipCommand.Execute((tip, InfoType.Success));
                }
            }
            else
            {
                if (showTip)
                {
                    await this.Get<AppViewModel>().ShowMessageDialogAsync(ResourceToolkit.GetLocalizedString(StringNames.FailedToImportHistory));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse history file.");
            if (showTip)
            {
                await this.Get<AppViewModel>().ShowMessageDialogAsync(ex.Message);
            }
        }
        finally
        {
            MessageCount = _predefinedMessages?.Count ?? 0;
        }
    }

    private async Task ParseSystemPromptAsync(string filePath)
    {
        var file = await StorageFile.GetFileFromPathAsync(filePath);
        var text = await FileIO.ReadTextAsync(file);
        SystemPrompt = text;
        await UpdatePromptVariablesAsync();
    }

    [RelayCommand]
    private async Task UpdatePromptVariablesAsync()
    {
        try
        {
            // 匹配 $ 后跟随一个或多个大写字母，直到遇到空格或者字符串结尾
            var regex = new Regex(@"\$(?<variable>[A-Z]+)");
            var matches = regex.Matches(SystemPrompt);
            var variables = new List<VariableItemViewModel>();
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    var name = match.Groups["variable"].Value;
                    if (variables.Any(p => p.Name == name))
                    {
                        continue;
                    }

                    var variable = new VariableItemViewModel { Name = name };
                    variables.Add(variable);
                }
            }

            for (var i = Variables.Count - 1; i >= 0; i--)
            {
                var variable = Variables[i];
                if (!variables.Any(p => p.Name == variable.Name))
                {
                    Variables.RemoveAt(i);
                }
            }

            foreach (var variable in variables)
            {
                if (!Variables.Any(p => p.Name == variable.Name))
                {
                    Variables.Add(variable);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse system prompt file.");
            await this.Get<AppViewModel>().ShowMessageDialogAsync(ex.Message);
        }
        finally
        {
            IsVariableEmpty = Variables.Count == 0;
        }
    }
}
