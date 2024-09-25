// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using System.Text.RegularExpressions;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelChat.Models.Client;
using Windows.Storage;
using Windows.System;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 提示词测试页面视图模型.
/// </summary>
public sealed partial class PromptTestPageViewModel
{
    private static async Task<List<string>> ParseJsonLFileAsync(StorageFile file)
    {
        if (Path.GetExtension(file.Path) != ".jsonl")
        {
            return null;
        }

        var inputs = new List<string>();
        var jsonLines = await FileIO.ReadLinesAsync(file);
        foreach (var line in jsonLines)
        {
            var json = JsonDocument.Parse(line);
            var root = json.RootElement;
            if (root.TryGetProperty("question", out var textElement))
            {
                inputs.Add(textElement.GetString());
            }
        }

        return inputs;
    }

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
    private async Task ImportVariablesAsync()
    {
        var jsonFile = await FileToolkit.PickFileAsync(".json", this.Get<AppViewModel>().ActivatedWindow);
        if (jsonFile is StorageFile file)
        {
            PresetVariablesFilePath = file.Path;
            SettingsToolkit.WriteLocalSetting(SettingNames.PromptTestPresetVariablesFilePath, PresetVariablesFilePath);
            Variables.Clear();
            _variables?.Clear();
            await ParsePresetVariablesAsync(PresetVariablesFilePath);
        }
    }

    [RelayCommand]
    private async Task ImportInputsAsync()
    {
        var jsonFile = await FileToolkit.PickFileAsync(".json,.jsonl", this.Get<AppViewModel>().ActivatedWindow);
        if (jsonFile is StorageFile file)
        {
            InputFilePath = file.Path;
            SettingsToolkit.WriteLocalSetting(SettingNames.PromptTestInputFilePath, InputFilePath);
            await ParseInputAsync(InputFilePath);
        }
    }

