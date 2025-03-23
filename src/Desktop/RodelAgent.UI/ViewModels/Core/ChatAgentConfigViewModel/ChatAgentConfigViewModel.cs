// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Richasy.AgentKernel;
using Richasy.AgentKernel.Chat;
using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.Interfaces;
using RodelAgent.Models.Feature;
using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.View;
using System.Text.RegularExpressions;
using Windows.Storage;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// 聊天预设视图模型.
/// </summary>
public sealed partial class ChatAgentConfigViewModel : ViewModelBase
{
    public ChatAgentConfigViewModel(IStorageService storageService)
    {
        _storageService = storageService;
        StepCount = 2;
        CheckMessageCount();
        this.Get<AppViewModel>().RequestReloadChatServices += (_, _) => ReloadAvailableServicesCommand.Execute(default);
    }

    public void InjectFunc(
        Func<ChatOptions> func,
        Func<bool>? stream,
        Func<int>? maxRounds)
    {
        _getCurrentOptions = func;
        _getIsStreamOutput = stream;
        _getMaxRounds = maxRounds;
    }

    public void InjectFunc(
        Func<string>? getEmoji)
    {
        _getEmoji = getEmoji;
    }

    public void SetData(ChatAgentItemViewModel data)
    {
        IsManualClose = false;
        Agent = data.Data;
        Name = data.Name;
        SystemInstruction = Agent.SystemInstruction;
        StopSequences.Clear();
        FilterCharacters.Clear();
        Messages.Clear();
        if (Agent.Options?.StopSequences != null)
        {
            foreach (var item in Agent.Options.StopSequences)
            {
                StopSequences.Add(item);
            }
        }

        if (Agent.FilterCharacters != null)
        {
            foreach (var item in Agent.FilterCharacters)
            {
                FilterCharacters.Add(item);
            }
        }

        CurrentStep = 0;
        CheckStep();

        if (Agent.History != null)
        {
            foreach (var item in Agent.History)
            {
                Messages.Add(item);
            }
        }

        if (AvailableServices.Count == 0)
        {
            ReloadAvailableServicesCommand.Execute(default);
        }

        CheckMaxTurnEnabled();
        CheckMessageCount();
        SelectService(AvailableServices.FirstOrDefault(x => x.ProviderType == data.Data.Provider) ?? AvailableServices.First());
    }

    [RelayCommand]
    private async Task ReloadAvailableServicesAsync()
    {
        var providers = Enum.GetValues<ChatProviderType>();
        var services = new List<ChatServiceItemViewModel>();
        var chatConfigManager = this.Get<IChatConfigManager>();
        foreach (var p in providers)
        {
            var config = await chatConfigManager.GetChatConfigAsync(p);
            if (config?.IsValid() == true)
            {
                services.Add(new ChatServiceItemViewModel(p));
            }
        }

        AvailableServices.Clear();
        foreach (var item in services)
        {
            AvailableServices.Add(item);
        }
    }

    [RelayCommand]
    private async Task ReloadAvailableModelsAsync()
    {
        Models.Clear();
        SelectedModel = null;
        var chatService = this.Get<IChatService>(SelectedService!.ProviderType.ToString());
        var serverModels = chatService!.GetPredefinedModels().ToList();
        var config = await this.Get<IChatConfigManager>().GetChatConfigAsync(SelectedService!.ProviderType);
        var customModels = config?.CustomModels?.ToList() ?? [];
        var models = serverModels.Concat(customModels).ToList().ConvertAll(p => new ChatModelItemViewModel(p));
        models.ForEach(Models.Add);
        SelectedModel = Models.FirstOrDefault(x => x.Data.Id == Agent?.Model) ?? Models.First();
    }

    [RelayCommand]
    private void SelectService(ChatServiceItemViewModel? service)
    {
        SelectedService = service;
        ReloadAvailableModelsCommand.Execute(default);
        RequestReloadOptionsUI?.Invoke(this, EventArgs.Empty);
        CheckNextStepEnabled();
    }

    [RelayCommand]
    private void GoNext()
    {
        if (!IsLastStep)
        {
            CurrentStep++;
        }
    }

    [RelayCommand]
    private void GoPrev()
        => CurrentStep--;