    [RelayCommand]
    private async Task UpdatePromptVariablesAsync()
    {
        if (string.IsNullOrEmpty(UserPromptTemplate))
        {
            Variables.Clear();
            return;
        }

        try
        {
            // 匹配 $ 后跟随一个或多个大写字母，直到遇到空格或者字符串结尾
            var regex = new Regex(@"\$(?<variable>[A-Z]+)");
            var matches = regex.Matches(UserPromptTemplate);
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

                    if (_variables?.Count > 0 && _variables.ContainsKey(name))
                    {
                        var values = _variables[name];
                        var variable = new VariableItemViewModel(name, values, values.FirstOrDefault());
                        variables.Add(variable);
                    }
                    else
                    {
                        var variable = new VariableItemViewModel(name);
                        variables.Add(variable);
                    }
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
                var original = Variables.FirstOrDefault(p => p.Name == variable.Name);
                if (original is null)
                {
                    Variables.Add(variable);
                }
                else if (original.Values?.Count != variable.Values?.Count)
                {
                    original.Values = variable.Values;
                    original.Value = variable.Value;
                }
            }

            var firstTextVariable = Variables.FirstOrDefault(p => p.Values is null);
            _defaultInputVariable = firstTextVariable?.Name ?? string.Empty;
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

    [RelayCommand]
    private async Task SaveHistoryJsonAsync()
    {
        if (_predefinedMessages is null)
        {
            return;
        }

        try
        {
            for (var i = 0; i < SystemPrompts.Count; i++)
            {
                var targetMessage = _predefinedMessages.Where(p => p.Role == RodelChat.Models.Constants.MessageRole.System).ElementAtOrDefault(i);
                if (targetMessage is ChatMessage message)
                {
                    message.Content.First().Text = SystemPrompts[i].Content;
                }
            }

            var lastUserMessage = _predefinedMessages.LastOrDefault(p => p.Role == RodelChat.Models.Constants.MessageRole.User);
            if (lastUserMessage is ChatMessage userMessage)
            {
                userMessage.Content.First().Text = UserPromptTemplate;
            }

            var json = JsonSerializer.Serialize(_predefinedMessages, new JsonSerializerOptions { WriteIndented = true });
            var file = await FileToolkit.SaveFileAsync(".json", this.Get<AppViewModel>().ActivatedWindow);
            await FileIO.WriteTextAsync(file, json);
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Saved), InfoType.Success));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save history file.");
            await this.Get<AppViewModel>().ShowMessageDialogAsync(ex.Message);
        }
    }

    [RelayCommand]
    private async Task OpenInputFileAsync()
        => await Launcher.LaunchFileAsync(await StorageFile.GetFileFromPathAsync(InputFilePath));

    [RelayCommand]
    private async Task OpenVariablesFileAsync()
        => await Launcher.LaunchFileAsync(await StorageFile.GetFileFromPathAsync(PresetVariablesFilePath));

    [RelayCommand]
    private async Task OpenHistoryFileAsync()
        => await Launcher.LaunchFileAsync(await StorageFile.GetFileFromPathAsync(MessageJsonFilePath));

    [RelayCommand]
    private void RemoveInput()
    {
        InputFilePath = string.Empty;
        SettingsToolkit.WriteLocalSetting(SettingNames.PromptTestInputFilePath, InputFilePath);
        _inputs.Clear();
        InputsCount = 0;
    }

    [RelayCommand]
    private void CheckDefaultInputVariable()
    {
        if (string.IsNullOrEmpty(_defaultInputVariable))
        {
            return;
        }

        var shouldDisableDefaultInputVariable = _inputs?.Count > 0;
        var defaultInputVariable = Variables.FirstOrDefault(p => p.Name == _defaultInputVariable);
        if (defaultInputVariable is not null)
        {
            defaultInputVariable.IsEnabled = !shouldDisableDefaultInputVariable;
        }
    }

    [RelayCommand]
    private Task ReloadMessagesAsync()
        => ParseHistoryAsync(MessageJsonFilePath, true);

    private async Task ParseInputAsync(string filePath, bool showTip = true)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(filePath);
            if (Path.GetExtension(filePath) == ".jsonl")
            {
                _inputs = await ParseJsonLFileAsync(file);
            }
            else
            {
                var json = await FileIO.ReadTextAsync(file);
                _inputs = JsonSerializer.Deserialize<List<string>>(json);
            }

            InputsCount = _inputs.Count;
            if (showTip)
            {
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.InputsImported), InfoType.Success));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse input file.");
            if (showTip)
            {
                await this.Get<AppViewModel>().ShowMessageDialogAsync(ex.Message);
            }
        }
    }

    private async Task ParsePresetVariablesAsync(string filePath)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(filePath);
            var json = await FileIO.ReadTextAsync(file);
            _variables = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
            PresetVariablesCount = _variables.Count;
            UpdatePromptVariablesCommand.Execute(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse variables file.");
            await this.Get<AppViewModel>().ShowMessageDialogAsync(ex.Message);
        }
    }

    private async Task ParseHistoryAsync(string filePath, bool showTip = true)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(filePath);
            var json = await FileIO.ReadTextAsync(file);
            var messages = JsonSerializer.Deserialize<List<ChatMessage>>(json);
            if (messages is List<ChatMessage> chatMessages && chatMessages.Count > 0)
            {
                _predefinedMessages = chatMessages;

                var systemMessages = chatMessages.Where(p => p.Role == RodelChat.Models.Constants.MessageRole.System).ToList();
                var systemPrompts = new List<PromptTestSystemPromptItemViewModel>();
                for (var i = 0; i < systemMessages.Count; i++)
                {
                    systemPrompts.Add(new PromptTestSystemPromptItemViewModel(i, systemMessages[i].GetFirstTextContent()));
                }

                SystemPrompts = systemPrompts;
                CurrentSystemPrompt = SystemPrompts.FirstOrDefault();

                var lastUserMessage = chatMessages.LastOrDefault(p => p.Role == RodelChat.Models.Constants.MessageRole.User);
                UserPromptTemplate = lastUserMessage?.GetFirstTextContent() ?? string.Empty;
                UpdatePromptVariablesCommand.Execute(default);

                var tip = ResourceToolkit.GetLocalizedString(StringNames.HistoryImported);
                tip = string.Format(tip, _predefinedMessages.Count);

                if (showTip)
                {
                    this.Get<AppViewModel>().ShowTipCommand.Execute((tip, InfoType.Success));
                }

                SystemPromptInitialzied?.Invoke(this, EventArgs.Empty);
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
}