    [RelayCommand]
    private async Task SaveAsync()
    {
        UpdatePresetData();
        await _storageService.AddOrUpdateChatAgentAsync(Agent!);
        this.Get<ChatPageViewModel>().ReloadAvailableAgentsCommand.Execute(default);
        this.Get<ChatSessionViewModel>().ForceReloadLogoCommand.Execute(default);
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task ImportHistoryAsync()
    {
        var xmlFile = await this.Get<IFileToolkit>().PickFileAsync(".xml", this.Get<AppViewModel>().ActivatedWindow);
        if (xmlFile != null)
        {
            try
            {
                var xmlContent = await FileIO.ReadTextAsync(xmlFile);
                var messages = new List<ChatInteropMessage>();
                var regex = new Regex(@"<message([^>]*)>(.*?)<\/message>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                foreach (Match match in regex.Matches(xmlContent))
                {
                    var element = new ChatInteropMessage
                    {
                        Message = match.Groups[2].Value.Trim()
                    };

                    var attributes = match.Groups[1].Value;
                    var attrRegex = new Regex(@"(\w+)\s*=\s*""([^""]*)""", RegexOptions.IgnoreCase);
                    foreach (Match attrMatch in attrRegex.Matches(attributes))
                    {
                        var key = attrMatch.Groups[1].Value;
                        var value = attrMatch.Groups[2].Value;
                        if (key.Equals("role", StringComparison.OrdinalIgnoreCase))
                        {
                            element.Role = value;
                        }
                        else if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
                        {
                            element.Id = value;
                        }
                    }

                    if (string.IsNullOrEmpty(element.Id))
                    {
                        element.Id = Guid.NewGuid().ToString("N");
                    }

                    messages.Add(element);
                }

                Messages.Clear();
                foreach (var item in messages)
                {
                    Messages.Add(item);
                }

                CheckMessageCount();
            }
            catch (Exception ex)
            {
                this.Get<ILogger<ChatAgentConfigViewModel>>().LogError(ex, "Import history failed.");
                this.Get<AppViewModel>().ShowTipCommand.Execute((ex.Message, InfoType.Error));
            }
        }
    }

    private void UpdatePresetData()
    {
        var options = _getCurrentOptions?.Invoke() ?? new ChatOptions();
        var maxRounds = _getMaxRounds?.Invoke() ?? 0;
        var stream = _getIsStreamOutput?.Invoke() ?? true;
        var emoji = _getEmoji?.Invoke() ?? string.Empty;
        Agent ??= new ChatAgent();
        if (string.IsNullOrEmpty(Agent.Id))
        {
            Agent.Id = Guid.NewGuid().ToString("N");
        }

        if (!IsMaxRoundEnabled)
        {
            maxRounds = 0;
        }

        Agent.Name = Name!;
        Agent.SystemInstruction = SystemInstruction;
        Agent.Provider = SelectedService!.ProviderType;
        Agent.Model = SelectedModel!.Data.Id;
        Agent.History = Messages.Count > 0 ? [.. Messages] : default;
        Agent.MaxRounds = maxRounds;
        Agent.UseStreamOutput = stream;
        Agent.Emoji = emoji;
        if (StopSequences.Count > 0)
        {
            options.StopSequences = [.. StopSequences];
        }

        Agent.Options = options;

        if (FilterCharacters.Count > 0)
        {
            Agent.FilterCharacters = [.. FilterCharacters];
        }
    }

    private void CheckStep()
    {
        IsModelSelectionStep = CurrentStep == 0;
        IsPresetDetailStep = CurrentStep == 1;
        IsLastStep = CurrentStep == StepCount - 1;
        IsPreviousStepShown = CurrentStep > 0;
        CheckNextStepEnabled();
    }

    private void CheckNextStepEnabled()
        => IsNextButtonEnabled = (IsModelSelectionStep && !string.IsNullOrEmpty(Agent?.Name)) || !IsModelSelectionStep;

    private void CheckMessageCount()
    {
        IsMessageEmpty = Messages.Count == 0;
        CheckMaxTurnEnabled();
    }

    private void CheckMaxTurnEnabled()
    {
        if (!IsMessageEmpty && IsMaxRoundEnabled)
        {
            IsMaxRoundEnabled = false;
        }
        else if (IsMessageEmpty && !IsMaxRoundEnabled)
        {
            IsMaxRoundEnabled = true;
        }
    }

    partial void OnCurrentStepChanged(int value)
        => CheckStep();

    partial void OnNameChanged(string? value)
    {
        if (Agent != null)
        {
            Agent.Name = value ?? string.Empty;
        }

        CheckNextStepEnabled();
    }
}
